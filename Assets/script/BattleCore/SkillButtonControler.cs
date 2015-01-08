using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillButtonControler : MonoBehaviour
{
	public const int NOTREADYSTATE=0;
	public const int READYSTATE=1;
	public const int COOLDOWNSTATE = 2;
	public const int DISABLEDSTATE = 3;
	public const int SELECTEDSTATE = 4;
	public bool isSkill = false;
	public bool applyToEnemy = true;
	private int currentState=NOTREADYSTATE;
	public float cooldownTime = 0.0f;
	public bool rowOnly;
	public bool colOnly;
	private SkillConfiguration.SkillData _skillData = null;
	private Dictionary<string,Sprite> numberDic = new Dictionary<string, Sprite>();
	private int APCost=0;
	private int _addToTimer;
	private bool bindByBoss = false;
	public int addToTimer
	{
		get{return _addToTimer;}
	}
	public string defaultUnitId;
	private string _unitId;
	public string unitId
	{
		get{ return _unitId;}
		set
		{
			_unitId = value;
			int cost = UnitCost.instance.getUnitCost(_unitId);
			_addToTimer = UnitCost.instance.getUnitAddtoTimer(_unitId);
			APCost = cost;
			transform.FindChild("cost1").gameObject.GetComponent<SpriteRenderer>().sprite = numberDic[(APCost/10).ToString()];
			transform.FindChild("cost0").gameObject.GetComponent<SpriteRenderer>().sprite = numberDic[(APCost%10).ToString()];
			if (APCost < 10)
				transform.FindChild ("cost1").gameObject.SetActive (false);
		}
	}
	public SkillConfiguration.SkillData skillData
	{
		get { return _skillData; }
		internal set { _skillData = value; }
	}

	void Awake()
	{
		Sprite[] numbers = Resources.LoadAll<Sprite>("numbers");
		numberDic["0"]=numbers[0];
		numberDic["1"]=numbers[1];
		numberDic["2"]=numbers[2];
		numberDic["3"]=numbers[3];
		numberDic["4"]=numbers[4];
		numberDic["5"]=numbers[5];
		numberDic["6"]=numbers[6];
		numberDic["7"]=numbers[7];
		numberDic["8"]=numbers[8];
		numberDic["9"]=numbers[9];
	}

	// Use this for initialization
	void Start ()
	{
		unitId = defaultUnitId;
		if (isSkill) 
		{
			currentState=DISABLEDSTATE;
			SkillConfiguration skillConfiguration = GameObject.Find("SkillDatabase").GetComponent<SkillConfiguration>();
			if (skillConfiguration != null) 
			{
				foreach (SkillConfiguration.SkillData data in skillConfiguration.availableSkills)
				{
					if (data._name == _unitId)
					{
						skillData = data;
						break;
					}
				}
				
				if (skillData != null)
				{
					// set UnitIcon according to the skill configuration
					Sprite skillSprite = Resources.Load<Sprite>(skillData._sprite);
					if (skillSprite != null)
					{
						transform.FindChild("UnitIcon").GetComponent<SpriteRenderer>().sprite = skillSprite;
					}
				}
			}
		}
		gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (false);
	}

	public void BindByBoss()
	{
		StartCoroutine(CoBindByBoss());
	}

	private IEnumerator CoBindByBoss()
	{
		bindByBoss = true;
		yield return new WaitForSeconds(5.0f);
		bindByBoss = false;
	}

	void Update()
	{
		if(currentState == COOLDOWNSTATE || currentState == DISABLEDSTATE||currentState==SELECTEDSTATE) return;
		if (BattleGrid.instance.getAP () >= APCost && bindByBoss == false)
			setState (READYSTATE);
		else
			setState (NOTREADYSTATE);
		
	}

	void OnEnable()
	{
		BattleGrid.PauseGame += onGamePaused;
		BattleGrid.StartGame += onGameStarted;
	}
	
	void OnDisable()
	{
		BattleGrid.PauseGame -= onGamePaused;
		BattleGrid.StartGame -= onGameStarted;
	}
	private void onGameStarted()
	{
		if (isSkill)
			setState (NOTREADYSTATE);
	}
	private void onGamePaused()
	{
		if (isSkill)
			setState (DISABLEDSTATE);
		if (currentState == SELECTEDSTATE)
			setState (NOTREADYSTATE);
	}



	public void OnTap(TapGesture gesture)
	{
		Debug.Log ("SkillButtonController onTap: selection is : " + gesture.Selection);
		if (currentState != READYSTATE||gesture.StartSelection != gameObject)
		{
			return;
		}

		BattleGrid.instance.doTap(gesture, this);
	}

	public void setState(int newState)
	{
		int oldState = currentState;
		currentState = newState;
		if (oldState != newState)
			changeState ();
	}

	private void changeState()
	{
		if (currentState == NOTREADYSTATE)
		{
			gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (false);
			gameObject.transform.FindChild ("herodetail").gameObject.SetActive (true);
		}
		else if(currentState == READYSTATE)
		{
			gameObject.transform.FindChild ("herodetail").gameObject.SetActive (false);
			gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (true);
		}
		else if (currentState == DISABLEDSTATE)
		{
			gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (false);
			gameObject.transform.FindChild ("herodetail").gameObject.SetActive (true);
		}
		else if(currentState == SELECTEDSTATE)
		{
			gameObject.transform.FindChild ("herodetail").gameObject.SetActive (false);
			gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (false);
		}
		else if (currentState == COOLDOWNSTATE)
		{
			gameObject.transform.FindChild ("ReadyMask").gameObject.SetActive (false);
			gameObject.transform.FindChild ("herodetail").gameObject.SetActive (true);
		}
	}

	public void startCooldown()
	{
		StartCoroutine(coolingDown());
	}

	private IEnumerator coolingDown()
	{
		if (cooldownTime > 0.0f) 
		{
			setState(COOLDOWNSTATE);

			float currentTime = Time.realtimeSinceStartup;
			while (currentTime + cooldownTime > Time.realtimeSinceStartup)
			{
				yield return new WaitForSeconds(0.1f);
			}

			setState(NOTREADYSTATE);
		}
	}
	public int getState()
	{
		return currentState;
	}
	public int getCost()
	{
		return this.APCost;
	}
}

