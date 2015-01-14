using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	protected float speed=0f;
//	public LineRenderer pathLine;
	private bool isEnemy = true;
	public bool isLeader = false;
	private bool isTapSelected = false;
//	private PMFSMCommunicator playMakerFSMCommunicator;
	public string character_id;
	protected GridObject gridObject;
	protected Vector2 destinationCoord;

	protected Unit target;
	protected float stopDelay = 0;
	private float max_hp;
	private Transform hitpoints;

	private GameObject _spark;
	private GameObject _upgradeArrow;
	private bool _displaySpark = false;
	private int _sparkTimer = 0;
	private const int TIMERLIMIT = 10;

	public bool isStarted = false;
	
	private const int MELEE_RANGE = 1;
	private const int ROGUE_RANGE = 1;
	private const int MAGE_RANGE = 2;
	private const int ARCHER_RANGE = 3;

	private const int IDLE_STATE = 1;
	private const int WALK_STATE = 2;
	private const int ATTACK_STATE = 3;
	private const int DIE_STATE = 4;
	private const int CRITICAL_STATE = 5;
	
	public int attackRange;
	private Animator animator;

	// for skills
	public string[] skillPrefabNames;
	public bool skillRotatable = true;
	private Vector3[] skillOffsets;
	private List<GameObject> availableSkills = new List<GameObject>();
	private int currentSkillIdx = 0;
	private bool isCastingSkill = false;
	protected Vector3 skillOffset = Vector3.zero;
	private GameObject skillObj;
	private float backAngle;
	private UnitBase unitVO;
	private DualCalculator dualCalculator;
	public bool isAlive = true;
	private GameObject HP;
	private GameObject DamageTxT;
	private float maxHP;
	GameObject dmg_prefab;
	public List<Unit> targets;
	public List<Unit> debugTargets;

	private float currentDMG=0;
	private bool isTriggeringCritical=false;
	private bool isMoving = false;
	public bool isFrozen = false;
	private Vector3 initialPosition=Vector3.zero;
	private float initialHP=0f;
	public float cost;
	private bool isAttacking = false;
	private int currentState = 1;
	private float speedFactor = 0.02f;

	// for artifact
	protected int currentLevel = 1;
	public List<int> levelUpCost;

	public bool enemy
	{
		get{return isEnemy;}
		set
		{
			isEnemy = value;
			if (!isEnemy) 
			{
				transform.Rotate (0,180,0);
				HP.transform.Rotate (0,180,0);
				HP.transform.position += new Vector3(-1.1f,0f,0f);
				isCastingSkill = false;
			}
		}
	}

	public bool tapSelected
	{
		get { return isTapSelected; }
		set { isTapSelected = value; }
	}

	public void initUnit ()
	{
		isAlive = true;
		isFrozen = false;
		isMoving = false;
		isTriggeringCritical=false;
		isStarted = false;
		_displaySpark = false;
		gameObject.GetComponent<UnitBase> ().setHP (gameObject.GetComponent<UnitBase> ().getMaxHP());
	}
	void Awake()
	{		
		gridObject = GetComponent<GridObject>();
		_spark = transform.FindChild ("spark").gameObject;
		_spark.GetComponent<Renderer> ().enabled = false;
		
		
		if(skillPrefabNames!=null&&skillPrefabNames.Length>0)
		{
			skillOffsets = new Vector3[]{new Vector3(0f,0.5f,0f)};
			GameObject skill_prefab = (GameObject)Resources.Load(skillPrefabNames[0]);
			skillOffset = skillOffsets[0];
			skillObj = GameObject.Instantiate(skill_prefab) as GameObject;
			skillObj.transform.parent = transform;
			skillObj.transform.position = transform.position+skillOffset;
			skillObj.SetActive(false);
			skillObj.layer = 7;//skills layer
			skillObj.GetComponent<Renderer> ().sortingLayerName = "skills";
			availableSkills.Add(skillObj);
		}
		animator = this.GetComponent<Animator>();
		unitVO = GetComponent<UnitBase> ();
		if(unitVO.unitType==UnitTypes.ARTIFACT)
		{
			_upgradeArrow = transform.FindChild ("upgrade").gameObject;
			_upgradeArrow.GetComponent<Renderer> ().enabled = false;
		}
		dualCalculator = DualCalculator.getInstance();
		HP = transform.FindChild ("HPBar").gameObject;
		dmg_prefab = (GameObject)Resources.Load ("damagePrefab");
		isTapSelected = false;
		speed = unitVO.getMovingSpeed() * speedFactor;
	}

	void Start()
	{
	}



	void OnEnable()
	{
		BattleGrid.PauseGame += StopMovement;
		BattleGrid.StartGame += BeginMovement;
		BattleGrid.notifyUpdateMana += onUpdateMana;
	}

	void OnDisable()
	{
		BattleGrid.PauseGame -= StopMovement;
		BattleGrid.StartGame -= BeginMovement;
		BattleGrid.notifyUpdateMana -= onUpdateMana;
	}
	void onDeath()
	{
		StopAllCoroutines ();
		isAlive = false;
		isStarted = false;
		BattleGrid.instance.removeUnit (gridObject.row,this);
		changeAninationState(DIE_STATE);
	}

	void StopMovement()
	{
		isStarted = false;
		changeAninationState (IDLE_STATE);
		StopCoroutine ("FindTarget");
	}
	private void changeAninationState(int state)
	{
		switch(state)
		{
			case IDLE_STATE:
			animator.SetBool ("idle",true);
			break;
			case WALK_STATE:
			animator.SetBool ("idle",false);
			break;
			case ATTACK_STATE:
			animator.SetTrigger("attack");
			break;
			case DIE_STATE:
			animator.SetTrigger("die");
			break;
			case CRITICAL_STATE:
			animator.SetTrigger("critical");
			break;
		}
		currentState = state;
	}
	public void BeginMovement()
	{
		target = null;
		isStarted = true;
		initialHP = Hp;
		initialPosition = transform.position;
		unitVO.setCoolDownFactor (0);
		isLeader = unitVO.unitType == UnitTypes.ARTIFACT || unitVO.unitType == UnitTypes.WARLOARD;
		if(!isMoving&&unitVO.unitType!=UnitTypes.ARTIFACT)
			StartCoroutine ("FindTarget");
	}
	public void frozeUnit (float duration)
	{
		isFrozen = true;
		animator.speed = 0f;
		StartCoroutine ("unFrozeUnit",duration);
	}
	IEnumerator unFrozeUnit(float duration)
	{
		yield return new WaitForSeconds (duration);
		isFrozen = false;
		animator.speed = 1f;
	}
	public void OnHit(float damage,bool isCritical)
	{
//		Debug.Log ("OnHit unitId: " +unitId +", character_id: "+character_id);

		GameObject damageObj = Instantiate (dmg_prefab);
		damageObj.transform.parent = transform;
		damageObj.transform.position = transform.position + new Vector3 (-0.2f,2f,0);
//		DamageTxT.GetComponent<HUDText> ().Add (damage, Color.red, 0f);
		this._displaySpark = true;
		this._sparkTimer = 0;
		damageObj.GetComponent<DamagePopup>().critical = isCritical;
		damageObj.GetComponent<DamagePopup>().displayType = DamagePopup.DAMAGE;
		damageObj.GetComponent<DamagePopup> ().Value = Mathf.FloorToInt(damage);

	}

	public void OnHeal(float heal)
	{
		GameObject damageObj = Instantiate (dmg_prefab);
		damageObj.transform.parent = transform;
		damageObj.transform.position = transform.position + new Vector3 (-0.2f,2f,0);
//		this._displaySpark = true;
//		this._sparkTimer = 0;
		damageObj.GetComponent<DamagePopup>().displayType = DamagePopup.HEAL;
		damageObj.GetComponent<DamagePopup>().Value = Mathf.FloorToInt(heal);	
	}

	protected void AttackFireSkill()
	{
		GameObject skillObj = availableSkills[currentSkillIdx];
		if (skillObj.activeSelf == true || isCastingSkill == true) return;

		isCastingSkill = true;
		skillObj.SetActive(true);
		positionSkillEffect (skillObj.transform);

	}
	private void endEffect(float angle)
	{
//		Debug.Log ("endEffect --- effect ended");
		backAngle = angle * -1;
		if(skillRotatable)
			skillObj.transform.Rotate (0,0,backAngle);
		skillObj.transform.position = transform.position + skillOffset;
		skillObj.SetActive(false);
		isCastingSkill = false; 
		doDamage();
//		_fsm.SendEvent ("attacking");
	}
	public void OnEndEffect()
	{

	}
	private void positionSkillEffect(Transform trans)
	{
		if (target == null) 
		{
			trans.gameObject.SetActive(false);
			isCastingSkill = false; 	
			return;
		}
		Transform targetTrans = target.transform;
		Vector3 relative = trans.InverseTransformPoint(targetTrans.position+new Vector3(0f,0.5f,0f));
		float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
//		Debug.Log ("positionSkillEffect -- reletive: " + angle);
		angle += 180;
		if(skillRotatable)
		{
			trans.Rotate(0, 0, angle);
		}
		double duration = Mathf.Sqrt (relative.y * relative.y + relative.x * relative.x)*0.1;
		iTween.MoveTo (trans.gameObject, iTween.Hash ("position",targetTrans.position+new Vector3(0f,0.5f,0f),
		                                              "time",duration,"looptype","none",
		                                              "easetype", iTween.EaseType.linear,
		                                              "onCompleteTarget", this.gameObject,
		                                              "oncomplete","endEffect",
		                                              "oncompleteparams",angle));
	}

	public void updateHP()
	{
		float relativeHP = Hp / unitVO.getMaxHP ();
		HP.GetComponent<HPBar> ().SetValueF (relativeHP);
	}
	
	void Update()
	{
		if(!isAlive) return;
		if (_displaySpark)
		{
//			Debug.Log(this+"maxhp:" + unitVO.getMaxHP());
			_spark.GetComponent<Renderer> ().enabled = true;

			_sparkTimer++;
		}

		if (_sparkTimer >= TIMERLIMIT)
		{
			_spark.GetComponent<Renderer> ().enabled = false;
			_displaySpark = false;
		}
		if (Hp<=0)
		{
			onDeath();
		}
	}

	public float Hp
	{
		get
		{
			float hp = unitVO.getHP();
			hp = hp>=0?hp:0;
			return hp;
		}
	}
	public int getRefundPoints(GameObject storedHeroes, bool gameStarted)
	{
		bool justDeployed = true;
		for (int i=0; i< storedHeroes.transform.childCount; i++)
		{
			if (storedHeroes.transform.GetChild(i).GetComponent<Unit>() == this)
			{
				justDeployed = false;
				break;
			}
		}

		int refund = (int)cost;
		if (justDeployed == false || gameStarted == true) refund = Mathf.FloorToInt (cost * Hp / unitVO.getMaxHP ()*BattleGrid.instance.refundFactor);
		GameObject damageObj = Instantiate (dmg_prefab);
		damageObj.transform.parent = transform.parent;
		damageObj.transform.position = transform.position + new Vector3 (-0.2f,2f,0);
		damageObj.GetComponent<DamagePopup>().displayType = DamagePopup.REFUND;
		damageObj.GetComponent<DamagePopup> ().Value = refund;
		return refund;
	}

	void DestoryUnit()
	{
		if (BattleGrid.instance == null) return;

		if (enemy)
		{
			BattleGrid.instance.enemies.Remove (this);
		}
		else 
		{
			BattleGrid.instance.heroes.Remove (this);
		}

		onDestroyUnit();
	}

	protected virtual void onDestroyUnit()
	{
		Destroy (gameObject);
	}

	void OnAttacking()
	{
		if (target == null || target.isAlive == false || !isAlive || isFrozen)
		{
			target = null;
			return;
		}
		if (attackRange>1 && currentSkillIdx < availableSkills.Count ) 
		{
			AttackFireSkill();
			currentSkillIdx = ++currentSkillIdx % availableSkills.Count;
		}
		else
		{
			doDamage();
		}
	}
	void OnCritical()
	{
		Debug.Log (this+"is on critical");
		if (target == null || target.isAlive == false || !isAlive || isFrozen)
		{
			target = null;
			return;
		}
		doDamage();
	}
	IEnumerator FindTarget()
	{
		float cooldown = this.gameObject.GetComponent<UnitBase> ().getCoolDown ();
		List<Unit> enemies;
		targets = new List<Unit> ();
		while (true) 
		{
			enemies = BattleGrid.instance.getUnitByRow(gridObject.row,isEnemy);
			targets.Clear();
			foreach(Unit enemy in enemies)
			{
				if(enemy && enemy.isAlive&&isTargetInRange(enemy))
					targets.Add(enemy);
			}

			if(targets.Count>0 && !isFrozen && isAlive)
			{
				changeAninationState (IDLE_STATE);
				targets.Sort(new TargetComparer(enemy));
				target = targets[0] as Unit;
				if(!isAttacking)
					StartCoroutine("AttackTarget",cooldown);
			}
			else if(targets.Count==0 && enemies.Count>0)
			{
				moveUnit();
			}
			yield return null;
		}

	}
	private void moveUnit()
	{
		if(currentState != WALK_STATE)
			changeAninationState (WALK_STATE);
		float step = isEnemy ? -1*speed : speed;
		transform.position += new Vector3(step,0f,0f);
	}
	private bool isTargetInRange(Unit target)
	{
//		Debug.Log ("isTargetInRange:"+(transform.position.x - target.transform.position.x));
		return Mathf.Abs (transform.position.x - target.transform.position.x) <= attackRange;
	}
	IEnumerator AttackTarget(float cooldown)
	{
		isAttacking = true;
		UnitBase targetVO = target.GetComponent<UnitBase>();
		isTriggeringCritical = dualCalculator.isCritical(unitVO);
		currentDMG = dualCalculator.getDamage(unitVO,targetVO);
		if(isTriggeringCritical)
			currentDMG*=3;
		if(!isTriggeringCritical||attackRange>1)
			changeAninationState (ATTACK_STATE);
		else
		{
			changeAninationState (CRITICAL_STATE);
		}
		yield return new WaitForSeconds(cooldown);
		isAttacking = false;
	}
	private void doDamage()
	{
		if(target && target.isAlive && isAlive&&!isFrozen)
		{
			UnitBase targetVO = target.GetComponent<UnitBase>();
			float newHP = targetVO.getHP () - currentDMG;
			newHP = newHP<0?0:newHP;
			targetVO.setHP(newHP);
			target.OnHit(currentDMG,isTriggeringCritical);
		}
	}
		


	public void toggleEnemySelection()
	{
		if (!isAlive || isMoving || !isStarted) return;

		StopCoroutine("FindTarget");
		StartCoroutine("FindTarget");
	}

	private void OnPause()
	{
		isStarted = false;
		StopCoroutine ("FindTarget");
	}

	public bool isPlayerArtifact()
	{
		return unitVO.unitType == UnitTypes.ARTIFACT && !enemy;
	}

	public void OnTap(TapGesture gesture)
	{
//		Debug.Log (gameObject+"Unit is enemy:" +isEnemy );
//		if (isEnemy && gesture.Selection.Equals (gameObject)) 
//		{
//			isTapSelected = !isTapSelected;
//			BattleGrid.instance.toggleEnemySelection(this);
//		}
//
//		if (BattleGrid.instance.tappedButtonCtrl)
//		{
//			BattleGrid.instance.OnTap (gesture);
//			BattleGrid.instance.unitSelected(null);
//			return;
//		}
//		if(gesture.Selection.Equals(gameObject))
//		{
//			BattleGrid.instance.unitSelected(gameObject);
//		}

		// upgrade artifact
		if (isPlayerArtifact() && currentLevel <= levelUpCost.Count)
		{
			if (BattleGrid.instance.getMana() > levelUpCost[currentLevel-1])
			{
				BattleGrid.instance.ModifyMana(-levelUpCost[currentLevel-1]);
				_upgradeArrow.GetComponent<Renderer>().enabled = false;
				++currentLevel;
				onLevelChanged();
			}
		}
		else
		{
			BattleGrid.instance.OnTap(gesture);
		}
	}

	protected virtual void onLevelChanged()
	{
	}

	private void onUpdateMana(float currentMana)
	{
		if (unitVO.unitType != UnitTypes.ARTIFACT || enemy) return;

		if (_upgradeArrow == null) return;

		if (currentLevel > levelUpCost.Count) 
		{
			_upgradeArrow.SetActive(false);
			return;
		}

		if (currentMana > levelUpCost [currentLevel - 1]) 
		{
			_upgradeArrow.SetActive(true);
			_upgradeArrow.GetComponent<Renderer>().enabled = true;
		} 
		else
		{
			_upgradeArrow.SetActive(false);
			_upgradeArrow.GetComponent<Renderer>().enabled = false;
		}
	}
}

