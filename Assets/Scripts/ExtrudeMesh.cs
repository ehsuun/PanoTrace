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
using System.Collections.Generic;

//[RequireComponent(typeof(LineRenderer))]
public class ExtrudeMesh : MonoBehaviour
{
    public Vector2[] vertices2D;
    public Vector3[] vertices3D;
    public Material mat;
    public float lineWidth = 0.1f;

    public bool doesDraw = true;
    public bool doesExtrude = false;
    private bool firstFrame = true;
    public float extrudeLength;

    private Mesh tempMesh;

    private Vector3[] newVerts;
    private Vector3 ExtrNormal;
    private int[] newTris;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;

    public bool getRayfromCamera;
    public Ray ray;

    public GameObject creator;
    public Vector2 viveAxis;

    private LineRenderer line;
    private SceneController sceneController;


    public void StartDrawing(GameObject cr)
    {
        doesDraw = true;
        creator = cr;
    }

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        float w = (Camera.main.transform.position - transform.position).magnitude;
        line.SetWidth(w * lineWidth, w * lineWidth);
        Vector3[] pos = { transform.position, transform.position };
        line.SetPositions(pos);
    }

    void Start()
    {
        gameObject.layer = 8;

        if (!GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent(typeof(MeshRenderer));
            gameObject.AddComponent(typeof(MeshFilter));
        }
            GetComponent<MeshRenderer>().material = mat;


        
        sceneController = creator.transform.parent.GetComponent<SceneController>();
        /*
        Wireframe w;
        w = gameObject.AddComponent<Wireframe>();
        w.enabled = sceneController.wMode;
        w.lineMaterial = sceneController.wireFrame;
                        w.render_lines_1st = true;
                        w.lineWidth = 10;
                        */
    }

    void Update()
    {

        float w = (Camera.main.transform.position - transform.position).magnitude;
        if(line)line.SetWidth(w * lineWidth, w * lineWidth);

        if (getRayfromCamera)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        
        if (doesDraw)
        {
            if (Input.GetMouseButtonDown(0) || controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)|| firstFrame)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector2[] tempArray2d = new Vector2[vertices2D.Length + 1];
                    Vector3[] tempArray3d = new Vector3[vertices3D.Length + 1];
                    vertices2D.CopyTo(tempArray2d, 0);
                    vertices3D.CopyTo(tempArray3d, 0);
                    tempArray2d[vertices2D.Length] = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

                    tempArray3d[vertices3D.Length] = transform.InverseTransformPoint(creator.GetComponent<MeshCreator>().snapPoint);
                    vertices3D = tempArray3d;
                    line.SetVertexCount(vertices3D.Length + 1);
                    Vector3[] linePoints = new Vector3[vertices3D.Length+1];
                    for (int i = 0; i < vertices3D.Length; i++)
                    {
                        linePoints[i] = vertices3D[i];// + Vector3.up * 0.1f;
                    }
                    linePoints[vertices3D.Length] = linePoints[0];
                    line.SetPositions(linePoints);
                    vertices2D = tempArray2d;
                    UpdateMesh(vertices2D);
                }
                if (firstFrame) firstFrame = false;
            }
            if (Input.GetMouseButtonDown(1) || controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                doesDraw = false;
                doesExtrude = true;
                // Destroy(line);
                // hide the gizmo
                if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y> 0){
                    transform.tag = "transformable";
                }
                transform.GetChild(0).gameObject.SetActive(false);
            }

            }
            else if (doesExtrude)
        {
            //extrudeLength += Input.GetAxis("Mouse Y")*0.2f;
            if(viveAxis.y>-0.2f) extrudeLength += viveAxis.y;


            Vector3 dir = (transform.position - trackedObj.gameObject.transform.position).normalized;
            dir.Scale(new Vector3(1, 0, 1));
            Vector3 dir2 = trackedObj.gameObject.transform.forward;
            if (dir2.y < 0)
            {
                dir2.Scale(new Vector3(1, 0, 1));
            }
            extrudeLength = Mathf.Tan(Vector3.Angle(dir, dir2) *Mathf.Deg2Rad);

            Debug.DrawRay(trackedObj.gameObject.transform.position, dir2, Color.red);
            Debug.DrawRay(trackedObj.gameObject.transform.position, dir, Color.green);

            //viveAxis.y;
            Extrude(extrudeLength);
            if (Input.GetMouseButtonDown(1)|| Input.GetMouseButtonDown(0) ||
                controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)||
                controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)
                )
            {
                AcceptMesh();
            }
        }
        if (doesDraw || doesExtrude)
        {
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                AcceptMesh();
                Destroy(line.gameObject);
                Destroy(gameObject);
            }
        }

    }


    void UpdateMesh(Vector2[] verts)
    {
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(verts);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[verts.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(verts[i].x, verts[i].y, 0);
        }

        if(verts.Length > 3)
        ExtrNormal = Vector3.Cross(vertices3D[indices[0]] - vertices3D[indices[1]], vertices3D[indices[0]] - vertices3D[indices[2]]).normalized;

        Vector3 dir = trackedObj.transform.position - transform.position;

        if (Vector3.Dot(ExtrNormal, dir) < 0) // need to reverse
        {
            ExtrNormal = ExtrNormal * -1;
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices3D;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;

        GetComponent<MeshFilter>().mesh = msh;
    }

    void Extrude(float amount)
    {
        if (!tempMesh) {
            tempMesh = GetComponent<MeshFilter>().mesh;
        }
        
        //twice as high
        newVerts = new Vector3[tempMesh.vertexCount * 2];
        newTris = new int[tempMesh.triangles.Length*2 + tempMesh.vertexCount * 2 * 3];
        tempMesh.vertices.CopyTo(newVerts, 0);
        tempMesh.triangles.CopyTo(newTris, 0);

        for (int i = 0; i < tempMesh.triangles.Length; i++)
        {
            newTris[tempMesh.triangles.Length + i] = tempMesh.triangles[i] + tempMesh.vertexCount;
        }

        for (int i = 0; i < tempMesh.vertexCount; i++)
        {
            newVerts[tempMesh.vertexCount + i] = tempMesh.vertices[i] + ExtrNormal * amount;

                int a, b, c,d;
                a = i;
                b = i + tempMesh.vertexCount + 1;
                c = i + 1;
                d = i + tempMesh.vertexCount;
                if (b== newVerts.Length)
                {
                    b = tempMesh.vertexCount;
                }
                if(c== tempMesh.vertexCount)
                {
                    c = 0;
                }
                newTris[tempMesh.triangles.Length * 2 + 6 * i] = a;
                newTris[tempMesh.triangles.Length * 2 + 6 * i + 1] = c;     
                newTris[tempMesh.triangles.Length * 2 + 6 * i + 2] = b;

                newTris[tempMesh.triangles.Length * 2 + 6 * i +3] = a;
                newTris[tempMesh.triangles.Length * 2 + 6 * i + 4] = b;
                newTris[tempMesh.triangles.Length * 2 + 6 * i + 5] = d;

        }

        for (int i = 0; i < newVerts.Length; i++)
        {
            if(newVerts[i].x<0.001&& newVerts[i].x > -0.001)
            {
                Debug.Log("Vertex " + i + " is zero");
                Debug.Log(newVerts[i]);
            }
        }

        Mesh msh = new Mesh();
        msh.vertices = newVerts;
        msh.triangles = newTris;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = Create2SidedColliderMesh(msh);
    }

    void AcceptMesh()
    {

        if(line)line.enabled = false;
        tempMesh = GetComponent<MeshFilter>().mesh;
        doesExtrude = false;
        doesDraw = false;
        gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<EditableMesh>();
        //gameObject.tag = "transformable";
        GameObject.Find("MeshCenter").GetComponent<MeshCenter>().AddMesh(GetComponent<MeshFilter>());
        transform.parent = GameObject.Find("DrawMesh").transform;
        creator.SendMessage("FinishedDrawing");
    }

    // make a 2sided mesh for collider
    public static Mesh Create2SidedColliderMesh(Mesh input)
    {
        Mesh output;
        output = Instantiate(input) as Mesh;
        int[] _tris = new int[input.triangles.Length * 2];
        input.triangles.CopyTo(_tris, 0);
        for(int i = 0; i < input.triangles.Length; i++)
        {
            _tris[_tris.Length - 1 - i] = input.triangles[i];
        }
        output.triangles = _tris;
        return output;
    }


}