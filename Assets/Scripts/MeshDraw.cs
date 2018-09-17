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



public class MeshDraw : MonoBehaviour {
    public enum DrawingType { InsertMesh, Pipe };
    public DrawingType drawingType = DrawingType.InsertMesh;
    public float pressureSensitivity = 0;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;
    public GameObject brush;
    public Transform stroke;
    public Transform container;
    public bool draw;
    private bool lastDraw = false;
    private GameObject lastObj;
    private Vector3 lastPos;
    private float radius = 0.2f;
    private int counter;
    private Mesh brushMesh;
    public int total = 0;
    private GameObject brushUI;
    public GameObject brushVisual;
    public GameObject canvasUI;
    public GameObject wheel;

    private List<Transform> strokes;
    public float brushSize = 0.02f;
    public float fill = 1;
    private Vector2 lastTouch;
    private Vector2 touch;
    private float deltaX;
    private float stepper;
    private float step = 0.1f;
    public bool undoTarget = false;
    public CameraRigStabilizer stab;
    private Mesh ring;

    private List<Vector3> currentVerts;
    private List<Vector3> currentNorms;
    private List<int> currentTris;


    // Use this for initialization
    void Start () {
        brushUI = GameObject.Instantiate(brush, transform.position, transform.rotation, transform) as GameObject;
        brushUI.transform.localPosition = Vector3.forward * 0.1f + Vector3.up * 0.05f;
        if(brushVisual) brushVisual.transform.localPosition = brushUI.transform.localPosition;
        strokes = new List<Transform>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (!stroke)
        {
            NewStroke();
        }
        brushMesh = brushUI.GetComponent<MeshFilter>().sharedMesh;

        currentVerts = new List<Vector3>();
        currentNorms = new List<Vector3>();
        currentTris = new List<int>();
        ring = CreateRing(4);
        if(drawingType == DrawingType.Pipe)
        {
            brushUI.GetComponent<MeshFilter>().sharedMesh = ring;
        }else
        {
            brushVisual.SetActive(false);
        }
        
    }

    public void DrawEnable()
    {
        this.enabled = true;
    }

    public void DrawEnableMesh()
    {

        drawingType = DrawingType.InsertMesh;
        this.enabled = true;
        brushUI.GetComponent<MeshFilter>().sharedMesh = brush.GetComponent<MeshFilter>().sharedMesh;
        brushVisual.SetActive(false);
    }

    public void DrawEnablePipe()
    {
        drawingType = DrawingType.Pipe;
        this.enabled = true;
        brushUI.GetComponent<MeshFilter>().sharedMesh = ring;
        brushVisual.SetActive(true);
    }

    void OnEnable()
    {
        if (canvasUI) canvasUI.SetActive(true);
        SendMessage("SetUndoTargetFalse"); // tells every body else to turn off undo
        undoTarget = true;
        GetComponent<TouchUIController>().SetEnable(false);
        wheel.transform.parent.parent.gameObject.SetActive(true);
        if(brushUI)brushUI.SetActive(true);
        if (brushVisual) brushVisual.SetActive(true);
    }

    void SetUndoTargetFalse()
    {
        if (!enabled)
        undoTarget = false;
    }

    void OnDisable()
    {
        if(canvasUI)canvasUI.SetActive(false);
        undoTarget = false;
        brushUI.SetActive(false);
        if (brushVisual) brushVisual.SetActive(false);
        wheel.transform.parent.parent.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (!stroke)
        {
            Debug.Log("Fixed during update");
            stroke = strokes[strokes.Count - 1];
        }

        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            stab.enabled = true;
        }


        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            stab.enabled = false;
        }


        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            Disable();
        }

        draw = controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);



        touch = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

        if((lastTouch - touch).magnitude < 0.2)
        {
            deltaX = touch.x - lastTouch.x;

        }else
        {
            deltaX = 0;
        }
        brushSize += deltaX*0.1f;
        brushSize = Mathf.Clamp(brushSize, 0.001f, 1);

        radius = brushSize / fill;
        brushUI.transform.localScale = Vector3.one * brushSize;
        if (brushVisual)
        {
            brushVisual.transform.localScale = new Vector3(1,0.05f,1) * 2 * brushSize;
        }

        for (int i = 0; i < ring.vertexCount -1; i++)
        {
            Debug.DrawLine(brushUI.transform.TransformPoint(ring.vertices[i]), brushUI.transform.TransformPoint(ring.vertices[i + 1]));
        }


            if (draw)
        {
            if (drawingType == DrawingType.InsertMesh)
            {
                if ((transform.position - lastPos).magnitude > radius)
                {
                    lastObj = GameObject.Instantiate(brushUI, brushUI.transform.position, brushUI.transform.rotation, stroke) as GameObject;
                    lastObj.layer = 8;
                    lastPos = transform.position;
                    counter++;
                    total++;
                }
            }else if(drawingType == DrawingType.Pipe)
            {
                if ((transform.position - lastPos).magnitude > radius)
                {
                    float amp = -Mathf.Pow(controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x,3f)* pressureSensitivity;
                    

                    if (currentVerts.Count == 0)
                    {
                        currentVerts.Add(brushUI.transform.TransformPoint(Vector3.zero));
                        currentNorms.Add(brushUI.transform.TransformDirection(Vector3.one*1));
                    }


                    int count = currentVerts.Count - ring.vertexCount;

                    for (int i =0; i < ring.vertexCount; i++)
                    {
                        currentVerts.Add(brushUI.transform.TransformPoint(ring.vertices[i]* (1-amp)));
                        currentNorms.Add(brushUI.transform.TransformDirection(ring.normals[i]));
                    }

                    if(currentVerts.Count> ring.vertexCount+1) { 
                        for (int i = count; i < ring.vertexCount + count -1 ; i++)
                        {
                            List<int> twotris = new List<int> { i+1, i+ ring.vertexCount, i,
                                                                i+1,i+ ring.vertexCount+1,i+ ring.vertexCount };
                            currentTris.AddRange(twotris);
                        }
                    }else // make a cap
                    {
                        for (int i = 0; i < ring.vertexCount; i++)
                        {
                            List<int> tris = new List<int> { 0, i + 1, i  };
                            currentTris.AddRange(tris);
                        }
                    }



                    stroke.GetComponent<MeshFilter>().sharedMesh.vertices = currentVerts.ToArray();
                    stroke.GetComponent<MeshFilter>().sharedMesh.triangles = currentTris.ToArray();
                    stroke.GetComponent<MeshFilter>().sharedMesh.normals = currentNorms.ToArray();

                    lastPos = transform.position;
                }
            }
        }else if (lastDraw)
        {
            if(drawingType == DrawingType.Pipe)
            {
                int count = currentVerts.Count - ring.vertexCount;

                currentVerts.Add(brushUI.transform.TransformPoint(Vector3.zero));
                currentNorms.Add(brushUI.transform.TransformDirection(Vector3.one * -1));

                for (int i = count; i < count + ring.vertexCount; i++)
                {
                    List<int> tris = new List<int> { currentVerts.Count-1, i, i+1 };
                    currentTris.AddRange(tris);
                }

                stroke.GetComponent<MeshFilter>().sharedMesh.vertices = currentVerts.ToArray();
                stroke.GetComponent<MeshFilter>().sharedMesh.triangles = currentTris.ToArray();
                stroke.GetComponent<MeshFilter>().sharedMesh.normals = currentNorms.ToArray();
            }
        }

        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            if (drawingType == DrawingType.InsertMesh)
            {
                CollapseStroke();
            }
            else
            {
                currentVerts.Clear();
                currentNorms.Clear();
                currentTris.Clear();
                stroke.gameObject.AddComponent<MeshCollider>();
                stroke.tag = "transformable";
            }
            NewStroke();
        }


        if (counter > 50)
        {
            CollapseStroke();
        }


        if (total > 500)
        {
            NewStroke();
            total = 0;
        }

        lastTouch = touch;
        lastDraw = draw;

    }


    public void Undo()
    {

        if (undoTarget)
        {
            if (strokes.Count == 1)
            {
                Destroy(strokes[strokes.Count - 1].gameObject);
                strokes.RemoveAt(strokes.Count - 1);
                NewStroke();

                stroke = strokes[strokes.Count - 1];
            }
            else if (strokes.Count > 1)
            {
                Destroy(strokes[strokes.Count - 1].gameObject);
                strokes.RemoveAt(strokes.Count - 1);

                stroke = strokes[strokes.Count - 1];

            }
        }
    }

    Mesh CollapseMesh(Transform trans , bool deletechildren)
    {
        if (trans.childCount < 1)
        {
            return trans.GetComponent<MeshFilter>().mesh;
        }

        Mesh total = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<int> tris = new List<int>();

        foreach (Transform ch in stroke)
        {
            Mesh brushMesh = ch.GetComponent<MeshFilter>().mesh;
            List<Vector3> objWorldVecs = new List<Vector3>(brushMesh.vertices);
            List<Vector3> objWorldNorms = new List<Vector3>(brushMesh.normals);
            List<int> tempTris = new List<int>(brushMesh.triangles);
            for (int i= 0;i<objWorldVecs.Count;i++)
            {
                objWorldVecs[i] = ch.TransformPoint(objWorldVecs[i]);
                objWorldNorms[i] = ch.TransformDirection(objWorldNorms[i]);
            }

            for (int i = 0; i < tempTris.Count; i++)
            {
                tempTris[i] += verts.Count;
            }

            verts.InsertRange(verts.Count, objWorldVecs);
            norms.InsertRange(norms.Count, objWorldNorms);
            tris.InsertRange(tris.Count, tempTris);
            if (deletechildren)
            {
                Destroy(ch.gameObject);
            }
        }
        total.vertices = verts.ToArray();
        total.normals = norms.ToArray();
        total.triangles = tris.ToArray();

        return total;
    }

    private void NewStroke()
    {
        stroke = new GameObject().transform;
        stroke.name = "Stroke";
        stroke.position = Vector3.zero;
        stroke.rotation = Quaternion.identity;
        stroke.gameObject.AddComponent<MeshFilter>().sharedMesh = new Mesh();

        stroke.gameObject.AddComponent<MeshRenderer>().sharedMaterial = brushUI.GetComponent<Renderer>().sharedMaterial;
        strokes.Add(stroke);

    }

    void Disable()
    {
        GetComponent<TouchUIController>().SetEnable(true);
        this.enabled = false;
        
    }

    void CollapseStroke()
    {
        lastObj = new GameObject();
        lastObj.name = "Collapsed";
        lastObj.tag = "transformable";
        lastObj.layer = 8;
        lastObj.transform.position = Vector3.zero;
        lastObj.transform.rotation = Quaternion.identity;
        lastObj.AddComponent<MeshFilter>();
        lastObj.AddComponent<MeshRenderer>().sharedMaterial = brush.GetComponent<MeshRenderer>().sharedMaterial;
        lastObj.GetComponent<MeshFilter>().mesh = CollapseMesh(stroke, true);
        lastObj.transform.SetParent(stroke, true);
        lastObj.AddComponent<MeshCollider>(); 
        counter = 0;
    }

    Mesh CreateRing(int sides)
    {
        Mesh output = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();

        for (int i=0;i< sides+1; i++)
        {
            float phase = i * (2*Mathf.PI / (float)sides);
            verts.Add(new Vector3(Mathf.Cos(phase), 0, Mathf.Sin(phase)));
            norms.Add(verts[i].normalized);
        }

        output.vertices = verts.ToArray();
        output.normals = norms.ToArray();
        return output;
    }

}
