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
using UnityEngine.SceneManagement;
using System.Collections;

public class Teleoprter : MonoBehaviour {
    public string sceneName;
    private bool grabbed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!grabbed)
        {
            transform.position += new Vector3(0,0.0005f*Mathf.Sin(Time.time*3f),0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName);
        }
	}

    void OnTriggerStay(Collider col)
    {
        GrabObject g = col.GetComponent<GrabObject>();
        if(g)
        {
            Debug.Log(g.name + " is inside");
            if (g.grab&&(!GetComponent<FixedJoint>()))
            {
                Debug.Log("fixed joint added");
                grabbed = true;
                FixedJoint Joint =  gameObject.AddComponent<FixedJoint>();
                Joint.connectedBody = g.GetComponent<Rigidbody>();
                Joint.breakForce = 1000;

            }
            if(!g.grab&& GetComponent<FixedJoint>())
            {
                grabbed = false;
                Debug.Log("fixed joint destroyd");
                Destroy(GetComponent<FixedJoint>());
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.name.Contains("eye"))
        {
            Invoke("LoadScene", 0.2f);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
