using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CDBoostUnit : Unit
{
	public List<float> cooldownTimes;

	// Use this for initialization
	void Start () 
	{
		if (!enemy && cooldownTimes.Count > 0) 
		{
			BattleGrid.instance.publicCDTime = cooldownTimes[currentLevel-1];
		}
	}

	protected override void onLevelChanged()
	{
		if (!enemy && cooldownTimes.Count >= currentLevel) 
		{
			BattleGrid.instance.publicCDTime = cooldownTimes[currentLevel-1];
		}
	}

	void OnDestroy()
	{
		if (!enemy && BattleGrid.instance) 
		{
			BattleGrid.instance.publicCDTime = BattleGrid.defaultPublicCDTime;
		}
	}

	protected override void onDestroyUnit()
	{
		if (BattleGrid.instance == null) return;
		gameObject.SetActive (false);
		if (!enemy) 
		{
			BattleGrid.instance.publicCDTime = BattleGrid.defaultPublicCDTime;
		}
		
		BattleGrid.instance.onArtifactDestroyed(GetComponent<GridObject>().row);
	}
}
