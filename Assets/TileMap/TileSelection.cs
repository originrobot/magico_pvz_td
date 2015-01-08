using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMap))]
public class TileSelection : MonoBehaviour {

	TileMap _tileMap;
	
	public string selectionQuadPrefabName;
	public Color highlightColor;
	public Color blockingColor;
	public string deployableQuadPrefab;
	GameObject selectionQuadPrefab;
	List<GameObject> selectionQuads = new List<GameObject>();
	List<List<GameObject>> deployableQuads = new List<List<GameObject>>();
	
	void Start() {
		_tileMap = GetComponent<TileMap>();
		selectionQuadPrefab = Resources.Load(selectionQuadPrefabName) as GameObject;
		if (selectionQuadPrefab != null) {
			GameObject selectionQuad = GameObject.Instantiate(selectionQuadPrefab) as GameObject;
			selectionQuad.GetComponent<Renderer>().enabled = false;
			selectionQuads.Add(selectionQuad);
		}

		GameObject deployablePrefab = Resources.Load (deployableQuadPrefab) as GameObject;
		if (deployablePrefab != null) 
		{
			int col = _tileMap.size_x;
			int row = _tileMap.size_y;
			int vsize_x = _tileMap.size_x + 1;
			int vsize_y = _tileMap.size_y + 1;
			int offset_x = -(vsize_x+1) / 2;
			int offset_y = -(vsize_y+1) / 2;
			for (int x = 0; x < col; ++x)
			{
				List<GameObject> colQuads = new List<GameObject>();
				for (int y = 0; y < row; ++y)
				{
					GameObject deployableQuad = GameObject.Instantiate(deployablePrefab) as GameObject;
					deployableQuad.GetComponent<Renderer>().enabled = false;

					// set its position
					float minx = (x * _tileMap.tileSize + offset_x) * _tileMap.sizefactor_x;
					float miny = (y * _tileMap.tileSize + offset_y) * _tileMap.sizefactor_y;
					float maxx = ((x + 1) * _tileMap.tileSize + offset_x) * _tileMap.sizefactor_x;
					float maxy = ((y + 1) * _tileMap.tileSize + offset_y) * _tileMap.sizefactor_y;
					Vector3 vResult = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, 0.0f);
					deployableQuad.transform.position = transform.TransformPoint(vResult);

					// add to col list
					colQuads.Add(deployableQuad);
				}

				// add the whole col list to final list
				deployableQuads.Add(colQuads);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void GetTileIndex(Vector3 pos, out int x, out int y)
	{
		int vsize_x = _tileMap.size_x + 1;
		int vsize_y = _tileMap.size_y + 1;
		int offset_x = -(vsize_x+1) / 2;
		int offset_y = -(vsize_y+1) / 2;
		
		Vector3 localPos = transform.InverseTransformPoint(pos);
		x = Mathf.FloorToInt( (localPos.x / _tileMap.sizefactor_x - offset_x) / _tileMap.tileSize);
		y = Mathf.FloorToInt( (localPos.y / _tileMap.sizefactor_y - offset_y) / _tileMap.tileSize);
	}

	Vector3 GetTileCenter(Vector3 pos)
	{
		int vsize_x = _tileMap.size_x + 1;
		int vsize_y = _tileMap.size_y + 1;
		int offset_x = -(vsize_x+1) / 2;
		int offset_y = -(vsize_y+1) / 2;
		
		Vector3 localPos = transform.InverseTransformPoint(pos);
		int x = Mathf.FloorToInt( (localPos.x / _tileMap.sizefactor_x - offset_x) / _tileMap.tileSize);
		int y = Mathf.FloorToInt( (localPos.y / _tileMap.sizefactor_y - offset_y) / _tileMap.tileSize);
		
		float minx = (x * _tileMap.tileSize + offset_x) * _tileMap.sizefactor_x;
		float miny = (y * _tileMap.tileSize + offset_y) * _tileMap.sizefactor_y;
		float maxx = ((x + 1) * _tileMap.tileSize + offset_x) * _tileMap.sizefactor_x;
		float maxy = ((y + 1) * _tileMap.tileSize + offset_y) * _tileMap.sizefactor_y;

		Vector3 vResult = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, localPos.z);
		vResult = transform.TransformPoint(vResult);

		return vResult;
	}

	void ResolveTiles(int tileCount)
	{
		while (selectionQuads.Count < tileCount) {
			GameObject selectionQuad = GameObject.Instantiate(selectionQuadPrefab) as GameObject;
			selectionQuad.GetComponent<Renderer>().enabled = false;
			selectionQuads.Add(selectionQuad);
		}
	}
	
	public void SelectUnit(Vector3 pos, bool enemy, int attackRange, bool blocking = false, bool rowOnly = false, bool colOnly = false)
	{
		UnSelectUnit();
		int xIdx = 0;
		int yIdx = 0;
		GetTileIndex(pos, out xIdx, out yIdx);
		int xMin = 0;
		int xMax = xIdx + attackRange;
		xMax = Mathf.Clamp(xMax, 0, _tileMap.size_x - 1);
		xMax = xMax - xIdx;
		if (enemy == true) {
			xMax = 0;
			xMin = xIdx - attackRange;
			xMin = Mathf.Clamp(xMin, 0, _tileMap.size_x - 1);
			xMin = xMin - xIdx;
		}
		int yMin = yIdx - attackRange;
		yMin = Mathf.Clamp(yMin, 0, _tileMap.size_y - 1);
		yMin = yMin - yIdx;
		int yMax = yIdx + attackRange;
		yMax = Mathf.Clamp(yMax, 0, _tileMap.size_y - 1);
		yMax = yMax - yIdx;
		int tileCount = (xMax - xMin + 1) * (yMax - yMin + 1);
		ResolveTiles(tileCount);
		Vector3 basePos = GetTileCenter(pos);
		tileCount = 0;
		//Debug.LogError ("xMax:" + xMax + ", yMin:" + yMin + ", yMax:" + yMax);
		if(rowOnly) yMax = yMin;
		if (colOnly) xMax = xMin;
		for (int x = xMin; x < xMax + 1; ++x) {
			for (int y = yMin; y < yMax + 1; ++y) {
				Vector3 currentTilePos;
				currentTilePos.x = basePos.x + x * _tileMap.tileSize * _tileMap.sizefactor_x;
				currentTilePos.y = basePos.y + y * _tileMap.tileSize * _tileMap.sizefactor_y;
				currentTilePos.z = basePos.z;
				Ray ray = new Ray();
				ray.origin = new Vector3(currentTilePos.x, currentTilePos.y, -10.0f);
				ray.direction = new Vector3(0.0f, 0.0f, 1.0f);
				RaycastHit hitInfo;
				if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
					currentTilePos.z = hitInfo.point.z;
				}
				else
				{
					Debug.LogError("woops, missed");
				}
				if ( (Mathf.Abs(x) + Mathf.Abs(y)) <= attackRange ) {
					selectionQuads[tileCount].transform.position = GetTileCenter(currentTilePos);
					if (blocking) selectionQuads[tileCount].GetComponent<Renderer>().material.SetColor("_SpecColor",blockingColor);
					else selectionQuads[tileCount].GetComponent<Renderer>().material.SetColor("_SpecColor",highlightColor);
					selectionQuads[tileCount].GetComponent<Renderer>().enabled = true;
					++tileCount;
				}
			}
		}
	}

	public void UnSelectUnit()
	{
		for (int idx = 0; idx < selectionQuads.Count; ++idx) {
			selectionQuads[idx].GetComponent<Renderer>().enabled = false;
		}
	}

	public void ShowDeployableQuads(int frontline, bool applyToEnemy = false)
	{
		int endCol = frontline;
		int startCol = endCol-BattleGrid.instance.cols/2+1>=0?endCol-BattleGrid.instance.cols/2+1:0;

		if (applyToEnemy == true) 
		{
			startCol = frontline;
			endCol = _tileMap.size_x - 1;
		}

		for (int col = startCol; col <= endCol; ++col) 
		{
			List<GameObject> colQuads = deployableQuads[col];
			foreach (GameObject quad in colQuads)
			{
				quad.GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public void HideDeployableQuads()
	{
		foreach (List<GameObject> colQuads in deployableQuads) 
		{
			foreach (GameObject quad in colQuads)
			{
				quad.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
