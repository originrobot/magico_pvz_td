using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x = 8;
	public int size_y = 5;
	public float tileSize = 1.0f;
	public float sizefactor_x = 1.0f;
	public float sizefactor_y = 1.0f;
	private List<Vector3> tileCenters = new List<Vector3>();

	public List<Vector3> TileCenters
	{
		get { return tileCenters; }
	}

	void Awake()
	{
		BuildMesh();
		resolveCenters();
	}
	
	// Use this for initialization
	void Start () {
	}
	public void OnTap(TapGesture gesture)
	{
		BattleGrid.instance.OnTap(gesture);
	}
	private void resolveCenters()
	{
		int vsize_x = size_x + 1;
		int vsize_y = size_y + 1;
		int offset_x = -(vsize_x+1) / 2;
		int offset_y = -(vsize_y+1) / 2;
		for (int x = 0; x < size_x; ++x)
		{
			for (int y = 0; y < size_y; ++y)
			{
				float minx = (x * tileSize + offset_x) * sizefactor_x;
				float miny = (y * tileSize + offset_y) * sizefactor_y;
				float maxx = ((x + 1) * tileSize + offset_x) * sizefactor_x;
				float maxy = ((y + 1) * tileSize + offset_y) * sizefactor_y;
				Vector3 vResult = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, 0.0f);
				Vector3 center = transform.TransformPoint(vResult);
				tileCenters.Add(center);
			}
		}
	}
	
	public void BuildMesh() {
		
		int numTiles = size_x * size_y;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_y = size_y + 1;
		int numVerts = vsize_x * vsize_y;

		int offset_x = -(vsize_x+1) / 2;
		int offset_y = -(vsize_y+1) / 2; 
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, y;
		for(y=0; y < vsize_y; y++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ y * vsize_x + x ] = new Vector3( (x*tileSize + offset_x)*sizefactor_x, (y*tileSize + offset_y)*sizefactor_y, 0 );
				normals[ y * vsize_x + x ] = new Vector3(0.0f, 0.0f, -1.0f);
				uv[ y * vsize_x + x ] = new Vector2( (float)x / vsize_x, (float)y / vsize_y );
			}
		}

		for(y=0; y < size_y; y++) {
			for(x=0; x < size_x; x++) {
				int squareIndex = y * size_x + x;
				int triOffset = squareIndex * 6;
				triangles[triOffset + 0] = y * vsize_x + x + 		   0;
				triangles[triOffset + 1] = y * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 2] = y * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = y * vsize_x + x + 		   0;
				triangles[triOffset + 4] = y * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 5] = y * vsize_x + x + 		   1;
			}
		}

		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
	}
}
