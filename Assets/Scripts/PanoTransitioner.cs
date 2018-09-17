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

[ExecuteInEditMode]
public class PanoTransitioner : MonoBehaviour {
    public Transform[] places;
    public float distance = 3;
    public Transform place1;
    public Transform place2;

    public GameObject targetMaterial;

    private Transform currentTransform;
    private Transform nearest;

    public bool LockedMode = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (places.Length >1)
        {
            float dist = 0f;
            nearest = places[0];

            foreach (Transform t in places)
            {
                dist = (transform.position - nearest.position).magnitude;
                if ((transform.position - t.position).magnitude < dist)
                {
                    nearest = t;
                }

            }
            if (nearest != currentTransform)
            {
                targetMaterial.GetComponent<Renderer>().sharedMaterial.SetTexture("_CUBE",
                nearest.GetComponent<TextureProjection>().cubemap);

                if (currentTransform)
                    currentTransform.SendMessage("TurnOff");
                nearest.SendMessage("TurnOn");
                currentTransform = nearest;
            }
            if (dist > distance)
            {
                if (LockedMode)
                    transform.position = new Vector3(currentTransform.position.x, transform.position.y, currentTransform.position.z);
            }

        }
        //float blend = (transform.position - place1.position).magnitude / (place2.position - place1.position).magnitude;
        //targetMaterial.GetComponent<Renderer>().sharedMaterial.SetFloat("_Blend", Mathf.RoundToInt(blend));
    }
}
