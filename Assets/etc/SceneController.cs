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

public class SceneController : MonoBehaviour {
    public GameObject sceneMesh;
    public GameObject drawMesh;
    public MeshCenter meshCenter;
    public Material wireFrame;
    private Material pre;
    public bool wMode;


    // Use this for initialization
    void Start () {
      if(!sceneMesh) sceneMesh = GameObject.Find("SceneMesh");

        if (!GameObject.Find("DrawMesh"))
        {
            drawMesh = new GameObject("DrawMesh");
        }else
        {
            drawMesh = GameObject.Find("DrawMesh");
        }


         // setting up mesh center
        GameObject obj = new GameObject("MeshCenter");
        meshCenter = obj.AddComponent<MeshCenter>();
        meshCenter.editingMeshes = new MeshFilter[50];

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            sceneMesh.SetActive(!sceneMesh.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ToggleWireframe();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(5);
        }

    }

    void OnPreRender()
    {
        GL.wireframe = true;
    }
    void OnPostRender()
    {
        GL.wireframe = false;
    }


    public void ToggleWireframe()
    {
        if (wMode)
        {
            foreach (Transform t in sceneMesh.transform)
            {
                if (t.GetComponent<Wireframe>())
                {
                    t.gameObject.GetComponent<Wireframe>().enabled = false;
                    Destroy(t.GetComponent<Wireframe>());

                }
            }
            foreach (Transform t in drawMesh.transform)
            {
                if (t.GetComponent<Wireframe>())
                {
                    t.gameObject.GetComponent<Wireframe>().enabled = false;
                    Destroy(t.GetComponent<Wireframe>());
                }
            }
        }
        else
        {

            foreach (Transform t in sceneMesh.transform)
            {
                if (t.GetComponent<MeshRenderer>())
                {
                    Wireframe w;
                    w = t.GetComponent<Wireframe>();
                    if (w == null)
                    {
                        Destroy(t.GetComponent<LineRenderer>());
                        w = t.gameObject.AddComponent<Wireframe>();

                    }
                    else
                    {
                        w.enabled = true;
                        LineRenderer l = t.GetComponent<LineRenderer>();
                        if (l != null)
                        {
                            Destroy(l);
                        }
                    }

                    w.lineMaterial = wireFrame;
                    w.render_lines_1st = true;

                    w.lineWidth = 20;
                }

            }

            foreach (Transform t in drawMesh.transform)
            {
                if (t.GetComponent<MeshRenderer>())
                {
                    Wireframe w;
                    w = t.GetComponent<Wireframe>();
                    if (w == null)
                    {
                        Destroy(t.GetComponent<LineRenderer>());
                        w = t.gameObject.AddComponent<Wireframe>();
                    }
                    else
                    {
                        w.enabled = true;
                        LineRenderer l = t.GetComponent<LineRenderer>();
                        if (l != null)
                        {
                            Destroy(l);
                        }
                    }

                    w.lineMaterial = wireFrame;
                    w.render_lines_1st = true;

                    w.lineWidth = 20;
                }

            }

        }

        wMode = !wMode;
    }

}
