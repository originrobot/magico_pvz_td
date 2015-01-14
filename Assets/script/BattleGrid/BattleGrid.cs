using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class BattleGrid : MonoBehaviour {
	public static BattleGrid instance 
	{ 
		get 
		{ 
			GameObject battleGridObj = GameObject.FindGameObjectWithTag("BattleGrid");
			if (battleGridObj != null) 
			{
				return battleGridObj.GetComponent<BattleGrid>();
			}
			else 
			{
				return null;
			}
		} 
	}
	public delegate void BattleGrideDelegate();
	public static event BattleGrideDelegate StartGame;
	public static event BattleGrideDelegate PauseGame;
	public int cols;
	public int rows;
	public const float defaultManaGenSpeed = 2.5f;
	public float manaRegenSpeed = defaultManaGenSpeed;
	public const float defaultManaGenUnit = 1.0f;
	public float manaRegenUnit = defaultManaGenUnit;
	public bool allowAddUnits=true;
	public BoxCollider boxCollider;
	public Material mat; 
	public Vector2 cellSize { get; private set; }

	public delegate void UpdateManaDelegate(float currentMana);
	public static event UpdateManaDelegate notifyUpdateMana;

	public List<Unit> heroes = new List<Unit>();
	public List<Unit> enemies = new List<Unit>();
//	public PMFSMCommunicator communicator;
	public List<Unit>[,] objects;
	private List<bool> artifactAlives;

	internal int heroTeamFrontLine = 0;
	internal int enemyTeamFrontLine = 0;

	private const int MARCH_LEFT = -1;
	private const int MARCH_RIGHT = 1;

	public bool gameStarted = false;
	private Dictionary<string,object> boardData;
	private List<List<object>> waves = new List<List<object>>();
	private List<object> currentWave;
	private Dictionary<string,object> heroBoardData = new Dictionary<string, object>();
	private List<object> currentWaveHeroData = null;
	private int currentWaveIdx = -1;
	private TileMap tileMap;
	private int displayRange=-1;
	private GameObject unitToAdd = null;
//	private GameObject heroesContainer;
//	private GameObject defeatText;
	private int colLock = -1;
	private string[] defeatMSG = new string[]{"Huh...Loser!","You are defeated!","Is this the best you can do?","U SUCK!!!","GG NOOB!","UR ass is being kicked!"};
	private string[] winMSG = new string[]{"Yeah!","You Win!","Not bad newbie!","Dominating!!!","Good Job!","Unbeatable!"};
	private string[] availableUnits = new string[]{"unit1","unit2","unit3","unit4","unit5","unit6","unit11","unit12","unit13","unit14"
		,"unit15","unit16","unit21","unit22","unit23","unit24","unit25","unit26","bossUnit1"};
	private Dictionary<string,GameObject> unitPrefabs = new Dictionary<string,GameObject >();

	//Skill Buttons -- quick hack, should be moving to other scripts
	private int skillButtonCount = 4;
	private List<GameObject> skillButtons = new List<GameObject>();
	private GameObject startButton;
//	private GameObject dropdownList;
	private GameObject restartButton;
//	private GameObject timerBar;
	private GameObject manaBar;
//	private GameObject labelStageWave;
	private float currentMana;
	public float maxMana = 100f;
	public int timerFontSize = 30;
	public int timerYOffset = 30;
	public float HPRestorePerWave = 0.3f;
	private bool inputEnabled = true;
	private Vector3 manaScreenPosition;
	private Vector3 timeScreenPosition;
//	private int totalTime = 0;
//	private int timer=0;
	private int heroCount;
	private int enemyCount;
//	private SceneController sceneController;
//	private GameObject storedHeroes;
	public SkillButtonControler tappedButtonCtrl = null;
//	private List<Vector3> tileScreenPositions = new List<Vector3>();
	private GameObject currentSelectedUnit;
	public float refundFactor = 0.5f;
	private GameObject enemyManaGenArtifact;
	private GameObject enemyWarlordArtifact;
	private GameObject enemyCDBoostArtifact;
	private GameObject manaGenArtifact;
	private GameObject warlordArtifact;
	private GameObject CDBoostArtifact;
	public float secondWaveCheckingPoint = 60;
	public float thirdWaveCheckingPoint = 120;
	public List<string> firstWaveUnits;
	public List<string> secondWaveUnits;
	public List<string> thirdWaveUnits;
	public float firstWaveSpwanSpeedMin = 0f;
	public float firstWaveSpwanSpeedMax = 0f;
	public float secondWaveSpwanSpeedMin = 0f;
	public float secondWaveSpwanSpeedMax = 0f;
	public float thirdWaveSpwanSpeedMin = 0f;
	public float thirdWaveSpwanSpeedMax = 0f;
	private float currentTime = 0;

	// public cooldown
	public const float defaultPublicCDTime = 6.0f;
	private float _publicCDTime = defaultPublicCDTime;
	public float publicCDTime
	{
		get { return _publicCDTime; }
		set { _publicCDTime = value; }
	}

	private float publicCDStartTime;
	private bool _publicCoolingdown = false;
	public bool publicCoolingdown
	{
		get { return _publicCoolingdown; }
	}

	void Awake () {
		gameObject.tag = "BattleGrid";
		Init ();
	}

	void OnValidate()
	{
		Init ();
	}

	public void Init()
	{		 

		cellSize = new Vector2 (boxCollider.transform.localScale.x * boxCollider.size.x / cols, 
		                        boxCollider.transform.localScale.y * boxCollider.size.y / rows);

		mat.mainTextureScale = new Vector2 (cols, rows);
		enemyManaGenArtifact = GameObject.Find ("enemyManaGenArtifact");
		enemyWarlordArtifact = GameObject.Find ("enemyWarlordArtifact");
		enemyCDBoostArtifact = GameObject.Find ("enemyCDBoostArtifact");
		manaGenArtifact = GameObject.Find ("manaGenArtifact");
		warlordArtifact = GameObject.Find ("warlordArtifact");
		CDBoostArtifact = GameObject.Find ("CDBoostArtifact");

//		GameObject btnDropdown = GameObject.Find("btnDropdown");
//		dropdownList = btnDropdown.transform.FindChild("dropdownList").gameObject;
	}

	void Start()
	{

		tileMap = GameObject.Find ("TileMap").GetComponent<TileMap>();
		foreach (string str in availableUnits) 
		{
			GameObject unit_prefab = (GameObject)Resources.Load(str+"Prefab");
			unitPrefabs[str] = unit_prefab;
		}
		objects = new List<Unit>[1, 3];
		for (int i=0; i<3; i++) 
		{
			objects[0,i] = new List<Unit>();
		}
		artifactAlives = new List<bool>();
		for (int ii = 0; ii < 3; ++ii)
		{
			artifactAlives.Add(true);
		}

		initButtons ();
		startGame ();
		currentMana = 0.0f;
		updateManaBar ();

		// start public CD coroutine
		publicCDStartTime = Time.realtimeSinceStartup - publicCDTime - 1.0f;
		StartCoroutine(coPublicCD());
	}

	private IEnumerator coPublicCD()
	{
		while (true)
		{
			if (Time.realtimeSinceStartup - publicCDStartTime > publicCDTime)
			{
				_publicCoolingdown = false;
			}
			else
			{
				_publicCoolingdown = true;
			}
			yield return null;
		}
	}

	private void initWaves()
	{


	}

	public object getTeamDetails()
	{
		List<object> teamDetails = new List<object>();

		for (int ii = 0; ii < skillButtonCount; ++ii) 
		{
			Dictionary<string,object> skillDict = new Dictionary<string,object>();
			skillDict["id"] = skillButtons[ii].name;
			skillDict["unitid"] = skillButtons[ii].GetComponent<SkillButtonControler>().unitId;
			Transform unitIcon = skillButtons[ii].transform.FindChild("UnitIcon");
			skillDict["uniticon"] = unitIcon.GetComponent<SpriteRenderer>().sprite.name;
			teamDetails.Add(skillDict);
		}

		return teamDetails;
	}

	public void applyTeamDetail(object teamDetail)
	{
		List<object> teamDetails = teamDetail as List<object>;
		for (int ii = 0; ii < skillButtonCount; ++ii) 
		{
			Dictionary<string,object> skillDict = teamDetails[ii] as Dictionary<string,object>;
			Sprite unitSprite = Resources.Load<Sprite>("Buttons/" + skillDict["uniticon"].ToString());
			if (unitSprite != null)
			{
				Transform unitIcon = skillButtons[ii].transform.FindChild("UnitIcon");
				unitIcon.GetComponent<SpriteRenderer>().sprite = unitSprite;
			}

			skillButtons[ii].GetComponent<SkillButtonControler>().unitId = skillDict["unitid"].ToString();
		}
	}



	private GameObject currentSelectedSkillBtn = null;
	public void onSkillButtonClick(GameObject skillButton)
	{
		if (skillButton != null) 
		{
			currentSelectedSkillBtn = skillButton;
		}
	}

	private void initButtons()
	{
		for (int ii = 1; ii <= skillButtonCount; ++ii) 
		{
			GameObject skillButton = GameObject.Find("SkillButton" + ii.ToString());
			UIEventListener.Get(skillButton).onClick += onSkillButtonClick;
			skillButtons.Add(skillButton);
		}

		startButton = GameObject.Find ("startButton");
		startButton.SetActive(false);
		restartButton = GameObject.Find ("restartButton");
//		timerBar = GameObject.Find ("TimerBar");
//		timerBar.transform.parent.gameObject.SetActive (false);
		manaBar = GameObject.Find ("ManaBar");
		restartButton.SetActive(false);

		enemyManaGenArtifact.GetComponent<Unit> ().enemy = true;
		enemyWarlordArtifact.GetComponent<Unit> ().enemy = true;
		enemyCDBoostArtifact.GetComponent<Unit> ().enemy = true;
		manaGenArtifact.GetComponent<Unit> ().enemy = false;
		warlordArtifact.GetComponent<Unit> ().enemy = false;
		CDBoostArtifact.GetComponent<Unit> ().enemy = false;

		enemyManaGenArtifact.GetComponent<GridObject> ().AssignCoord (0, 2);
		enemyWarlordArtifact.GetComponent<GridObject> ().AssignCoord (0, 1);
		enemyCDBoostArtifact.GetComponent<GridObject> ().AssignCoord (0, 0);
		manaGenArtifact.GetComponent<GridObject> ().AssignCoord (0, 2);
		warlordArtifact.GetComponent<GridObject> ().AssignCoord (0, 1);
		CDBoostArtifact.GetComponent<GridObject> ().AssignCoord (0, 0);
	}


	public void bindByBoss()
	{
		List<GameObject> tmpSkills = new List<GameObject>(skillButtons);
		int count = 0;
		while (count < 3)
		{
			int idx = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, (float)tmpSkills.Count));
			tmpSkills[idx].GetComponent<SkillButtonControler>().BindByBoss();
			tmpSkills.RemoveAt(idx);
			++count;
		}
	}
	IEnumerator startTimer()
	{
		currentTime = 0f;
		while(true)
		{
			yield return new WaitForSeconds(1f);
			currentTime += 1f;
		}
	}
	IEnumerator spawnEnemy()
	{
		float spawnSpeed = 1000;
		float spawnSpeedMin = 0;
		float spawnSpeedMax = 1000;
		List<string> unitList;
		while(true)
		{
			if(gameStarted)
			{
				if(currentTime<secondWaveCheckingPoint)
				{
					spawnSpeedMin = firstWaveSpwanSpeedMin;
					spawnSpeedMax = firstWaveSpwanSpeedMax+1f;
					unitList = firstWaveUnits;
				}
				else if(currentTime>=secondWaveCheckingPoint&&currentTime<thirdWaveCheckingPoint)
				{
					spawnSpeedMin = secondWaveSpwanSpeedMin;
					spawnSpeedMax = secondWaveSpwanSpeedMax+1f;
					unitList = secondWaveUnits;
				}
				else
				{
					spawnSpeedMin = thirdWaveSpwanSpeedMin;
					spawnSpeedMax = thirdWaveSpwanSpeedMax+1f;
					unitList = thirdWaveUnits;
				}
				spawnSpeed = Random.Range(spawnSpeedMin,spawnSpeedMax);
				int r = Random.Range(0,unitList.Count);
				yield return new WaitForSeconds(spawnSpeed);
				spawnEnemyUnit(unitList[r]);
			}
			yield return null;
		}
	}
	public void spawnEnemyUnit(string prefabName)
	{
		List<int> rows = new List<int>();
		for (int ii = 0; ii < 3; ++ii)
		{
			rows.Add(ii);
		}

		for (int ii = 0; ii < 3; ++ii) 
		{
			if (artifactAlives[ii] == false) rows.Remove(ii);
		}

		if (rows.Count == 0) return;
		int idx = Random.Range(0, rows.Count);
		int row = rows[idx];

		GameObject go = GameObject.Instantiate(unitPrefabs[prefabName]);
		go.transform.parent = transform;
		int attackRange = go.GetComponent<UnitBase>().getRange();
		go.GetComponent<GridObject>().AssignCoord(0, row);
		go.transform.position = coordToPosition (new Vector2(0.4f,row));
		objects[0,row].Add (go.GetComponent<Unit>());
		Unit unit = go.GetComponent<Unit>();
		if(unit != null)
		{
			unit.enemy = true;
			unit.name = unit.character_id;
			unit.character_id = unit.character_id+System.DateTime.Now.Millisecond;
			unit.attackRange = unit.gameObject.GetComponent<UnitBase>().getRange();
			enemies.Add(unit);
			unit.BeginMovement();
		}
	}

	private void getEmptyCoord(ref int col, ref int row, int advance)
	{
		bool found = false;
		while (!found)
		{
			if (col < enemyTeamFrontLine) break;

			for (int ii = 0; ii < rows; ++ii)
			{
				if (CoordEmpty(col, ii))
				{
					row = ii;
					found = true;
					break;
				}
			}

			if (!found)
			{
				col += advance;
			}
		} 
	}

	public void ResetEnemyBoard()
	{
		currentWaveIdx = -1;
		startGame();
	}



	private void startGame()
	{
		enemies.Clear ();
		heroes.Clear ();
		currentMana = 0.0f;
		enemyManaGenArtifact.SetActive (true);
		enemyWarlordArtifact.SetActive (true);
		enemyCDBoostArtifact.SetActive (true);
		manaGenArtifact.SetActive (true);
		warlordArtifact.SetActive (true);
		CDBoostArtifact.SetActive (true);

		enemyManaGenArtifact.GetComponent<Unit>().initUnit ();
		enemyWarlordArtifact.GetComponent<Unit>().initUnit ();
		enemyCDBoostArtifact.GetComponent<Unit>().initUnit ();
		manaGenArtifact.GetComponent<Unit>().initUnit ();
		warlordArtifact.GetComponent<Unit>().initUnit ();
		CDBoostArtifact.GetComponent<Unit>().initUnit ();

		enemies.Add (enemyManaGenArtifact.GetComponent<Unit> ());
		enemies.Add (enemyWarlordArtifact.GetComponent<Unit> ());
		enemies.Add (enemyCDBoostArtifact.GetComponent<Unit> ());
		heroes.Add (manaGenArtifact.GetComponent<Unit> ());
		heroes.Add (warlordArtifact.GetComponent<Unit> ());
		heroes.Add (CDBoostArtifact.GetComponent<Unit> ());

		objects [0, 2].Add (enemyManaGenArtifact.GetComponent<Unit> ());
		objects [0, 1].Add (enemyWarlordArtifact.GetComponent<Unit> ());
		objects [0, 0].Add (enemyCDBoostArtifact.GetComponent<Unit> ());
		objects [0, 2].Add (manaGenArtifact.GetComponent<Unit> ());
		objects [0, 1].Add (warlordArtifact.GetComponent<Unit> ());
		objects [0, 0].Add (CDBoostArtifact.GetComponent<Unit> ());

		if (manaBar != null)
		{
			manaBar.GetComponent<EnergyBar> ().SetValueMax((int)maxMana);
			updateManaBar ();
		}
		restartButton.SetActive (false);
		startButton.SetActive (true);

	}
	void Update()
	{
		if (!gameStarted) return;
//		if (!enemyWarlordArtifact.activeSelf)
//		{
//			gameOver(true);
//			return;
//		}
//		if(!warlordArtifact.activeSelf)
//		{
//			gameOver (false);
//			return;
//		}
	}

	private void gameOver(bool winning)
	{
		StopCoroutine ("spawnEnemy");
		StopCoroutine ("generateMana");
		StopCoroutine ("startTimer");
		restartButton.SetActive (true);
	}

	public List<Unit> getUnitByRow(int row, bool isEnemy)
	{
//		Debug.Log ("getUnitByRow-- row" + row+",isEnemy="+isEnemy);
		List<Unit> units = new List<Unit>();
		if (row >= rows || row < 0) return null; 
		foreach(Unit u in objects [0, row])
		{
//			Debug.Log ("getUnitByRow-- u.enemy=" + u.enemy);
			if(u.enemy != isEnemy)
				units.Add (u);
		}
		return units;
	}
	public void removeUnit(int row,Unit unit)
	{
		objects[0,row].Remove(unit);
	}

	public Vector3 coordToPosition(Vector2 coord)
	{
		float yOffsets = -0.45f;
		yOffsets += Random.Range (-0.05f,0.05f);
		Vector3 localPosition = new Vector3(coord.x, coord.y/rows+yOffsets, 0f);
		return boxCollider.transform.localToWorldMatrix.MultiplyPoint (localPosition);
	}

	public Vector2 positionToCoord(Vector3 position)
	{
		Debug.Log ("positionToCoord"+position.x + ":"+position.y);
		Vector3 localPosition = boxCollider.transform.worldToLocalMatrix.MultiplyPoint (position); 
		Debug.Log ("positionToCoord"+localPosition.x + ":"+localPosition.y);
		Vector2 coord = new Vector2 (0, 
		                             Mathf.FloorToInt ((localPosition.y/3.6f+0.5f) * rows));

		return coord;
	}

	public bool CoordEmpty(int col, int row)
	{
		if (col != Mathf.Clamp(col, 0, cols - 1))
			return false;
		else if (row != Mathf.Clamp(row, 0, rows - 1))
			return false;

		return objects [col, row]==null;
	}

	// temp for playmaker
	public void OnStartButtonClicked()
	{
		startButton.SetActive (false);
		StartCoroutine ("startTimer");
		StartCoroutine ("spawnEnemy");
		StartCoroutine ("generateMana");
		StartGame ();
		gameStarted = true;
	}
	public void OnRestartButtonClicked()
	{
		startGame();
	}


	void updateManaBar()
	{
		if (manaBar == null) return;

		float relativeMana = currentMana / maxMana;
		manaBar.GetComponent<EnergyBar>().SetValueF(relativeMana);

		notifyUpdateMana(currentMana);
	}
	public int getMana()
	{
		return Mathf.FloorToInt(currentMana);
	}

	public void updateMana()
	{
		currentMana = Mathf.FloorToInt(currentMana + manaRegenUnit) > maxMana ? maxMana : currentMana + manaRegenUnit;
		updateManaBar();
	}

	public void ModifyMana(float delta)
	{
		currentMana += delta;
		currentMana = Mathf.Clamp(currentMana, 0.0f, maxMana);
		updateManaBar();
	}

	IEnumerator generateMana()
	{
		while(true)
		{
			updateMana();
			yield return new WaitForSeconds(manaRegenSpeed);
		}
	}

	public void toggleEnemySelection(Unit unit)
	{
		foreach (Unit h in enemies) 
		{
			if (h.tapSelected && unit.tapSelected && h != unit)
			{
				h.tapSelected = false;
			}
		}

		foreach (Unit h in heroes)
		{
			h.toggleEnemySelection();
		}
	}

	public void unitSelected(GameObject go)
	{
		if(!inputEnabled) return;
		currentSelectedUnit = null;

		if(go)
		{
			Unit unit = go.GetComponent<Unit>();
			if(unit.enemy == false)
			   currentSelectedUnit = go;
		}
	}

	public void onArtifactDestroyed(int row, bool warlord = false, bool enemy = false)
	{
		if (row >= artifactAlives.Count||artifactAlives[row]==false) return;
		artifactAlives [row] = false;

		// kill all unit in the row
		foreach(Unit u in objects[0, row])
		{

			UnitBase unitV0 = u.GetComponent<UnitBase>();
			unitV0.setHP(0);
		}

		objects[0, row].Clear();

		// warlord destroyed
		if (warlord == true) 
		{
			for (int ii = 0; ii < 3; ++ii)
			{
				foreach(Unit u in objects[0, ii])
				{
					UnitBase unitV0 = u.GetComponent<UnitBase>();
					unitV0.setHP(0);
				}

				objects[0, ii].Clear();
			}

			StartCoroutine(coWarlordDead(enemy));
		}
	}

	private IEnumerator coWarlordDead(bool enemy)
	{
		int count = 0;
		while (count < 2) 
		{
			yield return null;
			++count;
		}
		
		bool win = false;
		if (enemy) win = true;
		gameOver(win);
	}

	public void OnTap(TapGesture gesture)
	{
		if (tappedButtonCtrl == null)
		{
			return;
		}

		Vector3 worldPosition = new Vector3(gesture.Position.x, gesture.Position.y, transform.position.z);
		worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
		Vector2 coord = positionToCoord(worldPosition);
		coord += new Vector2 (-0.4f,0f);

		if (artifactAlives[(int)coord.y] == false) return;

		Vector3 localPosition = coordToPosition(coord);

		if (!unitPrefabs.ContainsKey(tappedButtonCtrl.unitId)) 
		{
			Debug.LogError("can't find prefab for: " + tappedButtonCtrl.unitId);
			tappedButtonCtrl = null;
			return;
		}
		
		unitToAdd = GameObject.Instantiate(unitPrefabs[tappedButtonCtrl.unitId]);
		unitToAdd.name = tappedButtonCtrl.unitId+"hero";
		unitToAdd.GetComponent<Unit>().enemy=false;
		unitToAdd.transform.parent = transform;
		unitToAdd.transform.position = localPosition;
		Debug.Log ("on tap: "+(int)coord.x+":"+(int)coord.y);
		unitToAdd.GetComponent<GridObject>().AssignCoord((int)coord.x,(int)coord.y);
		objects[0,(int)coord.y].Add (unitToAdd.GetComponent<Unit>());
		Unit unit = unitToAdd.GetComponent<Unit>();
		if(unit != null)
		{
			unit.character_id = "unit"+System.DateTime.Now.Millisecond;
			unit.attackRange = unit.gameObject.GetComponent<UnitBase>().getRange();
			unit.cost = (float)tappedButtonCtrl.getCost();
			heroes.Add(unit);
			if(gameStarted)
				unit.BeginMovement();
		}
		
		currentMana-=(float)tappedButtonCtrl.getCost();
		updateManaBar();
		unitToAdd = null;

		// start public CD after deploying hero units
		publicCDStartTime = Time.realtimeSinceStartup;
		
		tappedButtonCtrl.startCooldown();
		tappedButtonCtrl = null;
	}

	public void doTap(TapGesture gesture, SkillButtonControler sbc)
	{
		if(tappedButtonCtrl != null)
			tappedButtonCtrl.setState (SkillButtonControler.NOTREADYSTATE);
		tappedButtonCtrl = null;
		if (!allowAddUnits) return;
		if (!inputEnabled) 
		{
			if(unitToAdd != null)
			{
				Destroy (unitToAdd);
				unitToAdd = null;
			}
			return;
		}
		sbc.setState (SkillButtonControler.SELECTEDSTATE);
		tappedButtonCtrl = sbc;
	}
}
