using UnityEngine;
using System.Collections;
[RequireComponent(typeof(BoxCollider))]

public class EnemyGridObject : MonoBehaviour
{
	public int row=-1;
	public int col=-1;
	public string id;
	public string description;
	public EnemyBoard enemyBoard;
	protected Vector2 destinationCoord;
	public bool isTemplate = true;
	private Vector3 startPos = Vector3.zero;
	void OnDrag(DragGesture gesture) 
	{
		if (gesture.StartSelection != gameObject) {
			return;
		}

		if (gesture.State == GestureRecognitionState.InProgress)
		{	
			Vector3 worldPosition = new Vector3(gesture.Position.x, gesture.Position.y, 0);
			worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
			Vector2 coord = enemyBoard.positionToCoord(worldPosition+new Vector3(0,-0.5f,0f));
			if(enemyBoard.CoordEmpty((int)coord.x,(int)coord.y))
			{
				Vector3 localPosition = enemyBoard.coordToPosition(coord);
				this.gameObject.transform.position = localPosition;
			}
		}

		if (gesture.State == GestureRecognitionState.Ended)
		{	
			Vector3 worldPosition = new Vector3(gesture.Position.x, gesture.Position.y, 0);
			worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
			Vector2 coord = enemyBoard.positionToCoord(worldPosition+new Vector3(0,-0.5f,0f));
			if(!enemyBoard.CoordEmpty((int)coord.x,(int)coord.y))
			{
				Vector3 localPosition = transform.position;
				coord = enemyBoard.positionToCoord(enemyBoard.boxCollider.transform.localToWorldMatrix.MultiplyPoint3x4(localPosition)+new Vector3(0,-0.5f,0f));
			}
			if (isTemplate)
			{
				// clone an unit from 'this' and assign to enemyboard
				GameObject clone = GameObject.Instantiate(gameObject);	
				clone.GetComponent<EnemyGridObject>().isTemplate = false;
				clone.GetComponent<EnemyGridObject>().id = gameObject.name;
				clone.GetComponent<EnemyGridObject>().name = gameObject.name;
				clone.transform.position = transform.position;
				
				enemyBoard.AssignCoord(clone.GetComponent<EnemyGridObject>(),(int)coord.x,(int)coord.y);
				clone.GetComponent<EnemyGridObject>().col = (int)coord.x;
				clone.GetComponent<EnemyGridObject>().row = (int)coord.y;
				
				// put 'this' back to its original position
				transform.position = startPos;
			}
			else
			{
				enemyBoard.AssignCoord(this,(int)coord.x,(int)coord.y);
				col = (int)coord.x;
				row = (int)coord.y;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		if (!enemyBoard)
			enemyBoard = GameObject.Find ("EnemyBoard").GetComponent<EnemyBoard>();

		startPos = transform.position;
	}

	// Update is called once per frame
	void Update ()
	{

	}
}

