using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManaGenUnit : Unit
{
	public List<float> manaGenSpeed;

	// Use this for initialization
	void Start () 
	{
		if (!enemy && manaGenSpeed.Count > 0) 
		{
			BattleGrid.instance.manaRegenSpeed = manaGenSpeed[currentLevel-1];
		}
	}

	protected override void onLevelChanged()
	{
		if (!enemy && manaGenSpeed.Count >= currentLevel) 
		{
			BattleGrid.instance.manaRegenSpeed = manaGenSpeed[currentLevel-1];
		}
	}

	void OnDestroy()
	{
		if (!enemy && BattleGrid.instance) 
		{
			BattleGrid.instance.manaRegenSpeed = BattleGrid.defaultManaGenSpeed;
		}
	}

	protected override void onDestroyUnit()
	{
		if (BattleGrid.instance == null) return;
		gameObject.SetActive (false);
		if (!enemy) 
		{
			BattleGrid.instance.manaRegenSpeed = BattleGrid.defaultManaGenSpeed;
		}

		BattleGrid.instance.onArtifactDestroyed(GetComponent<GridObject>().row,false,enemy);
	}
}
