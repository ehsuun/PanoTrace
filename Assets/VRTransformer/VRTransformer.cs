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

public class VRTransformer : MonoBehaviour {


    
    private Transform handleX;
    private Transform handleY;
    private Transform handleZ;
    private Transform handleCenter;

    public float rotationDistance = 1;
    public GameObject handlePrefab;
    public VRTargetSetter targetSetter;
    public Color redC;
    public Color greenC;
    public Color blueC;

    public bool restartScale;

    private Vector3 lastLocalScale;
    private Vector3 lastPosition;
    private Vector3 lastTargetPosition;

    internal Vector3 point1;
    internal Vector3 lastpoint1;
    internal Vector3 point2;
    internal Vector3 lastpoint2;

    internal Vector3 initPoint1;
    internal Vector3 initPoint2;


    internal bool point1Locked;
    internal bool lastPoint1Locked;
    internal bool point2Locked;
    internal bool lastPoint2Locked;
    private bool firstisone; // if first controller is one or two

    private Quaternion lastRotation;


    public Transform target;
    public Transform targetCandidate;
    private Transform lastTarget;
    private Transform lastTargetCandidate;

    public Transform handle1;
    public Transform handle2;
    public Transform dummy;

    private LineRenderer line;
    private Vector3 meanUp;
    private Vector3 smoothMeanup;

    internal bool copyTarget = false;
    internal bool copyBaked = false;
    private GameObject selectionHollow;
    private GameObject selectionCandidateHollow;
    public Material selectionMat;
    public Material candidateMat;

    public GameObject playerHollow;


    // Use this for initialization
    void Start () {

        point1 = Vector3.zero;
        point2 = Vector3.zero;
        lastpoint1 = point1;
        lastpoint2 = point2;
        line = GetComponent<LineRenderer>();
        if (!dummy)
        {
            dummy = new GameObject().transform;
            dummy.name = "dummy";
        }

        if(!candidateMat)
        {
            candidateMat = Resources.Load("SelectionCandidate", typeof(Material)) as Material;
        }

        selectionHollow = new GameObject("selectionHollow");
        selectionHollow.AddComponent<MeshFilter>();
        selectionHollow.AddComponent<MeshRenderer>().sharedMaterial = selectionMat;

        selectionCandidateHollow = new GameObject("selectionCandidateHollow");
        selectionCandidateHollow.AddComponent<MeshFilter>();
        selectionCandidateHollow.AddComponent<MeshRenderer>().sharedMaterial = candidateMat;

        if (!playerHollow)
        {
            GameObject prefab = Resources.Load("PlayerHollow", typeof(GameObject)) as GameObject;
            playerHollow = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            playerHollow.name = "PlayerHollow";
        }

    }
	
	// Update is called once per frame
	void Update () {


        if (target)
        {
            selectionHollow.SetActive(true);

            if (lastTarget != target)
            {
                if (target)
                {
                    selectionHollow.GetComponent<MeshFilter>().sharedMesh = target.GetComponent<MeshFilter>().sharedMesh;
                }
                
            }

            if (copyTarget)
            {

                //targetSetter.LearnColor(target);

                target = Instantiate(target.gameObject).transform;
                if (copyBaked)
                {
                    if (target.GetComponent<TextureBaker>())
                    {
                        target.GetComponent<TextureBaker>().BakeTexture();
                    }
                    copyBaked = false;
                }
                targetSetter.Select(target);
                copyTarget = false;
            }

            TransformBasedOnPoints();

            selectionHollow.transform.position = target.position;
            selectionHollow.transform.rotation = target.rotation;
            selectionHollow.transform.localScale = target.lossyScale;

        }else
        {
            copyTarget = false;
            selectionHollow.SetActive(false);
            ScalePlayerBasedOnPoints();

            if (point1Locked && point2Locked)
            {
                playerHollow.SetActive(true);
            }else
            {
                playerHollow.SetActive(false);
            }
        }

        if (targetCandidate)
        {
            if (lastTargetCandidate != targetCandidate)
            {
                selectionCandidateHollow.SetActive(true);
                selectionCandidateHollow.GetComponent<MeshFilter>().sharedMesh = targetCandidate.GetComponent<MeshFilter>().sharedMesh;

            }
            selectionCandidateHollow.transform.position = targetCandidate.position;
            selectionCandidateHollow.transform.rotation = targetCandidate.rotation;
            selectionCandidateHollow.transform.localScale = targetCandidate.lossyScale;


        }
        else
        {
            if (lastTargetCandidate != targetCandidate)
            {
                selectionCandidateHollow.SetActive(false);
            }
        }

        lastTargetCandidate = targetCandidate;



    }


    void TransformBasedOnPoints()
    {
        Vector3 lastLook = lastpoint2 - lastpoint1;
        Vector3 look = point2 - point1;

        Vector3 delta1 = point1 - lastpoint1;
        Vector3 delta2 = point2 - lastpoint2;

        Vector3 up1 = Vector3.Cross(point1 - point2, delta1);
        Vector3 up2 = Vector3.Cross(point2 - point1, delta2);



        if ((up1-up2 ).magnitude > 0.01f)
        {
            meanUp = (up1-up2 ).normalized;
        }
        smoothMeanup = Vector3.Lerp(smoothMeanup, meanUp, Time.deltaTime * 40);

        line.SetPosition(0, point1);
        line.SetPosition(1, point1 + smoothMeanup * 0.3f);

        if ((lastPoint1Locked&&point1Locked)||(lastPoint2Locked&&point2Locked))
        {
            target.SetParent(dummy, true);
            
        }

        if (point1Locked && !point2Locked)
        {
            // if last frame was two handed
            if (lastPoint1Locked && lastPoint2Locked)
            {
                
                target.SetParent(null, true);
            }
            dummy.position = point1;

            if ((target.position - point1).magnitude < handle1.parent.localScale.magnitude * rotationDistance)
            {
                dummy.rotation = handle1.rotation;
            }
            firstisone = true;
        }
        else

        if (point2Locked && !point1Locked)
        {
            // if last frame was two handed
            if (lastPoint1Locked && lastPoint2Locked)
            {
                
                target.SetParent(null, true);
            }

            dummy.position = point2;
            if ((target.position - point2).magnitude < handle1.parent.localScale.magnitude* rotationDistance)
            {
                dummy.rotation = handle2.rotation;
            }
            firstisone = false;
        }
        else

        if(point1Locked&& point2Locked)
        {
            
            if (firstisone)
            {
                dummy.position = point1;
            }
            else
            {
                dummy.position = point2;
                
            }
            // if last frame was not two handed
            if (!(lastPoint1Locked && lastPoint2Locked))
            {
                initPoint1 = point1;
                initPoint2 = point2;
                target.SetParent(null, true);
            }
            dummy.LookAt((point1 + point2) / 2, (handle1.up + handle2.up).normalized);
            dummy.localScale = (point1 - point2).magnitude / (initPoint1 - initPoint2).magnitude * Vector3.forward + Vector3.up + Vector3.right;
            //dummy.localScale = (point1 - point2).magnitude / (initPoint1 - initPoint2).magnitude * Vector3.one;
       
        }
        else
        {

        }
        target.SetParent(null, true);
        dummy.localScale = Vector3.one;
        initPoint1 = point1;
        initPoint2 = point2;
        //target.SetParent(null, true);

        lastTarget = target;
        lastPoint1Locked = point1Locked;
        lastPoint2Locked = point2Locked;
        lastpoint1 = point1;
        lastpoint2 = point2;

    }



    void ScalePlayerBasedOnPoints()
    {
        Transform player = handle1.parent;
        Vector3 Ipoint1 = player.InverseTransformPoint(point1);
        Vector3 Ipoint2 = player.InverseTransformPoint(point2);
        Vector3 lastLook = lastpoint2 - lastpoint1;
        Vector3 look = point2 - point1;

        Vector3 delta1 = Ipoint1 - lastpoint1;
        Vector3 delta2 = Ipoint2 - lastpoint2;

        Vector3 up1 = Vector3.Cross(Ipoint1 - Ipoint2, delta1);
        Vector3 up2 = Vector3.Cross(Ipoint2 - Ipoint1, delta2);

        dummy.position = (point1 + point2) / 2;

        if (point1Locked && point2Locked)
        {
            if (firstisone)
            {
                dummy.position = (point1 + point2) /2;
            }
            else
            {
                dummy.position = (point1 + point2) / 2;

            }
        }

        if ((up1 - up2).magnitude > 0.01f)
        {
            meanUp = (up1 - up2).normalized;
        }
        smoothMeanup = Vector3.Lerp(smoothMeanup, meanUp, Time.deltaTime * 40);

        line.SetPosition(0, Ipoint1);
        line.SetPosition(1, Ipoint1 + smoothMeanup * 0.3f);

        if ((lastPoint1Locked && point1Locked) || (lastPoint2Locked && point2Locked))
        {
            player.SetParent(dummy, true);
        }

        if (point1Locked && point2Locked)
        {
            if (firstisone)
            {
                //dummy.position = point1;
            }
            else
            {
                //dummy.position = point2;

            }

            // if last frame was not two handed
            if (!(lastPoint1Locked && lastPoint2Locked))
            {
                initPoint1 = Ipoint1;
                initPoint2 = Ipoint2;
                //player.SetParent(null, true);
            }
            
            
            dummy.localScale = (lastpoint1 - lastpoint2).magnitude / (Ipoint1 - Ipoint2).magnitude * Vector3.one;
            Vector3 dist = initPoint1 - Ipoint1;
            
        }

        //dummy.LookAt((point1 + point2) / 2, (handle1.up + handle2.up).normalized);

        player.SetParent(null, true);
        dummy.localScale = Vector3.one;
        initPoint1 = Ipoint1;
        initPoint2 = Ipoint2;


        lastTarget = target;
        lastPoint1Locked = point1Locked;
        lastPoint2Locked = point2Locked;
        lastpoint1 = Ipoint1;
        lastpoint2 = Ipoint2;

    }

}
