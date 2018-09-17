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

public class HelperCreator : MonoBehaviour {
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;
    private LineRenderer line;
    public GameObject helperPrefab;
    public GameObject wheel;
    public GameObject projector;
    public GameObject uiCanvas;
    private GameObject lastHelper;
    private bool confirmed = false;
    public List<GameObject> helpers;

    private float brushSize = 1f;
    private Vector2 lastTouch;
    private Vector2 touch;
    private float deltaX;
    private float stepper;
    private float step = 0.1f;
    private bool undoTarget = false;
    public Color hitCol;
    public Color misCol;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        line = GetComponent<LineRenderer>();
        if (!projector)
        {
            projector = GameObject.Find("Proj");
        }

        if (this.isActiveAndEnabled)
        {
            line.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        UpdateWheel();

        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            Disable();
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);
            line.SetColors(hitCol, hitCol);
        }
        else
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.forward*50 + transform.position);
            line.SetColors(misCol, misCol);
        }


        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            Vector3 look = projector.transform.position - hit.point;
            look = new Vector3(look.x, 0, look.z);

            if (!lastHelper)
            {
                lastHelper = GameObject.Instantiate(helperPrefab, hit.point , Quaternion.LookRotation(look,Vector3.up)) as GameObject;
                lastHelper.GetComponent<Renderer>().sharedMaterial = projector.GetComponent<TextureProjection>().target.GetComponent<Renderer>().sharedMaterial;
                lastHelper.layer = 8;
                confirmed = true;
                controller.TriggerHapticPulse(500);
            }
            else{
                if (confirmed)
                {
                    StartCoroutine(LongVibration(0.1f, 0.1f));
                    helpers.Add(lastHelper);
                    lastHelper = null;
                    confirmed = false;
                } 
            }
        }

        if (lastHelper) lastHelper.transform.localScale = Vector3.one * brushSize;

    }

    void UpdateWheel()
    {
        touch = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        if ((lastTouch - touch).magnitude < 0.2)
        {
            deltaX = touch.x - lastTouch.x;

        }
        else
        {
            deltaX = 0;
        }
        brushSize += deltaX * brushSize;
        brushSize = Mathf.Clamp(brushSize, 0.001f, 10);

        lastTouch = touch;
    }

    void OnEnable()
    {
        if (uiCanvas) uiCanvas.SetActive(true);
        if(line)line.enabled = true;
        wheel.transform.parent.parent.gameObject.SetActive(true);
        GetComponent<TouchUIController>().SetEnable(false);
        undoTarget = true;
    }

    void OnDisable()
    {
        if (uiCanvas) uiCanvas.SetActive(false);
        if (line) line.enabled = false;
        wheel.transform.parent.parent.gameObject.SetActive(false);
        undoTarget = false;
    }

    public void HelperEnable()
    {
        this.enabled = true;
    }

    void Disable()
    {
        GetComponent<TouchUIController>().SetEnable(true);
        this.enabled = false;
    }

    public void Undo()
    {
        if (undoTarget)
        {
            if (helpers.Count > 0)
            {
                Destroy(helpers[helpers.Count - 1].gameObject);
                helpers.RemoveAt(helpers.Count - 1);
            }
        }
    }

    IEnumerator LongVibration(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
        yield return null;
    }
}

}
