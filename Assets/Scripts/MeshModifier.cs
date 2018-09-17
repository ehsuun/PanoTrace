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

public class MeshModifier : MonoBehaviour {
    public bool grab;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;


    // a dummy object to contain mesh data
    public MeshCenter meshCenter;

    private Vector3[] tempVertices;
    private Vector3[] tempVertices2;
    public float brushSize;
    private float[] selectionWeight;
    private Vector3 lastPosition;

    public MeshFilter nearest;

    private float shortestDistinUpdate;

    // Use this for initialization
    void Start()
    {
        

        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!meshCenter)
        {
            if (GameObject.Find("MeshCenter"))
            {
                meshCenter = GameObject.Find("MeshCenter").GetComponent<MeshCenter>();
            }
        }

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            transform.parent.SendMessage("ToggleWireframe");
        }

            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            foreach (MeshFilter editingMesh in meshCenter.editingMeshes)
            {
                
                if (editingMesh != null)
                {
                    nearest = meshCenter.editingMeshes[0];
                    for (int i = 0; i < editingMesh.mesh.vertexCount; i++)
                    {
                        float dist = (editingMesh.transform.TransformPoint(editingMesh.mesh.vertices[i]) - transform.position).magnitude;
                        if (dist < shortestDistinUpdate)
                        {
                            shortestDistinUpdate = dist;
                            nearest = editingMesh;
                            Debug.Log(nearest);
                        }
                    }
                }
            }
            shortestDistinUpdate = Mathf.Infinity;
        }

       

            foreach (MeshFilter editingMesh in meshCenter.editingMeshes)
        {
            if (editingMesh!=null)
            {

                if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    grab = true;
                    lastPosition = transform.position;
                    tempVertices = nearest.mesh.vertices;
                    tempVertices2 = nearest.mesh.vertices;
                    selectionWeight = new float[tempVertices.Length];

                    for (int i = 0; i < tempVertices.Length; i++)
                    {
                        float dist = (nearest.transform.TransformPoint(tempVertices[i]) - transform.position).magnitude;
                        if (dist < brushSize)
                        {
                            Debug.Log(i);
                            // the selection function can be defined here
                            selectionWeight[i] = 1f;
                            selectionWeight[i] = Mathf.Clamp(((brushSize - dist) / brushSize) * 2, 0, 1);

                        }
                        else
                        {
                            selectionWeight[i] = 0;
                        }
                    }
                }


            if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                grab = false;
                   if(editingMesh.GetComponent<MeshCollider>())
                    {
                        editingMesh.GetComponent<MeshCollider>().sharedMesh = editingMesh.mesh;
                    }
            }
            if (grab) // continue editing as long as grab is true
            {
                for (int i = 0; i < tempVertices.Length; i++)
                {
                    tempVertices2[i] = tempVertices[i] + editingMesh.transform.InverseTransformVector(transform.position - lastPosition) * selectionWeight[i];
                }
                    nearest.mesh.vertices = tempVertices2;
                    nearest.mesh.RecalculateNormals();
                    nearest.mesh.RecalculateBounds();
                }


        }
        }

        
    }
}
