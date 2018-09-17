using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class DeselectNonMeshScript{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class DeselectNonMesh : ScriptableObject
{
    [MenuItem("GameObject/Deselect NonMesh")]
    static void DeselectNonMeshObjects()
    {
        List<GameObject> objs = new List<GameObject>();

        foreach(GameObject o in Selection.objects)
        {
            if (o.GetComponent<MeshRenderer>())
            {
                objs.Add(o);
            }
        }

        Selection.objects = objs.ToArray();
    }
}
