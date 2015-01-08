using UnityEngine;
using System.Collections;

public class DropMana : MonoBehaviour
{
	public float addMana=5f;
	public float endpointY = -7f;
	public bool isDropping = false;
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	public void startDropping()
	{
		StartCoroutine ("drop");
		isDropping = true;
	}
	IEnumerator drop()
	{
		while (transform.position.y>endpointY) 
		{
			transform.position+=new Vector3(0f,-0.01f,0f);
			yield return new WaitForSeconds(0.02f);
		}
		transform.position = new Vector3 (0f,2f,25f);
		isDropping = false;
		gameObject.SetActive (false);
	}
	void OnTap(TapGesture gesture)
	{
		if(gesture.Selection.Equals(gameObject))
		{
			BattleGrid.instance.addManaFromDrop (addMana);
			StopCoroutine("drop");
			transform.position = new Vector3 (0f,2f,25f);
			isDropping = false;
			gameObject.SetActive(false);
		}
	}
}

