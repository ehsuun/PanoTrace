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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSwitcher : MonoBehaviour {
    public enum PanoModel { TwoD,PanoTrace,GroundTruth};
    public GameObject panoTrace;
    public GameObject groundTruth;
    public int mode = 0;
    public PanoModel modelMode;

    public GameObject cameraparent;
    public GameObject eye;
    public GameObject proj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mode++;
            if (mode > 2)
            {
                mode = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mode--;
            if (mode < 0)
            {
                mode = 2;
            }            

        }

        if(Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (mode == 0)
            {
                panoTrace.SetActive(false);
                groundTruth.SetActive(false);
                modelMode = PanoModel.TwoD;

            }
            if (mode == 1)
            {
                panoTrace.SetActive(true);
                groundTruth.SetActive(false);
                modelMode = PanoModel.PanoTrace;

            }
            if (mode == 2)
            {
                panoTrace.SetActive(false);
                groundTruth.SetActive(true);
                modelMode = PanoModel.GroundTruth;

            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Recalibrate();
        }



    }

    void Recalibrate()
    {
        cameraparent.transform.Translate(proj.transform.position - eye.transform.position);
    }
}
