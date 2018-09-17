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

public class VRTransformController : MonoBehaviour {
    public VRTransformer transformer;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    public int pointer = 1;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)|| controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            if (pointer == 1)
            {
                transformer.point1Locked = true;

            }
            if (pointer == 2)
            {
                transformer.point2Locked = true;
            }

            if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
            {
                transformer.copyTarget = true;
                if (pointer == 2)
                {
                    transformer.copyBaked = true;
                }

            }

        }

        if (pointer == 1)
        {
            transformer.point1 = transform.position;

        }
        if (pointer == 2)
        {
            transformer.point2 = transform.position;
        }

        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) || controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            if (pointer == 1)
            {
                transformer.point1Locked = false;
            }
            if (pointer == 2)
            {
                transformer.point2Locked = false;
            }
        }
    }
}
