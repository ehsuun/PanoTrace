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

public class ObjectMaker : MonoBehaviour {
    public GameObject redCube;
    public GameObject greenCube;
    public GameObject blueCube;

    public GameObject redSphere;
    public GameObject greenSphere;
    public GameObject blueSphere;

    public GameObject cylinder;
    public GameObject quad;
    public GameObject capsule;

    public GameObject multipleRedCubes;
    public GameObject multipleGreenCubes;
    public GameObject multipleBlueCubes;

    private GameObject lastObject;
    private Vector3 instPos;
    private Transform cam;
    public float delta = 0;
    Color CurrentCol = Color.white;
    private VRTransformer transformer;
    // Use this for initialization
    void Start () {
        cam = Camera.main.transform;
        transformer = GameObject.Find("VRTransformer").GetComponent<VRTransformer>();
    }
	
	// Update is called once per frame
	void Update () {
        instPos = cam.forward * 3 + cam.position;
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit))
        {
            instPos = hit.point - cam.forward * (hit.point- cam.position).magnitude*0.3f;
        }
        if (transformer.target)
        {
            if (delta > 0.0001||delta <-0.0001)
            {
                //lastObject.transform.Rotate(new Vector3(0, delta, 0));
                CurrentCol = transformer.target.GetComponent<Renderer>().material.color;
                float h;float s; float v;
                Color.RGBToHSV(CurrentCol, out h, out s, out v);
                transformer.target.GetComponent<Renderer>().material.color = Color.HSVToRGB(Mathf.Repeat(h+delta*0.003f,1f),s,v);
            }
            
        }

        instPos = cam.position + cam.forward * 0.5f;

    }


    public void MakeRedSphere()
    {
        lastObject = GameObject.Instantiate(redSphere, instPos, transform.rotation) as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeGreenSphere()
    {
        lastObject = GameObject.Instantiate(greenSphere, instPos, transform.rotation) as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeBlueSphere()
    {
        lastObject = GameObject.Instantiate(blueSphere, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeRedCube()
    {
        lastObject = GameObject.Instantiate(redCube, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeGreenCube()
    {
        lastObject = GameObject.Instantiate(greenCube, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeBlueCube()
    {
        lastObject = GameObject.Instantiate(blueCube, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeMultipleRedCubes()
    {
        lastObject = GameObject.Instantiate(multipleRedCubes, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }
    public void MakeMultipleGreenCubes()
    {
        lastObject = GameObject.Instantiate(multipleGreenCubes, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }
    public void MakeMultipleBlueCubes()
    {
        lastObject = GameObject.Instantiate(multipleBlueCubes, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeQuad()
    {
        lastObject = GameObject.Instantiate(quad, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }
    public void MakeCylinder()
    {
        lastObject = GameObject.Instantiate(cylinder, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }

    public void MakeCapsule()
    {
        lastObject = GameObject.Instantiate(capsule, instPos, transform.rotation)as GameObject;
        lastObject.name = lastObject.name + Time.time;
    }




}
