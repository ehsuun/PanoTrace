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


[RequireComponent( typeof(LineRenderer))]
public class MeshCreator : MonoBehaviour {
    public GameObject meshPrefab;

    public bool shouldPlace;

    public bool getRayfromCamera;
    public bool snappingEnabled;
    internal Vector3 snapPoint;
    private Ray ray;
    public Transform rayPointer;

    ExtrudeMesh activeMesh;
    private LineRenderer line;
    public float lineWidth = 0.1f;
    private Vector3[] linePositions = new Vector3[2];

    public Color hitColor;
    public Color emptyColor;

    public GameObject canvasUI;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private Vector2 localDeltaEuler;
    private Vector3 lastLocalEuler;

    private Vector2 lastPad;
    public TextureProjection currentProjector;
    private bool undoTarget = false;


    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            Disable();
        }

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            snappingEnabled = !snappingEnabled;
            if (currentProjector)
            {
                if (snappingEnabled) {
                currentProjector.SendMessage("TrunOnCanny");
                }else
                {
                    currentProjector.SendMessage("TrunOffCanny");
                }
            }
        }

        if (snappingEnabled) {
            snapPoint = GetComponent<CubeSnap>().pointer2.position;
        }
        localDeltaEuler = transform.localEulerAngles - lastLocalEuler;
        lastLocalEuler = transform.localEulerAngles;
        if (getRayfromCamera)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }else
        {
            ray = new Ray(rayPointer.transform.position, rayPointer.transform.forward);
        }
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            linePositions[0] = rayPointer.position;
            if (snappingEnabled)
            {
                snapPoint = GetComponent<CubeSnap>().pointer2.position;
            }
            else
            {
                snapPoint = hit.point;
            }
            linePositions[1] = snapPoint;
            line.SetColors(hitColor, hitColor);

            if (shouldPlace)
            {
                if (Input.GetMouseButtonDown(0) || controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    GameObject obj = Instantiate(meshPrefab, snapPoint, meshPrefab.transform.rotation) as GameObject;



                    activeMesh = obj.GetComponent<ExtrudeMesh>();
                    activeMesh.StartDrawing(this.gameObject);
                    activeMesh.trackedObj = trackedObj;
                    shouldPlace = false;
                }


            }
        }else
        {
            linePositions[0] = rayPointer.position;
            linePositions[1] = rayPointer.position + ray.direction * 30;
            line.SetColors(emptyColor, emptyColor);
        }
            line.SetPositions(linePositions);

        if (activeMesh)
        {
            activeMesh.getRayfromCamera = getRayfromCamera;
            activeMesh.ray = ray;
            if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).magnitude > 0.0001f &&
                lastPad.magnitude > 0.0001f)
            {
                activeMesh.viveAxis = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) - lastPad;
            }
            if((controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) - lastPad).magnitude < 0.0001f)
            {
                activeMesh.viveAxis = new Vector2(0, 0);
            }
            lastPad = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            //(new Vector2(Mathf.Tan(-Mathf.Deg2Rad*localDeltaEuler.y), Mathf.Tan(-Mathf.Deg2Rad * localDeltaEuler.x)))*6f;
        }

    }




    public void SetRay(Ray r)
    {
        getRayfromCamera = false;
        ray = r;
    }

    void FinishedDrawing()
    {
        shouldPlace = true;
        this.enabled = false;
        GetComponent<TouchUIController>().SetEnable(true);
    }

    void OnDisable()
    {

        snappingEnabled = false;
        if (currentProjector)
        {
            currentProjector.SendMessage("TrunOffCanny");
        }

        if (canvasUI) canvasUI.SetActive(false);
        undoTarget = false;
        line.enabled = false;
        currentProjector.SendMessage("TurnOffCanny",SendMessageOptions.DontRequireReceiver);
    }

    void OnEnable()
    {
        SendMessage("SetUndoTargetFalse"); // tells every body else to turn off undo
        undoTarget = true;
        if (line) line.enabled = true;
        if (canvasUI) canvasUI.SetActive(true);
    }

    void Undo()
    {
        if (undoTarget)
            GameObject.Find("MeshCenter").SendMessage("DeleteLast");
    }

    void SetUndoTargetFalse()
    {
        if (!enabled)
            undoTarget = false;
    }

    void Disable()
    {
        
        GetComponent<TouchUIController>().SetEnable(true);
        this.enabled = false;
    }

}
