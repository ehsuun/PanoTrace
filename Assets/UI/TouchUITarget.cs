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

public class TouchUITarget : MonoBehaviour {
    public float AngleStart;
    public float AngleEnd;
    
    public Color baseColor;
    public Color hover;
    public Color click;
    public GameObject target;
    public string message;

    // Use this for initialization
    void Start () {
        baseColor = GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnHover()
    {
        GetComponent<Image>().color = hover; //new Color(baseColor.r - 0.1f, baseColor.g, baseColor.b - 0.1f, baseColor.a);
    }

    public void OnPush()
    {
        GetComponent<Image>().color = click; //new Color(baseColor.r - 0.3f, baseColor.g, baseColor.b - 0.3f, baseColor.a);
    }

    public void OnDown()
    {
        GetComponent<Image>().color = click; //new Color(baseColor.r , baseColor.g - 0.3f, baseColor.b - 0.3f, baseColor.a);
        if(target)target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
    }

    public void OnUp()
    {
        GetComponent<Image>().color = baseColor;
    }

    public void OnIdle()
    {
        GetComponent<Image>().color = baseColor;
    }




}
