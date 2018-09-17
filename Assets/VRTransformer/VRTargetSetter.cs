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

public class VRTargetSetter : MonoBehaviour {
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;

    public LayerMask layerMask;
    public string selectionTag;
    public Transform lastTarget;
    public Transform target;

    public Transform lastTransformerTarget;

    public VRTransformer transformer;


    private Color transformerSavedColor;
    private Color savedColor;
    private Color color;

    public Color Indication;
    public Color selection;
    public LineRenderer line;
    public PipeRenderer selectionCone;

    public float deselectAngle = 10;

	// Use this for initialization
	void Start () {
        savedColor = Color.white;
        if (!trackedObj)
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        line = GetComponent<LineRenderer>();
        //selectionCone = GameObject.Find("SelectionCone").GetComponent<PipeRenderer>();
        if (!selectionCone)
        {
            selectionCone = new GameObject("SelectionCone").AddComponent<PipeRenderer>();
            selectionCone.mat = Resources.Load<Material>("redTrans") as Material;
        }
        selectionCone.gameObject.SetActive(false);
        selectionCone.radiusStart = 0;
    }
	
	// Update is called once per frame
	void Update () {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * 100);

        selectionCone.transform.position = transform.position;
        

        if (transformer.target)
        {
            selectionCone.transform.LookAt(transformer.target,Vector3.up);
            selectionCone.transform.Rotate(Vector3.right, 90, Space.Self);
            selectionCone.end = (transformer.target.position - transform.position).magnitude;
            selectionCone.radiusEnd = Mathf.Sin(deselectAngle * Mathf.Deg2Rad) * selectionCone.end;
        }

        RaycastHit hit;
	    if(Physics.Raycast(transform.position,transform.forward,out hit,1000, layerMask.value)){
            line.SetPosition(1, hit.point);
            target = hit.transform;
            if(hit.transform.tag == selectionTag)
            {
                if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
                {
                    transformer.target = target;
                    selectionCone.gameObject.SetActive(true);



                    if (lastTransformerTarget != transformer.target)
                    { // we are assigning a new transformer target
                        if (lastTransformerTarget) {
                            //lastTransformerTarget.GetComponent<Renderer>().material.color = transformerSavedColor;
                        }

                        transformerSavedColor = savedColor;
                    }

                }else
                {
                    transformer.targetCandidate = target;
                }
            }else // tag does not match
            {
                target = null;
            }
        }else // no hit
        {
            transformer.targetCandidate = null;
            if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                if (transformer.target)
                {
                    Debug.Log(Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(transformer.target.position).normalized));
                    if ((Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(transformer.target.position).normalized) < Mathf.Cos(deselectAngle*Mathf.Deg2Rad)) &&
                     (transform.InverseTransformPoint(transformer.target.position)).magnitude > 0.3f)
                    {
                        DeselectTarget();
                    }
                }


            }
        }

        if (lastTarget != target)
        { 
          // if we just moved from target to nothing or another target
          //first we should change the previous target to normal color
            if (lastTarget)
            {
                //lastTarget.GetComponent<Renderer>().material.color = savedColor;
            }
            if (target)
            {
                //color = Indication;
                //savedColor = target.GetComponent<Renderer>().material.color;
                //target.GetComponent<Renderer>().material.color = Indication;
            }

            if (transformer.target)
                Select(transformer.target);
        }

        lastTarget = target;
        if (transformer.target)
            lastTransformerTarget = transformer.target;

    }

    public void LearnColor(Transform t)
    {
        //lastTransformerTarget.GetComponent<Renderer>().material.color = transformerSavedColor;
    }

    public void DeselectTarget()
    {
        target = null;
        transformer.target = null;
        selectionCone.gameObject.SetActive(false);
    }

    public void Select(Transform t)
    {
        //t.GetComponent<Renderer>().material.color = selection;
    }
}
