using UnityEngine;
using System.Collections;

public class WarlordUnit : Unit
{

	// Use this for initialization
	void Start () {
	
	}
	
	protected override void onDestroyUnit()
	{
		if (BattleGrid.instance == null) return;
		gameObject.SetActive (false);
		BattleGrid.instance.onArtifactDestroyed(GetComponent<GridObject>().row, true, enemy);
	}
}
