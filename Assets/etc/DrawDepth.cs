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

public class DrawDepth : MonoBehaviour {
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    public Texture2D tex;
    public Vector2 point;
    public Material targetMat;
    public float val;
    private SteamVR_TrackedController cnt;
    public GameObject uiCanvas;
    private LineRenderer line;
    public Color lineCol;
    public Color lineActive;
    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();
        tex = new Texture2D(32, 16,TextureFormat.RGBAFloat,false);
        var fillColorArray = tex.GetPixels();
        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = new Color(0.0f, 0.0f, 0.0f);
        }
        tex.SetPixels(fillColorArray);
        tex.Apply();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        cnt = GetComponent<SteamVR_TrackedController>();
        targetMat.SetTexture("_DepthDrawingTex", tex);

        line.SetWidth(0.1f, 0.1f);
        if (this.isActiveAndEnabled)
        {
            line.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update() {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hit.point);
            line.SetColors(lineCol, lineCol);
        }
        else
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.forward * 50 + transform.position);
            line.SetColors(lineCol, lineCol);
        }


        if (cnt.triggerPressed)
        {
            line.SetColors(lineActive, lineActive);
            point = dirtoSpherical(transform.forward);
            int x = (int)(point.x * tex.width); int y = (int)(point.y * tex.height);
            tex.SetPixel(x, y, tex.GetPixel(x,y) + new Color(val, val, val));
            tex.Apply();
            targetMat.SetTexture("_DepthDrawingTex", tex);
        }

        if (cnt.gripped)
        {
            DrawDepthDisable();
        }

	}

    void DrawDepthDisable()
    {
        this.enabled = false;
    }

    void OnEnable()
    {
        if(line) line.enabled = true;
        if (uiCanvas) uiCanvas.SetActive(true);

    }
    void OnDisable()
    {
        line.enabled = false;
        if (uiCanvas) uiCanvas.SetActive(false);
        GetComponent<TouchUIController>().SetEnable(true);

    }

    Vector2 dirtoSpherical(Vector3 xyz)
    {
        xyz = -xyz;
        float r = xyz.magnitude;
        xyz *= 1.0f / r;
        float theta = Mathf.Asin(xyz.y);
        float phi = Mathf.Atan2(xyz.z, xyz.x);
        phi += (phi < 0) ? 2 * Mathf.PI : 0;  // only if you want [0,2pi)

        Vector2 uv = new Vector2();

        uv.y = 1f - (theta / Mathf.PI + 0.5f);
        uv.x = 1f - phi / (2 * Mathf.PI);
        return uv;
    }
}
