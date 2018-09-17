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

public class FollowSmooth : MonoBehaviour {

    public Transform target1;
    public Transform target2;

    private Transform target;
    private Transform smoothTarget;

    public float speed;
    public bool followPos;

    private Vector3 initPos;
    // Use this for initialization
	void Start () {
        smoothTarget = new GameObject("smooth").transform;
        initPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (target1.parent.gameObject.activeSelf)
        {
            target = target1;
        }
        else if(target2.parent.gameObject.activeSelf)
        {
            target = target2;
        }

        smoothTarget.position = Vector3.Lerp(smoothTarget.position, target.position, speed);
        smoothTarget.rotation = Quaternion.Lerp(smoothTarget.rotation, target.rotation, speed);
        // if(followPos)
        //transform.position = smoothTarget.position + ((smoothTarget.forward * -6) + (smoothTarget.up * 2)+ (smoothTarget.right * 6)) * target.parent.localScale.x;

        //transform.position = initPos * target.parent.localScale.x;
        transform.LookAt(smoothTarget, Vector3.up);
	}   
}
