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

public class ShakeDetector : MonoBehaviour {

    private bool shake;

    public float damping = 0.7f;
    public float speedTreshould = 0.06f;
    private bool[] changes;

    private Vector3 dampedPosition;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Vector3 lastVelocity;

    public bool m1 = true;
    public GameObject target;
    public bool m2 = true;
    public GameObject target2;
    

    public string message;
    public string message2;
    // Use this for initialization
    void Start () {
        changes = new bool[4];
    }
	
	// Update is called once per frame
	void Update () {
        dampedPosition = Vector3.Lerp(dampedPosition, transform.localPosition, damping);
        Debug.DrawRay(dampedPosition, Vector3.up, Color.red);
        velocity = (dampedPosition - lastPosition)/Time.deltaTime;

        if (Vector3.Dot(velocity, lastVelocity) < 0 && velocity.magnitude> speedTreshould)
        {
            
            for(int i = 0; i < changes.Length; i++)
            {
                if (!changes[i])
                {
                    changes[i] = true;
                    StartCoroutine("MakeChangeTrue", i);
                    break; 
                }
            }
        }

        bool all = true;
        for (int i = 0; i < changes.Length; i++)
        {
            all = (all & changes[i]);
        }

        if (all)
        {
            if (target)
            {
                if(m1)
                target.SendMessage(message,SendMessageOptions.DontRequireReceiver);
            }
            if (target2)
            {
                if (m2)
                    target2.SendMessage(message2);
            }
            for (int i = 0; i < changes.Length; i++)
            {
                changes[i] = false;
            }
        }

        lastPosition = dampedPosition;
        lastVelocity = velocity;
    }


    IEnumerator MakeChangeTrue(int index)
    {
        
        yield return new WaitForSeconds(1);
        changes[index] = false;
    }
    
}
