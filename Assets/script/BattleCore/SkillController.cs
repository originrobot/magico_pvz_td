using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillController : MonoBehaviour {

	public Vector3 skillOffset = Vector3.zero;
	private GameObject skillObj = null;
	public bool isAttackSkill = true;
	public float freezeDutation = 3f;

	void Start ()
	{
	}

//	public void CastSkill(SkillConfiguration.SkillData skillData, Vector3 pos)
//	{
//		if (skillData._type == "columndamage") {
//			StartCoroutine (CastColumnDamageSkill(skillData, pos));
//			return;
//		}
//		
//		if (skillData._type == "columnheal") {
//			StartCoroutine (CastColumnHealSkill(skillData, pos));
//			return;
//		}
//		
//		if (skillData._type == "columnfreeze") {
//			StartCoroutine (CastColumnFreezeSkill(skillData, pos));
//			return;
//		}
//	}
//
//	private void finishColumnSkill(float x)
//	{
//		Destroy(skillObj);
//		Resources.UnloadUnusedAssets();
//
//		doColumnDamage(x);
//	}
//
//	private void doColumnDamage(float x)
//	{
//		float currentDMG = Random.Range(200.0f, 400.0f);
//		for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//		{
//			Unit target = BattleGrid.instance.getUnitByCoord(y, (int)x);
//			if(target && target.isAlive)
//			{
//				UnitBase targetVO = target.GetComponent<UnitBase>();
//				float newHP = targetVO.getHP () - currentDMG;
//				newHP = newHP<0?0:newHP;
//				targetVO.setHP(newHP);
//				target.OnHit (currentDMG,false);
//			}
//		}
//
//	}
//
//	private IEnumerator CastColumnDamageSkill(SkillConfiguration.SkillData skillData, Vector3 pos)
//	{
//		Vector2 posCoord = BattleGrid.instance.positionToCoord(pos);
//		List<GameObject> skillList = new List<GameObject>();
//		List<GameObject> availableSkillList = new List<GameObject>();
//		GameObject skillPrefab = (GameObject)Resources.Load(skillData._prefab);
//		int loopCount = skillData._loopCount;
//		if (loopCount == 0) loopCount = 1;
//		for (int ii = 0; ii < loopCount; ++ii) 
//		{
//			for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//			{
//				Unit target = BattleGrid.instance.getUnitByCoord(y, (int)posCoord.x);
//				if(target && target.isAlive)
//				{
//					GameObject newSkill = null;
//					if (availableSkillList.Count > 0)
//					{
//						newSkill = availableSkillList[0];
//						availableSkillList.RemoveAt(0);
//					}
//					else
//					{
//						newSkill = GameObject.Instantiate(skillPrefab) as GameObject;
//					}
//					Debug.Log("casting spell: target is " + target);
//					Debug.Log("casting spell: newAkill is " + newSkill);
//					Debug.Log("casting spell: newSkill.transform.parent is " + newSkill.transform.parent);
////					newSkill.transform.parent = target.transform;
//					newSkill.transform.position = target.transform.position + skillOffset;
//					newSkill.SetActive(true);
//					newSkill.layer = 7;//skills layer
//					newSkill.GetComponent<Renderer> ().sortingLayerName = "skills";
//					skillList.Add(newSkill);
//				}
//			}
//			
//			yield return new WaitForSeconds(skillData._duration);
//			
//			
//			for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//			{
//				Unit target = BattleGrid.instance.getUnitByCoord(y, (int)posCoord.x);
//				if(target && target.isAlive)
//				{
//					UnitBase targetVO = target.GetComponent<UnitBase>();
//					float skillDamage = DualCalculator.getInstance().getDamage(skillData, targetVO);
//					float newHP = targetVO.getHP () - skillDamage;
//					newHP = newHP<0?0:newHP;
//					targetVO.setHP(newHP);
//					target.OnHit (skillDamage,false);
//				}
//			}
//
//			foreach (GameObject skill in skillList) {
//				skill.SetActive(false);
//				availableSkillList.Add(skill);
//			}
//
//			skillList.Clear();
//
//			yield return new WaitForSeconds(skillData._timeBetweenLoop);
//		}
//
//		foreach (GameObject skill in availableSkillList) {
//			Destroy(skill);
//		}
//
//		Resources.UnloadUnusedAssets();
//	}
//
//	private IEnumerator CastColumnHealSkill(SkillConfiguration.SkillData skillData, Vector3 pos)
//	{
//		Vector2 posCoord = BattleGrid.instance.positionToCoord(pos);
//		List<GameObject> skillList = new List<GameObject>();
//		GameObject skillPrefab = (GameObject)Resources.Load(skillData._prefab);
//		for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//		{
//			Unit target = BattleGrid.instance.getUnitByCoord(y, (int)posCoord.x);
//			if(target && target.isAlive)
//			{
//				GameObject newSkill = GameObject.Instantiate(skillPrefab) as GameObject;
//				newSkill.transform.parent = target.transform;
//				newSkill.transform.position = target.transform.position + skillOffset;
//				newSkill.SetActive(true);
//				newSkill.layer = 7;//skills layer
//				newSkill.GetComponent<Renderer> ().sortingLayerName = "skills";
//				skillList.Add(newSkill);
//			}
//		}
//		
//		yield return new WaitForSeconds(2.0f);
//		
//		
//		for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//		{
//			Unit target = BattleGrid.instance.getUnitByCoord(y, (int)posCoord.x);
//			if(target && target.isAlive)
//			{
//				UnitBase targetVO = target.GetComponent<UnitBase>();
//				float skillHealing = DualCalculator.getInstance().getHeal(skillData, targetVO);
//				float newHP = targetVO.getHP () + skillHealing;
//				newHP = newHP>targetVO.getMaxHP()?targetVO.getMaxHP():newHP;
//				targetVO.setHP(newHP);
//				target.OnHeal (skillHealing);
//			}
//		}
//		
//		foreach (GameObject skill in skillList) {
//			Destroy(skill);
//		}
//		
//		Resources.UnloadUnusedAssets();
//	}
//
//	private IEnumerator CastColumnFreezeSkill(SkillConfiguration.SkillData skillData, Vector3 pos)
//	{
//		Vector2 posCoord = BattleGrid.instance.positionToCoord(pos);
//		List<GameObject> skillList = new List<GameObject>();
//		GameObject skillPrefab = (GameObject)Resources.Load(skillData._prefab);
//		for (int y = 0; y < BattleGrid.instance.rows; ++y) 
//		{
//			Unit target = BattleGrid.instance.getUnitByCoord(y, (int)posCoord.x);
//			if(target && target.isAlive)
//			{
//				GameObject newSkill = GameObject.Instantiate(skillPrefab) as GameObject;
//				newSkill.transform.parent = target.transform;
//				newSkill.transform.position = target.transform.position + skillOffset;
//				newSkill.SetActive(true);
//				newSkill.layer = 7;//skills layer
//				newSkill.GetComponent<Renderer> ().sortingLayerName = "skills";
//				skillList.Add(newSkill);
//				target.frozeUnit(freezeDutation);
//			}
//		}
//		
//		yield return new WaitForSeconds(3.0f);
//		
//		foreach (GameObject skill in skillList) {
//			Destroy(skill);
//		}
//		
//		Resources.UnloadUnusedAssets();
//	}	
}
