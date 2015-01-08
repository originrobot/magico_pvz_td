using UnityEngine;
using System.Collections;

public class EnemyGrideObjectFixator : MonoBehaviour
{
	public EnemyBoard enemyBoard;
	// Use this for initialization
	void Start ()
	{

	}


	void Update ()
	{
		Vector2 coord = enemyBoard.positionToCoord (transform.position);
		coord.x = Mathf.Clamp(coord.x, 0, enemyBoard.cols - 1);
		coord.y = Mathf.Clamp(coord.y, 0, enemyBoard.rows - 1);
		this.transform.position = enemyBoard.coordToPosition (coord);
	}
}

