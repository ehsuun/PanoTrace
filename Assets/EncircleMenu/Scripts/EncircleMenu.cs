// Copyright (C) 2018 The Regents of the University of California (Regents).
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//
//     * Neither the name of The Regents or University of California nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Please contact the author of this library if you have any questions.
// Author: Ehsan Sayyad (ehsan@mat.ucsb.edu)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncircleMenu : MonoBehaviour {

    internal SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;
    public int inverseMeshQuality = 4;
    private int lastsides;
    internal bool isenabled = false;
    private bool wasenabled = false; // to prevent simultaneous clicking
    internal bool hideChildren = false;
    public Material menuMat;
    public EncircleSubMenu[] submenus;

    public float transitionSpeed;
    public float extendedSize = 1.5f;
    public float shrunkenSize = 1;
    public float currentSize = 1;
    public float uiDistFromCenter = 0.8f;

    private int currentHover = -2;
    public float deltaAngle;
    private float lastAngle;
    private float deltaX;
    private float deltaY;
    private Vector2 pos = Vector2.zero;
    private Vector2 lastPos = Vector2.zero;

    // Use this for initialization
    void Start () {

        //this should be neated up!
        if (!trackedObj)
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
            if (!trackedObj)
            {
                trackedObj = transform.parent.GetComponent<SteamVR_TrackedObject>();
                if (!trackedObj)
                {
                    trackedObj = transform.parent.parent.GetComponent<SteamVR_TrackedObject>();
                    if (!trackedObj)
                    {
                        trackedObj = transform.parent.parent.parent.GetComponent<SteamVR_TrackedObject>();
                        if (!trackedObj)
                        {
                            trackedObj = transform.parent.parent.parent.parent.GetComponent<SteamVR_TrackedObject>();
                            if (!trackedObj)
                            {
                                trackedObj = transform.parent.parent.parent.parent.parent.GetComponent<SteamVR_TrackedObject>();
                            }
                        }
                    }
                }
            }
        }

        // if its the root menu we enable it
        if (transform.parent.GetComponent<EncircleMenu>())
        {
            isenabled = false;
        }
        else
        {
            isenabled = true;
        }

        lastsides = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (inverseMeshQuality < 1) inverseMeshQuality = 1;
        pos = Vector2.zero;
        bool pressed = false;
        bool down = false;
        bool up = false;
        if ((int)trackedObj.index != -1)
        {
             //pos = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
             pos = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
             pressed = controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Axis0);
             down = controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Axis0);
             up = controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Axis0);
        }

        float angle = Vector2.Angle(new Vector2(-1, 0), pos);
        if (pos.y < 0) angle = 360-angle;
        if (pos.magnitude < 0.0000001) angle = -1;
        if (wasenabled) {
            for(int i = 0; i < submenus.Length; i++)
            {
                if (360f*i/ submenus.Length < angle && angle<360f * (i+1) / submenus.Length)
                {
                    if (pressed) { submenus[i].OnPush(); }
                    else
                    {
                        // call unhover on all children except the one we are on it.
                        for (int j = 0; j < submenus.Length; j++)
                        {
                            if (j != i)
                            submenus[j].OnUnHover();
                        }
                        if(i!= currentHover)
                        {
                            submenus[i].OnHover();
                            controller.TriggerHapticPulse(500);
                            currentHover = i;
                        }
                        
                    }
                    if (down)
                    {
                        controller.TriggerHapticPulse(900);
                        submenus[i].OnDown();
                    }
                }
                else
                {
                   
                    submenus[i].OnIdle();
                }
                // if we just finished or started touching the pad, delta would be zero
                if (angle == -1 || lastAngle == -1)
                {
                    currentHover = -1;
                    deltaAngle = 0;
                    deltaX = 0;
                    deltaY = 0;
                }
                else

                {
                    deltaAngle = lastAngle - angle;
                    deltaX = lastPos.x - pos.x;
                    deltaY = lastPos.y - pos.y;
                }

                lastAngle = angle;
                lastPos = pos;
                submenus[i].deltaX = deltaX;
                submenus[i].deltaY = deltaY;
                submenus[i].angle = angle;
                submenus[i].deltaAngle = deltaAngle;
                submenus[i].touchX = pos.x;
                submenus[i].touchY = pos.y;
            }
        }

        if(lastsides!= inverseMeshQuality)
        {
            lastsides = inverseMeshQuality;

            if (hideChildren)
            {
                SetDisabled();
            }
        }

        wasenabled = isenabled;

    }

    void AddUI(Transform t,int index,int total)
    {
        Mesh m = new Mesh();
        //if for each 10 deg 1 vert
        // 360/4 = 90 // 90/10 = 9// 9 +2 = 11
        int verlength;
        verlength = ((360 / total) / inverseMeshQuality) + 3;
        
        Vector3[] verts = new Vector3[verlength];
        Vector2[] uvs = new Vector2[verts.Length];
        // 9 triangles in 90 = 9 * 3 ints

        int[] tris = new int[(verlength-2) *3]; 
        verts[0] = Vector3.zero;
        uvs[0] = Vector2.zero;

        for (int i = 1; i < verts.Length - 1; i++)
        {
            verts[i] = new Vector3(Mathf.Cos(((360f / total * index) + (i - 1) * inverseMeshQuality) * Mathf.Deg2Rad), Mathf.Sin(((360f / total * index) + (i - 1) * inverseMeshQuality) * Mathf.Deg2Rad), 0);
            uvs[i] = new Vector2(Mathf.Sin(Mathf.Deg2Rad * (90 - (i-1) * (90f / (verts.Length - 2f)))), Mathf.Cos(Mathf.Deg2Rad * (90 - (i-1) * (90f / (verts.Length - 2f)))));
        }
        verts[verts.Length - 1] = new Vector3(Mathf.Cos(((360f / total * (index + 1))) * Mathf.Deg2Rad), Mathf.Sin(((360f / total * (index + 1))) * Mathf.Deg2Rad), 0);
        uvs[verts.Length - 1] = Vector2.right;
        for (int i = 0; i < verlength - 2; i++)
        {
            tris[3*i] = 0; tris[3*i + 1] = i+1; tris[3*i + 2] = i+2;
        }

        m.vertices = verts;
        m.triangles = tris;
        m.uv = uvs;
        m.RecalculateBounds();
        m.RecalculateNormals();
        MeshFilter filter;
        Renderer renderer;

        if (t.gameObject.GetComponent<MeshFilter>()!= null)
        {
            filter = t.gameObject.GetComponent<MeshFilter>();
            renderer = t.gameObject.GetComponent<MeshRenderer>();
        }
        else
        {
            filter = t.gameObject.AddComponent<MeshFilter>();
            renderer = t.gameObject.AddComponent<MeshRenderer>();
        }

        //UI Placement
        if (t.GetComponent<EncircleSubMenu>().UI)
        {
            t.GetComponent<EncircleSubMenu>().UI.transform.localPosition = verts[Mathf.RoundToInt((verts.Length - 1) / 2)] * uiDistFromCenter
                + Vector3.forward * 0.01f;
                
        }

        foreach (Transform ch in t)
        {
            if (ch.GetComponent<EncircleSubMenu>())
            {
                if (!t.gameObject.GetComponent<EncircleMenu>()) { 
                    EncircleMenu menu =
                    t.gameObject.AddComponent<EncircleMenu>();
                    menu.menuMat = menuMat;
                    menu.trackedObj = trackedObj;
                    menu.hideChildren = true;
                    menu.inverseMeshQuality = inverseMeshQuality;
                    menu.UpdateUI();
                    menu.SetDisabled();
                    break;
                }else
                {
                    t.gameObject.GetComponent<EncircleMenu>().UpdateUI();
                }
            }
        }

        isenabled = true;

        filter.mesh = m;
        renderer.sharedMaterial = menuMat;

    }

    public void SetEnabled()
    {
        isenabled = true;
    }

    public void SetDisabled()
    {
        for(int i = 0; i < submenus.Length; i++)
        {
            submenus[i].zeroOut();
        }
        isenabled = false;
    }


    public void SetEnabledWithAnimation()
    {
        deltaAngle = 0;
        deltaX = 0;
        deltaY = 0;

        SetEnabled();
        StartCoroutine("EnableAnimation");
    }

    public void SetDisabledWithAnimation()
    {
        SetDisabled();
        StartCoroutine("DisableAnimation");
    }


    public IEnumerator EnableAnimation()
    {
        if (transitionSpeed != 0) { // we can set transition speed to 0 to disable rendering 
        foreach (EncircleSubMenu ch in submenus)
        {
            if (ch.GetComponent<Renderer>())
            {
                ch.GetComponent<Renderer>().enabled = true;
                    if(ch.UI)ch.UI.SetActive(true);
            }
        }

        while (currentSize + transitionSpeed / 100f < extendedSize)
        {
                foreach (EncircleSubMenu ch in submenus)
                {
                    ch.transform.localScale = new Vector3(currentSize + transitionSpeed / 100f,
                    currentSize + transitionSpeed / 100f,
                    currentSize + transitionSpeed / 100f);
                }
                currentSize += transitionSpeed / 100f;
            yield return null;
        }
            currentSize = extendedSize;
            foreach (EncircleSubMenu ch in submenus)
            {
                ch.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            }
      }
    }

    public IEnumerator DisableAnimation()
    {
        if (transitionSpeed != 0)
        {
            while (currentSize - transitionSpeed / 100f >shrunkenSize)
            {
                foreach (EncircleSubMenu ch in submenus)
                {
                    ch.transform.localScale = new Vector3(currentSize - transitionSpeed / 100f,
                    currentSize - transitionSpeed / 100f,
                    currentSize - transitionSpeed / 100f);
                }
                currentSize -= transitionSpeed / 100f;
                yield return null;
            }

            currentSize = shrunkenSize;
            foreach (EncircleSubMenu ch in submenus)
            {
                ch.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
                if (ch.GetComponent<Renderer>())
                {
                    ch.GetComponent<Renderer>().enabled = false;
                    if (ch.UI) ch.UI.SetActive(false);
                }
            }
        }
    }

    public void Back()
    {
        if (transform.parent.GetComponent<EncircleMenu>())
        {
            SetDisabled();
            transform.parent.GetComponent<EncircleMenu>().SetEnabled();
        }
    }

    public void UpdateUI()
    {

        List<EncircleSubMenu> temp = new List<EncircleSubMenu>();

        for (int i = 0; i < transform.childCount; i++)
        {
            EncircleSubMenu tempsub = transform.GetChild(i).GetComponent<EncircleSubMenu>();
            if (tempsub != null)
            {
                temp.Add(tempsub);
                transform.GetChild(i).localPosition = transform.position - transform.up * 0.01f;
            }

            
        }
        submenus = temp.ToArray();

        for (int i = 0; i < submenus.Length; i++)
        {
            AddUI(submenus[i].transform, i, submenus.Length);
        }

        // if its the root menu we enable it
        if (transform.parent.GetComponent<EncircleMenu>())
        {
            isenabled = false;
        }else
        {
            isenabled = true;
        }
    }

    public void GenerateUIContainers()
    {
        for (int i = 0; i < submenus.Length; i++)
        {
            if (!submenus[i].UI)
            {
                submenus[i].UI = new GameObject("UI" + submenus[i].name);
                submenus[i].UI.transform.parent = submenus[i].transform;
                submenus[i].UI.transform.localPosition = Vector3.zero;
                submenus[i].UI.transform.localEulerAngles = Vector3.zero;
                submenus[i].UI.transform.localScale = Vector3.one;
            }

            if (submenus[i].GetComponent<EncircleMenu>())
            {
                submenus[i].GetComponent<EncircleMenu>().GenerateUIContainers();
            }
        }
    }
}
