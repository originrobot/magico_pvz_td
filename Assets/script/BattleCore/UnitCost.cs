using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UnitCost : MonoBehaviour
{

	public int unit1Cost;
	public int unit2Cost;
	public int unit3Cost;
	public int unit4Cost;
	public int unit5Cost;
	public int unit6Cost;
	public int skillFireCost;
	public int skillHealCost;
	public int unit1Timer;
	public int unit2Timer;
	public int unit3Timer;
	public int unit4Timer;
	public int unit5Timer;
	public int unit6Timer;
	public int skillFireTimer;
	public int skillHealTimer;
	private Dictionary<string,int> costDic = new Dictionary<string, int>();
	private Dictionary<string,int> timerDic = new Dictionary<string, int>();
	// Use this for initialization
	public static UnitCost instance
	{
		get
		{
			GameObject obj = GameObject.FindGameObjectWithTag("UnitCost");
			if (obj != null) 
			{
				return obj.GetComponent<UnitCost>();
			}
			else 
			{
				return null;
			}
		}
	}
	void Awake()
	{
		costDic ["unit1"] = unit1Cost;
		costDic ["unit2"] = unit2Cost;
		costDic ["unit3"] = unit3Cost;
		costDic ["unit4"] = unit4Cost;
		costDic ["unit5"] = unit5Cost;
		costDic ["unit6"] = unit6Cost;
		costDic ["unit11"] = unit1Cost;
		costDic ["unit12"] = unit2Cost;
		costDic ["unit13"] = unit3Cost;
		costDic ["unit14"] = unit4Cost;
		costDic ["unit15"] = unit5Cost;
		costDic ["unit16"] = unit6Cost;
		costDic ["unit21"] = unit1Cost;
		costDic ["unit22"] = unit2Cost;
		costDic ["unit23"] = unit3Cost;
		costDic ["unit24"] = unit4Cost;
		costDic ["unit25"] = unit5Cost;
		costDic ["unit26"] = unit6Cost;
		costDic ["aoe_fireball"] = skillFireCost;
		costDic ["aoe_heal"] = skillHealCost;
		timerDic ["unit1"] = unit1Timer;
		timerDic ["unit2"] = unit2Timer;
		timerDic ["unit3"] = unit3Timer;
		timerDic ["unit4"] = unit4Timer;
		timerDic ["unit5"] = unit5Timer;
		timerDic ["unit6"] = unit6Timer;
		timerDic ["unit11"] = unit1Timer;
		timerDic ["unit12"] = unit2Timer;
		timerDic ["unit13"] = unit3Timer;
		timerDic ["unit14"] = unit4Timer;
		timerDic ["unit15"] = unit5Timer;
		timerDic ["unit16"] = unit6Timer;
		timerDic ["unit21"] = unit1Timer;
		timerDic ["unit22"] = unit2Timer;
		timerDic ["unit23"] = unit3Timer;
		timerDic ["unit24"] = unit4Timer;
		timerDic ["unit25"] = unit5Timer;
		timerDic ["unit26"] = unit6Timer;
		timerDic ["aoe_fireball"] = skillFireTimer;
		timerDic ["aoe_heal"] = skillHealTimer;
	}
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	public int getUnitCost(string unitId)
	{
		Debug.Log ("getUnitCost: unitId=" + unitId);
		return costDic[unitId];
	}
	public int getUnitAddtoTimer(string unitId)
	{
		return timerDic[unitId];
	}
}

