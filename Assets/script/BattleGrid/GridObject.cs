using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GridObjectFixator))]
public class GridObject : MonoBehaviour
{
	public int row;
	public int col;

	public void AssignCoord(int col, int row)
	{
//		Debug.Log(this.gameObject+"AssignCoord col: "+col+"row :"+row);
//		BattleGrid.instance.objects [this.col, this.row]=null;

		this.row = row;
		this.col = col;
		
//		BattleGrid.instance.objects [this.col, this.row]=this;

//		this.transform.position = BattleGrid.instance.coordToPosition (new Vector2 (col, row));
	}

}

