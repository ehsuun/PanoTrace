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

public class PlaneMaker : MonoBehaviour {
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private GameObject active;
    public GameObject planePrefab;
    private Vector2 lastPad;
    private Vector2 viveAxis;
    private SceneController sceneController;
    public GameObject wheel;
    public GameObject uiCanvas;
    public bool undoTarget = false;
    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

    }
	
    void OnEnable()
    {
        if (wheel) wheel.SetActive(true);
        if (uiCanvas) uiCanvas.SetActive(true);
        SendMessage("SetUndoTargetFalse"); // tells every body else to turn off undo
        undoTarget = true;
        active = GameObject.Instantiate(planePrefab, new Vector3(transform.position.x,0, transform.position.z), Quaternion.Euler(0, 0, 0)) as GameObject;
        active.layer = 8;
        sceneController = transform.parent.GetComponent<SceneController>();
        active.transform.parent = sceneController.drawMesh.transform;
        Wireframe w;
        w = active.gameObject.AddComponent<Wireframe>();
        w.enabled = sceneController.wMode;
        w.lineMaterial = sceneController.wireFrame;
        w.render_lines_1st = true;
        w.lineWidth = 20;
    }

	// Update is called once per frame
	void Update () {

        if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).magnitude > 0.0001f &&
             lastPad.magnitude > 0.0001f)
        {
            viveAxis = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) - lastPad;
        }
        if ((controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) - lastPad).magnitude < 0.0001f)
        {
            viveAxis = new Vector2(0, 0);
        }
        lastPad = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            this.enabled = false;
        }


        if (active)
        {
            if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).magnitude > 0.0001f)
            {
                active.transform.position += new Vector3(0f,viveAxis.y, 0f);
            }
        }









        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            FinishedDrawingPlane();
        }
	}

    void OnDisable()
    {
        if (wheel) wheel.SetActive(false);
        if (uiCanvas) uiCanvas.SetActive(false);
        if (active)
        {
            Destroy(active);
        }
        GetComponent<TouchUIController>().SetEnable(true);
        undoTarget = false;
    }

    void FinishedDrawingPlane()
    {
        GameObject.Find("MeshCenter").GetComponent<MeshCenter>().AddMesh(active.GetComponent<MeshFilter>());
        active = null;
        enabled = false;
    }

    void SetUndoTargetFalse()
    {
        if(!enabled)
        undoTarget = false;
    }

    void Undo()
    {
        if(undoTarget)
        GameObject.Find("MeshCenter").SendMessage("DeleteLast");
    }
}
