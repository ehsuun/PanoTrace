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

public class CubeSnap : MonoBehaviour
{
    internal Cubemap cubemap;
    internal CubemapFace face;
    internal Vector2 pos;

    public Transform pointer;
    public Transform pointer2;

    private Transform positiveX;
    private Transform positiveY;
    private Transform positiveZ;
    private Transform negativeX;
    private Transform negativeY;
    private Transform negativeZ;

    private Transform faceTransform;
    public Transform projector;
    public bool useController;

    private int pixelYmod;
    private int pixelXmod;
    internal int pixelY;
    internal int pixelX;

    private float currentColor;
    private float lastColor;

    public void FaceSeletor( Vector3 dir, out CubemapFace face, out Vector2 pos , out Transform faceT)
    {
        
        float largest =-9999999;
        Vector3 proj;
        pos = new Vector2(0, 0);

        face = CubemapFace.PositiveZ;
        faceT = positiveZ;
        float f = Vector3.Dot(dir, Vector3.forward);
        if(largest < f)
        {
            largest = f;
            face = CubemapFace.PositiveZ; // z = 0.5f
            faceT = positiveZ;
            proj = (0.5f / dir.z) * dir;
            pos = new Vector2(proj.x+0.5f, 1-(proj.y+0.5f));
        }
        float b = Vector3.Dot(dir, Vector3.back);
        if (largest < b)
        {
            largest = b;
            face = CubemapFace.NegativeZ; // z = -0.5;
            faceT = negativeZ;
            proj = (-0.5f / dir.z) * dir;
            pos = new Vector2(1-(proj.x + 0.5f), 1-(proj.y + 0.5f));
        }
        float u = Vector3.Dot(dir, Vector3.up);
        if (largest < u)
        {
            largest = u;
            face = CubemapFace.PositiveY;
            faceT = positiveY;
            proj = (0.5f / dir.y) * dir; // y = 0.5;
            pos = new Vector2(proj.x + 0.5f, (proj.z + 0.5f));
        }
        float d = Vector3.Dot(dir, Vector3.down);
        if (largest < d)
        {
            largest = d;
            face = CubemapFace.NegativeY;
            faceT = negativeY;
            proj = (-0.5f / dir.y) * dir; // y =- 0.5;
            pos = new Vector2(proj.x + 0.5f, 1-(proj.z + 0.5f));
        }
        float l = Vector3.Dot(dir, Vector3.left);
        if (largest < l)
        {
            largest = l;
            face = CubemapFace.NegativeX;
            faceT = negativeX;
            proj = (-0.5f / dir.x) * dir; // x = -0.5;
            pos = new Vector2(proj.z + 0.5f, 1-(proj.y + 0.5f));
        }
        float r = Vector3.Dot(dir, Vector3.right);
        if (largest < r)
        {
            largest = r;
            face = CubemapFace.PositiveX;
            faceT = positiveX;
            proj = (0.5f / dir.x) * dir; // x = 0.5;
            pos = new Vector2(1 - (proj.z + 0.5f), 1-(proj.y + 0.5f));
        }

        

    }

    Vector2 SnaptoTexture(int x, int y, Cubemap tex ,CubemapFace cFace)
    {
        int lowesti = pixelXmod;
        int lowestj = pixelYmod;
       // Debug.Log("snapping on" + cFace+" x = " +x+" y= "+y);
        while (currentColor > lastColor)
        {
            for (int i = -5; i < 6; i++)
            {
                for (int j = -5; j < 6; j++)
                {
                    if (tex.GetPixel(cFace,pixelXmod + i, pixelYmod + j).grayscale > tex.GetPixel(cFace,lowesti, lowestj).grayscale)
                    {
                        lowesti = pixelXmod + i;
                        lowestj = pixelYmod + j;
                    }
                }
            }

            currentColor = tex.GetPixel(cFace,lowesti, lowestj).grayscale;
            lastColor = tex.GetPixel(cFace,pixelXmod, pixelYmod).grayscale;

            //if (currentColor > lastColor) Debug.Log(currentColor + " morethan " + lastColor);
            pixelYmod = lowestj;
            pixelXmod = lowesti;
        }
        return new Vector2((float)pixelXmod / (float)tex.width, (float)pixelYmod / (float)tex.height);
    }
    void Start()
    {
        currentColor = 100;

        positiveX = new GameObject("positiveX").transform;
        positiveX.LookAt(Vector3.right, Vector3.up);
        positiveY = new GameObject("positiveY").transform;
        positiveY.LookAt(Vector3.up, Vector3.up);
        positiveZ = new GameObject("positiveZ").transform;
        positiveZ.LookAt(Vector3.forward, Vector3.up);
        negativeX = new GameObject("negativeX").transform;
        negativeX.LookAt(Vector3.left, Vector3.up);
        negativeY = new GameObject("negativeY").transform;
        negativeY.LookAt(Vector3.down, Vector3.up);
        negativeZ = new GameObject("negativeZ").transform;
        negativeZ.LookAt(Vector3.back, Vector3.up);


        positiveX.hideFlags = HideFlags.HideInHierarchy;
        positiveY.hideFlags = HideFlags.HideInHierarchy;
        positiveZ.hideFlags = HideFlags.HideInHierarchy;
        negativeX.hideFlags = HideFlags.HideInHierarchy;
        negativeY.hideFlags = HideFlags.HideInHierarchy;
        negativeZ.hideFlags = HideFlags.HideInHierarchy;

    }

    void Update()
    {


        currentColor = 100;

        if (cubemap == null)
        {
            if (useController)
            {
                cubemap = gameObject.GetComponent<CubeMapCanny>().cannyCubemap;
            }
            else
            {
                cubemap = Camera.main.GetComponent<CubeMapCanny>().cannyCubemap;
            }
            
        }
        else
        {
            pixelXmod = Mathf.FloorToInt(pos.x * cubemap.width);
            pixelYmod = Mathf.FloorToInt(pos.y * cubemap.width);
            if (useController)
            {/* // working without projection
                pointer.position = transform.position + transform.forward*500;
                FaceSeletor(pointer.position.normalized, out face, out pos, out faceTransform);
                Vector2 tempPos = SnaptoTexture(Mathf.FloorToInt(pos.x * cubemap.width), Mathf.FloorToInt(pos.y * cubemap.width), cubemap, face);
                pointer2.position = faceTransform.TransformPoint(new Vector3(tempPos.x - 0.5f, (1 - tempPos.y - 0.5f), 0.5f).normalized * 499);
            */

                int layerMask = 1 << 8;
                Ray ray = new Ray(transform.position, transform.forward);
                //pointer.position = transform.position + transform.forward * 500;
                RaycastHit hit;
                if (Physics.Raycast(ray.origin,ray.direction, out hit,10000, layerMask))
                {
                    Ray ray2 = new Ray(projector.position, hit.point-projector.position);
                    RaycastHit hit2;
                    if (Physics.Raycast(ray2.origin, ray2.direction, out hit2, 10000, layerMask))
                    {
                        pointer.position = hit.point;
                        FaceSeletor((pointer.position - projector.position).normalized, out face, out pos, out faceTransform);
                        Vector2 tempPos = SnaptoTexture(Mathf.FloorToInt(pos.x * cubemap.width), Mathf.FloorToInt(pos.y * cubemap.width), cubemap, face);


                        pointer2.position = faceTransform.TransformPoint(new Vector3(tempPos.x - 0.5f, (1 - tempPos.y - 0.5f), 0.5f).normalized * 499);
                        Ray ray3 = new Ray(projector.position, pointer2.position - projector.position);
                        RaycastHit hit3;
                        if (Physics.Raycast(ray3.origin, ray3.direction, out hit3, 10000, layerMask))
                        {
                            pointer2.position = hit3.point;
                        }
                    }

                }

            }

            else
            {
                pointer.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)).normalized * 500;
                FaceSeletor(pointer.position.normalized, out face, out pos, out faceTransform);
                Vector2 tempPos = SnaptoTexture(Mathf.FloorToInt(pos.x * cubemap.width), Mathf.FloorToInt(pos.y * cubemap.width), cubemap, face);
                pointer2.position = faceTransform.TransformPoint(new Vector3(tempPos.x - 0.5f, (1 - tempPos.y - 0.5f), 0.5f).normalized * 499);
            }

            //pointer2.position = new Vector3(tempPos.x - 0.5f, (1 - tempPos.y - 0.5f), 0.5f).normalized * 499;
        }

    }
}