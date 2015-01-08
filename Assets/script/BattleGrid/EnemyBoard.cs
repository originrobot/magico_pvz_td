using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyBoard : MonoBehaviour {
	
	public int cols;
	public int rows;
	public Vector2 cellSize { get; private set; }
	public BoxCollider boxCollider;
	public Material mat; 


	private Dictionary<string,object> boardData = new Dictionary<string, object>();

	private List<EnemyGridObject[,]> listObjects = new List<EnemyGridObject[,]>();
	private const string UNIT_DISPLAY_NAME= "unit";
	private const string UNIT_STORE_NAME = "unit";

	private int currentBoard = 1;

	void Awake () {
		gameObject.tag = "EnemyBoard";

		// set texture tiling
		transform.FindChild("grid").GetComponent<Renderer>().material.mainTextureScale = new Vector2(cols, rows);
	}

	void OnValidate()
	{
		cellSize = new Vector2 (boxCollider.transform.localScale.x * boxCollider.size.x / cols, 
		                        boxCollider.transform.localScale.y * boxCollider.size.y / rows);
		
		
		mat.mainTextureScale = new Vector2 (cols, rows);
	}
	void Start()
	{
		EnemyGridObject[,] tmpObjects = new EnemyGridObject[cols,rows];
		listObjects.Add(tmpObjects);
	}

	private void saveCurrentBoard(bool deactiveUnit = true)
	{
		List<object> wave1Data = new List<object>();


		EnemyGridObject[,] tmpObjects = listObjects[currentBoard - 1];
		for (int i=0; i<cols; i++) 
		{
			for (int j=0; j<rows; j++)
			{
				if(tmpObjects[i,j] != null)
				{
					EnemyGridObject ego = tmpObjects[i,j];
					if (deactiveUnit) ego.gameObject.SetActive(false);
					Dictionary<string,object> unitDic = new Dictionary<string,object>();
					unitDic["id"] = ego.id;
					unitDic["name"] = ego.name;
					unitDic["row"] = j;
					unitDic["col"] = i;
					wave1Data.Add(unitDic);
				}
			}
		}

		int keyIdx = currentBoard;
		boardData["wave" + keyIdx.ToString()] = wave1Data;
	}

	private void applyDataToBoard()
	{
		EnemyGridObject[,] tmpObjects = null;
		if (currentBoard > listObjects.Count)
		{
			tmpObjects = new EnemyGridObject[cols,rows];
			listObjects.Add(tmpObjects);
			return;
		}

		tmpObjects = listObjects[currentBoard - 1];
		for (int i=0; i<cols; i++) 
		{
			for (int j=0; j<rows; j++)
			{
				if(tmpObjects[i,j] != null)
				{
					EnemyGridObject ego = tmpObjects[i,j];
					ego.gameObject.SetActive(true);
				}
			}
		}
	}

	private void saveData()
	{
		saveCurrentBoard(false);
		DataProvider.getInstance ().SavaData (boardData);
	}

	public void goBackToMain()
	{
		Application.LoadLevel("dragMoveScene");
	}

	public void ClearData()
	{		 
		boardData.Clear ();
	}
	
	public void AssignCoord(EnemyGridObject obj, int col, int row)
	{
		EnemyGridObject[,] tmpObjects = listObjects[currentBoard-1];
		if(obj.row>=0 && obj.col >=0)
			tmpObjects[obj.col, obj.row] = null;
		tmpObjects[col, row] = obj;
	}
	public Vector3 coordToPosition(Vector2 coord)
	{
		Vector3 localPosition = new Vector3((coord.x + 0.5f)/cols - 0.5f, (coord.y + 0.6f)/rows - 0.6f, -1 + coord.y * 0.1f);
		return boxCollider.transform.localToWorldMatrix.MultiplyPoint3x4 (localPosition);
	}
	
	public Vector2 positionToCoord(Vector3 position)
	{
		Vector3 localPosition = boxCollider.transform.worldToLocalMatrix.MultiplyPoint3x4 (position); 
		
		Vector2 coord = new Vector2 (Mathf.FloorToInt ((localPosition.x + 0.5f) * cols), 
		                             Mathf.FloorToInt ((localPosition.y + 0.6f) * rows));

		coord.x = Mathf.Clamp(coord.x, 0, cols - 1);
		coord.y = Mathf.Clamp(coord.y, 0, rows - 1);
		
		return coord;
	}
	
	public bool CoordEmpty(int col, int row)
	{
		if (col != Mathf.Clamp(col, 0, cols - 1))
			return false;
		else if (row != Mathf.Clamp(row, 0, rows - 1))
			return false;


		return listObjects[currentBoard-1][col, row] == null;
	}

	private void previousBoard()
	{
		if (currentBoard == 1) return;

		saveCurrentBoard();

		currentBoard -= 1;

		applyDataToBoard();
	}

	private void nextBoard()
	{
		saveCurrentBoard();
		
		currentBoard += 1;
		
		applyDataToBoard();
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (Screen.width/2-100,  50, 140, 60), "Previous"))
		{
			previousBoard();
		}

		GUI.Label(new Rect(Screen.width/2+50, 80, 60, 60), "Board:" + currentBoard.ToString());

		if (GUI.Button (new Rect (Screen.width/2 + 200,  50, 140, 60), "Next"))
		{
			nextBoard();
		}

		if (GUI.Button (new Rect (Screen.width/2 - 100, Screen.height - 100, 140, 60), "Save"))
		{
			this.saveData();
		}

		if (GUI.Button (new Rect (Screen.width / 2 + 200, Screen.height - 100, 140, 60), "Back")) 
		{
			this.goBackToMain();
		}
	}

}
