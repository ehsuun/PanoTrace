using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeRenderer : MonoBehaviour {

    private MeshRenderer mrender;
    private MeshFilter filter;

    public float start = 0;
    public float end =1 ;
    public float radiusStart = 1;
    public float radiusEnd = 1;

    private int[] startInd;
    private int[] endInd;

    private Mesh mesh;
    private Mesh ring;
    public Material mat;
    private List<Vector3> currentVerts;
    private List<Vector3> currentNorms;
    private List<int> currentTris;

    // Use this for initialization
    void Start () {

        currentVerts = new List<Vector3>();
        currentNorms = new List<Vector3>();
        currentTris = new List<int>();

        mrender = gameObject.AddComponent<MeshRenderer>();
        mrender.material = mat;
        filter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;
        ring = CreateRing(8);

        for (int i = 0; i < ring.vertexCount; i++)
        {
            currentVerts.Add(ring.vertices[i] * radiusStart + Vector3.up * start);
            currentNorms.Add(ring.vertices[i].normalized);
        }

        for (int i = 0; i < ring.vertexCount; i++)
        {
            currentVerts.Add(ring.vertices[i] * radiusEnd + Vector3.up * end);
            currentNorms.Add(ring.vertices[i].normalized);
        }

        for (int i = 0; i < ring.vertexCount-1; i++)
        {
            List<int> twotris = new List<int> { i, i+ ring.vertexCount, i+1,
                                                                i+ ring.vertexCount,i+ ring.vertexCount+1,i+ 1 };
            currentTris.AddRange(twotris);
        }

        mesh.vertices = currentVerts.ToArray();
        mesh.triangles = currentTris.ToArray();
        mesh.normals = currentNorms.ToArray();
        
    }
	
	// Update is called once per frame
	void Update () {
        //firstSide

	    for(int i =0; i<ring.vertexCount; i++)
        {
            currentVerts[i] = (ring.vertices[i] * radiusStart+Vector3.up* start);
        }

        for (int i = ring.vertexCount; i < ring.vertexCount*2; i++)
        {
            currentVerts[i]  = ring.vertices[i- ring.vertexCount] * radiusEnd + Vector3.up * end;
        }

        mesh.vertices = currentVerts.ToArray();

    }

    Mesh CreateRing(int sides)
    {
        Mesh output = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();

        for (int i = 0; i < sides + 1; i++)
        {
            float phase = i * (2 * Mathf.PI / (float)sides);
            verts.Add(new Vector3(Mathf.Cos(phase), 0, Mathf.Sin(phase)));
            norms.Add(verts[i].normalized);
        }

        output.vertices = verts.ToArray();
        output.normals = norms.ToArray();
        return output;
    }
}
