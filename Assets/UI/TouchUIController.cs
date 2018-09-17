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

public class TouchUIController : MonoBehaviour {

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;

    public TouchUITarget[] targets;
    public GameObject canvas;
    public RectTransform pointer;

    private bool isenabled = true;


    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector2 pos = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        pointer.anchoredPosition = pos * 130;

        bool pressed = controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        bool down = controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        bool up = controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        float angle = Vector2.Angle(new Vector2(0, 1), pos);
        if (pos.x < 0) angle = 360-angle;
        if (pos.magnitude < 0.0000001) angle = -1;


        if (isenabled) {
            for(int i=0; i<targets.Length;i++)
            {
                if (360f*i/targets.Length<angle && angle<360f * (i+1) / targets.Length)
                {
                    if (pressed) { targets[i].OnPush(); }
                    else
                    {
                        targets[i].OnHover();
                    }
                    if (down)
                    {
                        targets[i].OnDown();
                    }
                }
                else
                {
                    targets[i].OnIdle();
                }
            }
        }
    }

   public void SetEnable(bool state)
    {
        isenabled = state;
        canvas.SetActive(isenabled);
    }

    public void MeshCreatorEnable()
    {
        GetComponent<MeshCreator>().enabled = true;
        SetEnable(false);
    }

    public void FlyAroundEnable()
    {
        GetComponent<FlyAround>().enabled = true;
        SetEnable(false);
    }

    public void Undo()
    {
        
    }

    public void PlaneMakerEnable()
    {
        GetComponent<PlaneMaker>().enabled = true;
        SetEnable(false);
    }

    public void DrawDepthEnable()
    {
        GetComponent<DrawDepth>().enabled = true;
        SetEnable(false);
    }
}
