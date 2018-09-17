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
using UnityEngine.UI;

public class EncircleSubMenu : MonoBehaviour {

    public GameObject UI;
    public Color baseColor;
    public Color hover;
    public Color click;
    public GameObject[] targets;
    public string[] messages;
    private Vector3 lastpos;

    internal float angle;
    internal float deltaAngle;
    internal float deltaX;
    internal float deltaY;
    internal float touchX;
    internal float touchY;
    internal EncircleMenu menu;

    // Use this for initialization
    void Start () {
        lastpos = transform.position;
        menu = transform.parent.GetComponent<EncircleMenu>();
    }
	
	// Update is called once per frame
	void Update () {
	    
        
	}

    public void OnHover()
    {
        GetComponent<Renderer>().material.color = hover;
    }

    public void OnUnHover()
    {
        GetComponent<Renderer>().material.color = baseColor;
    }

    public void OnPush()
    {
        GetComponent<Renderer>().material.color = click;
    }

    public void OnDown()
    {
        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].SendMessage(messages[i], SendMessageOptions.DontRequireReceiver);
        }
        
    }

    public void OnUp()
    {
        GetComponent<Renderer>().material.color = baseColor;
    }

    public void OnIdle()
    {
        GetComponent<Renderer>().material.color = baseColor;
    }



    public void zeroOut()
    {
        Debug.Log("Zero out");
      angle = -1;
      deltaAngle =0;
      deltaX = 0;
      deltaY = 0;
      touchX= -1;
      touchY = -1;
}
}
