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
}
