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
using System.Collections.Generic;


public class CubeMapCanny : MonoBehaviour
{
    public Cubemap cubemap;
    public TextureProjection projector;
    internal Cubemap cannyCubemap;
    public Material sky;
    public int s1 = 226;
    public int s2 = 100;
    public int mip = 3;
    public float f1 = 2;

    private int lasts1;
    private int lasts2;
    private float lastf1;
    // Use this for initialization
    void Start()
    {
        cubemap = projector.cubemap;
        Debug.Log(cubemap.mipmapCount);
        Debug.Log(cubemap.width / 2);
        cannyCubemap = new Cubemap(cubemap.width/Mathf.RoundToInt(Mathf.Pow(2,mip)), TextureFormat.RGBA32, true);
        //sky.SetTexture("_Tex", cannyCubemap);
        sky.SetTexture("_CUBE2", cannyCubemap);
    }


    void CannyCubemap(Cubemap cube, Cubemap cannycube)
    {
        Texture2D faceTex = new Texture2D(cannycube.height, cannycube.width);
        foreach (CubemapFace face in System.Enum.GetValues(typeof(CubemapFace)))
        {
            if (cube)
            {
                Color[] cols = cube.GetPixels(face, mip);
                faceTex.SetPixels(cols);
                cols = CannyTex2d(faceTex, s1, s2, f1).GetPixels();
                cannycube.SetPixels(cols, face);
            }

        }
        cannycube.Apply();
    }


    Texture2D CannyTex2d(Texture2D tex,float t1,float t2,float blur)
    {

        Texture2D output = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);

        // an edge detection algotrithm needs to be implemented

        output.Apply();
        return output;
    }



    void UpdateTex()
    {
        CannyCubemap(cubemap, cannyCubemap);
    }

    // Update is called once per frame
    void Update()
    {
        if ((lasts1 != s1) || lasts2 != s2 || lastf1 != f1)
        {
            Debug.Log("updated!");
            UpdateTex();
            lasts2 = s2;
            lasts1 = s1;
            lastf1 = f1;
        }
    }

}

