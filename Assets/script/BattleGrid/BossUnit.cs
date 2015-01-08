using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossUnit : Unit
{
	public float actionPeriod = 10000.0f;
	public List<string> actionSkills;
	public List<string> actionSpawnUnits;

	void Start()
	{
		BattleGrid.StartGame += onGameStarted;
	}

	private IEnumerator startAction()
	{
		while (true && isAlive) 
		{
			yield return new WaitForSeconds(actionPeriod);
			castAction();
		}
	}

	private void castAction()
	{
		float probability = UnityEngine.Random.Range(0.0f, 1.0f);
		if (probability < 0.3f) 
		{
			if (actionSkills.Count > 0)
			{
				int idx = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, (float)actionSkills.Count));
				SkillConfiguration skillConfiguration = GameObject.Find("SkillDatabase").GetComponent<SkillConfiguration>();
				if (skillConfiguration != null) 
				{
					foreach (SkillConfiguration.SkillData data in skillConfiguration.availableSkills)
					{
						if (data._name == actionSkills[idx])
						{
//							GetComponent<SkillController>().CastSkill(data, this.target.gameObject.transform.position);
							break;
						}
					}
				}
			}
		} 
		else if (probability < 0.5f) 
		{
			BattleGrid.instance.bindByBoss();
		} 
		else
		{
			if (actionSpawnUnits.Count > 0)
			{
				int idx = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, (float)actionSpawnUnits.Count));
				BattleGrid.instance.spawnEnemyUnit(actionSpawnUnits[idx]);
			}
		}
	}

	private void onGameStarted()
	{
		StartCoroutine(startAction());
	}
}
