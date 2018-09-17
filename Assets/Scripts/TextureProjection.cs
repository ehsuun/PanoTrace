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
using UnityEngine.SceneManagement;
using System.IO;

[ExecuteInEditMode]
public class TextureProjection : MonoBehaviour {

    public GameObject target;
    public string[] propertyNames = new string[4];
    public GameObject meshes;
    public Shader depthShader;
    public Cubemap cubemap;
    public RenderTexture depthCubemap;
    private RenderTexture depthFace;
    private Texture2D tempDepth;

    public bool hasQuarternion;
    public Quaternion definedRotation;

    private Camera cam;
    internal Matrix4x4 rotationMatrix;
    public Material skybox;
    public bool isOn;
    private bool lastOn;

	// Use this for initialization
	void Start () {
        cam = gameObject.GetComponent<Camera>();
        if(cam==null){
            cam = gameObject.AddComponent<Camera>();
		}
        cam.enabled = false;
        int layerMask = 1 << 8;
        //cam.cullingMask = layerMask;

        if (depthShader == null)
        {
            depthShader = Shader.Find("Projector/DepthShader");
        }
        if(target)target.GetComponent<Renderer>().sharedMaterial.SetTexture("_CUBE", cubemap);
        depthCubemap = new RenderTexture(2048 , 2048, 0, RenderTextureFormat.ARGBFloat);
        depthFace = new RenderTexture(2048, 2048, 24, RenderTextureFormat.ARGB32);
        tempDepth = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
        RenderDepthCubemap();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Selection");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RenderDepthFace();
        }

        if (hasQuarternion)
        {
            transform.localRotation = definedRotation;
            transform.rotation = Quaternion.LookRotation(transform.right*-1, transform.forward);
            rotationMatrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
        }else
        {
            rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(transform.eulerAngles * -1), Vector3.one);
        }
        
        if (isOn)
        {
            if (!lastOn)
            {
                target.GetComponent<Renderer>().sharedMaterial.SetTexture("_CUBE", cubemap);
                if (skybox)
                {
                    skybox.SetTexture("_Tex", cubemap);
                }
                
            }
            

            RenderDepthCubemap();
            if (propertyNames[0] == "")
            {
                propertyNames[0] = "_ProjectorX";
                propertyNames[1] = "_ProjectorY";
                propertyNames[2] = "_ProjectorZ";
                propertyNames[4] = "_ProjectorYAngle";
            }
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[0], transform.position.x);
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[1], transform.position.y);
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[2], transform.position.z);
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[3], -Mathf.Deg2Rad * transform.eulerAngles.y);

            ///////////////aditional and experimental
            if (propertyNames.Length == 7) { 
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[4], transform.position.x);
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[5], transform.position.y);
            target.GetComponent<Renderer>().sharedMaterial.SetFloat(propertyNames[6], transform.position.z);
            }

            target.GetComponent<Renderer>().sharedMaterial.SetMatrix("_TextureRotation", rotationMatrix);
        }

        lastOn = isOn;
    }

    public void TurnOn()
    {
        isOn = true;
        if(meshes)meshes.SetActive(true);
    }

    public void TurnOff()
    {
        isOn = false;
        if (meshes) meshes.SetActive(false);
    }

    public void RenderDepthCubemap()
    {
        depthCubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
        //cam.targetTexture = depthCubemap;
        cam.SetReplacementShader(depthShader, "");
        cam.RenderToCubemap(depthCubemap);
        
        target.GetComponent<Renderer>().sharedMaterial.SetTexture("_CUBEDEPTH", depthCubemap);
    }
    public void TrunOnCanny()
    {
        target.GetComponent<Renderer>().sharedMaterial.SetFloat("_Blend",1);
    }
    public void TrunOffCanny()
    {
        target.GetComponent<Renderer>().sharedMaterial.SetFloat("_Blend", 0);
    }

    public void RenderDepthFace()
    {
        cam.targetTexture = depthFace;
        cam.SetReplacementShader(depthShader, "");
        cam.Render();

        RenderTexture.active = depthFace;
        tempDepth.ReadPixels(new UnityEngine.Rect(0, 0, 2048, 2048), 0, 0);
        RenderTexture.active = null;
        cam.targetTexture = null;
        File.WriteAllBytes(Application.dataPath + "/depth.bin", tempDepth.GetRawTextureData());
        /*
        Mat imgMat = new Mat(tempDepth.height, tempDepth.width, CvType.CV_32FC1);
        Utils.texture2DToMat(tempDepth, imgMat);

        Imgproc.equalizeHist(imgMat, imgMat);
        imgMat.convertTo(imgMat, CvType.CV_8UC1, 255.0);
        Highgui.imwrite(Application.dataPath + "/depth.png", imgMat);
        */
    }

}
