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

public class ImageSnap : MonoBehaviour {

    public Transform pointer;
    public Transform pointer2;
    public Vector2 pointOnTexture;
    public int pixelY;
    public int pixelX;

    private int pixelYmod;
    private int pixelXmod;
    public float currentColor;
    public float lastColor;


    public Texture2D tex;
    public _Pix[] neighbours;
    private Vector3 lastpos;
	// Use this for initialization
    public struct _Pix
    {
        public Color color;
        public int x;
        public int y;
    }

	void Start () {
        neighbours = new _Pix[8];
        currentColor = 100;
    }
	
	// Update is called once per frame
	void Update () {
        if (lastpos != Input.mousePosition)
        {
            lastpos = Input.mousePosition;
            pixelXmod = pixelX;
            pixelYmod = pixelY;
            currentColor = 100;
            pointer2.position = pointer.position;
        }

        if (tex == null)
        {
            tex = GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
        }

        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            pointer.position = hit.textureCoord;
            pointOnTexture = hit.textureCoord;
            pixelX = Mathf.CeilToInt(tex.width * pointOnTexture.x);
            pixelY = Mathf.CeilToInt(tex.width * pointOnTexture.y);
            pointer2.position = SnaptoTexture(pixelX, pixelY,tex);
        }


	}

    Vector2 SnaptoTexture(int x,int y, Texture2D tex)
    {
        int lowesti = pixelXmod;
        int lowestj = pixelYmod;

        while (currentColor > lastColor)
        {
            for (int i = -5; i < 6; i++)
            {
                for (int j = -5; j < 6; j++)
                {
                    if (tex.GetPixel(pixelXmod + i, pixelYmod + j).grayscale > tex.GetPixel(lowesti, lowestj).grayscale)
                    {

                        lowesti = pixelXmod + i;
                        lowestj = pixelYmod + j;

                    }
                }
            }

            currentColor = tex.GetPixel(lowesti, lowestj).grayscale;
            lastColor = tex.GetPixel(pixelXmod, pixelYmod).grayscale;

            //if (currentColor > lastColor) Debug.Log(currentColor + " morethan " + lastColor);
            pixelYmod = lowestj;
            pixelXmod = lowesti;     
        }
        return new Vector2((float)pixelXmod / (float)tex.width, (float)pixelYmod / (float)tex.height);
    }
}
