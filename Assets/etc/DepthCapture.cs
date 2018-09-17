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
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DepthCapture : MonoBehaviour {

    public Shader depthShader;
    public int width = 2048;
    public int height = 2048;
    public Text fovText;
    private string imageName;
    private int index = 0;
    private Camera cam;

    public TextAsset binaryExample;
    public Texture2D testText;
    private string dpath;
    // Use this for initialization
    void Start () {
        dpath = Application.dataPath + "/../ScreenShots/";
        SceneManager.sceneLoaded += OnNewSceneLoaded;
        Object.DontDestroyOnLoad(transform.parent.gameObject);
        cam = GetComponent<Camera>();
        imageName = CreateName(4);
        fovText.text = "FOV : " + cam.fieldOfView.ToString("#.#");
        SceneManager.LoadScene("Render1");
    }
	
	// Update is called once per frame
	void Update () {



        if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
        {
            cam.fieldOfView += Input.mouseScrollDelta.y;
            fovText.text = "FOV : "+ cam.fieldOfView.ToString("#.#");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //ReadFile();
        }

        if (Input.GetMouseButton(0))
        {
            float h = 10 * Input.GetAxis("Mouse X");
            float v = 10 * Input.GetAxis("Mouse Y");
            transform.localEulerAngles += new Vector3(-v, h, 0);
        }
        

	}

    public void Capture()
    {

        if (!Directory.Exists(dpath))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(dpath);
        }

        RenderTexture tempRT = new RenderTexture(width, height, 24);
        // the 24 can be 0,16,24, formats like
        // RenderTextureFormat.Default, ARGB32 etc.

        cam.targetTexture = tempRT;
        cam.Render();

        RenderTexture.active = tempRT;
        Texture2D virtualPhoto =
            new Texture2D(width, height, TextureFormat.RGB24, false);
        // false, meaning no need for mipmaps
        virtualPhoto.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = null; //can help avoid errors 
        cam.targetTexture = null;
        // consider ... Destroy(tempRT);

        byte[] bytes;
        bytes = virtualPhoto.EncodeToPNG();

        System.IO.File.WriteAllBytes(
        dpath + imageName+index+".png", bytes);



        //Depth
        tempRT = new RenderTexture(width, height, 24,RenderTextureFormat.ARGBFloat);
        cam.targetTexture = tempRT;
        cam.RenderWithShader(depthShader,"");

        RenderTexture.active = tempRT;
        virtualPhoto =
            new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        // false, meaning no need for mipmaps
        virtualPhoto.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = null; //can help avoid errors 
        cam.targetTexture = null;
        // consider ... Destroy(tempRT);

        List<float> depth = new List<float>();
        for(int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                depth.Add(virtualPhoto.GetPixel(i, j).r);
            }
        }

        // create a byte array and copy the floats into it...
        var byteArray = new byte[depth.Count * 4];
        System.Buffer.BlockCopy(depth.ToArray(), 0, byteArray, 0, byteArray.Length);
        System.IO.File.WriteAllBytes(
        dpath + imageName + index + "depth.bytes", byteArray);

        index++;
    }

    public string CreateName(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        System.Text.StringBuilder res = new System.Text.StringBuilder();
        System.Random rnd = new System.Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    public void SetWidth(string w)
    {
        width = int.Parse(w);
    }

    public void SetHeigth(string h)
    {
        height = int.Parse(h);
    }

    void OnNewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.position = GameObject.Find("Proj").transform.position;
    }

    public void LoadScene(int i)
    {
        
        if (i==1)
        {
            SceneManager.LoadScene("Render1");
        }
        if (i == 2)
        {
            SceneManager.LoadScene("Render2");
        }
        if (i == 3)
        {
            SceneManager.LoadScene("Render3");
        }
        if (i == 4)
        {
            SceneManager.LoadScene("Render4");
        }
    }

    public void ReadFile()
    {
        Stream s = new MemoryStream(binaryExample.bytes);
        using (BinaryReader reader = new BinaryReader(s))
        {
            testText = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float val = reader.ReadSingle();
                    testText.SetPixel(i, j, new Color(val, val, val));
                }
            }
            testText.Apply();
        }
    }
}
