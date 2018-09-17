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
using UnityEditor;

[ExecuteInEditMode]
public class TextureBaker : MonoBehaviour {
    public Material sample;
    public TextureProjection projector;
    public Texture2D bakedTex;
    public Shader BakingShader;
    public Shader baked;
    public Shader projection;
    private Matrix4x4 rotationMatrix;
    private RenderTexture rt;
    public string[] propertyNames;
    public int res = 2048;

    private Camera virtuCamera;
    // Use this for initialization
    void Start () {
        if (!projector)
        {
            projector = GameObject.Find("Proj").GetComponent<TextureProjection>();
        }

        if (!BakingShader)
        {
            BakingShader = Shader.Find("Ehsan/TextureBaker");
        }
        if (!projection)
        {
            projection = Shader.Find("Unlit/Matterport");
        }

        if (!baked)
        {
            baked = Shader.Find("Unlit/Texture");
        }
        propertyNames = new string[4];
        bakedTex = new Texture2D(res, res);

        propertyNames[0] = "_ProjectorX";
        propertyNames[1] = "_ProjectorY";
        propertyNames[2] = "_ProjectorZ";
        propertyNames[3] = "_ProjectorYAngle";

    }
	
	// Update is called once per frame
	void Update () {

        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[0], projector.transform.position.x);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[1], projector.transform.position.y);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[2], projector.transform.position.z);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[3], -Mathf.Deg2Rad * projector.transform.eulerAngles.y);
        GetComponent<Renderer>().sharedMaterial.SetMatrix("_TextureRotation", projector.rotationMatrix);

        if (Input.GetKeyDown(KeyCode.B))
        {
            BakeTexture();
            
        }
    }

    public void BakeTexture()
    {
        //Unwrapping.GeneratePerTriangleUV(GetComponent<MeshFilter>().sharedMesh);
        GetComponent<Renderer>().material = sample;
        virtuCamera = gameObject.AddComponent<Camera>();
        virtuCamera.enabled = true;
        //previousShader = GetComponent<Renderer>().sharedMaterial.shader;
        GetComponent<Renderer>().material.shader = BakingShader;
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[0], projector.transform.position.x);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[1], projector.transform.position.y);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[2], projector.transform.position.z);
        GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[3], -Mathf.Deg2Rad * transform.eulerAngles.y);

        GetComponent<Renderer>().sharedMaterial.SetTexture("_CUBE", projector.cubemap);
        GetComponent<Renderer>().sharedMaterial.SetMatrix("_TextureRotation", projector.rotationMatrix);

        virtuCamera.aspect = 1.0f;
        // recall that the height is now the "actual" size from now on

        RenderTexture tempRT = new RenderTexture(res, res, 24);
        // the 24 can be 0,16,24, formats like
        // RenderTextureFormat.Default, ARGB32 etc.

        virtuCamera.targetTexture = tempRT;
        virtuCamera.Render();

        RenderTexture.active = tempRT;
        bakedTex =
            new Texture2D(res, res, TextureFormat.RGB24, false);
        // false, meaning no need for mipmaps
        bakedTex.ReadPixels(new Rect(0, 0, res, res), 0, 0);

        RenderTexture.active = null; //can help avoid errors 
        virtuCamera.targetTexture = null;
        // consider ... Destroy(tempRT);
        Debug.Log("Texture Baked");
        virtuCamera.enabled = false;
        GetComponent<Renderer>().material.shader = baked;
        bakedTex.Apply();
        GetComponent<Renderer>().material.SetTexture("_MainTex", bakedTex);
        DestroyImmediate(GetComponent<Camera>());
    }

    [MenuItem("Panorama Baker/Bake Selected")]
    static void DoSomething()
    {
        var obj = Selection.activeGameObject;
        if(obj.GetComponent<TextureBaker>()){
            obj.GetComponent<TextureBaker>().BakeTexture();
        }
        else
        {
            Debug.LogWarning("This Object does not have Baking ability. try adding TextureBaker component.");
        }
    }
}
