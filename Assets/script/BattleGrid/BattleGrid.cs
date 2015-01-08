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
	public float manaRegenSpeed = 1f;
	public float manaRegenUnit = 0.2f;
	public bool allowAddUnits=true;
	public BoxCollider boxCollider;
	public Material mat; 
	public Vector2 cellSize { get; private set; }

	public List<Unit> heroes = new List<Unit>();
	public List<Unit> enemies = new List<Unit>();
//	public PMFSMCommunicator communicator;
	public List<Unit>[,] objects;

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
	private TileSelection ts;
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
	private List<GameObject> skillButtons = new List<GameObject>();
	private GameObject startButton;
//	private GameObject dropdownList;
	private GameObject restartButton;
//	private GameObject timerBar;
	private GameObject manaBar;
//	private GameObject labelStageWave;
	private float AP;
	public float max_ap = 100f;
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
	private float storedMana;
//	private GameObject storedHeroes;
	public SkillButtonControler tappedButtonCtrl = null;
//	private List<Vector3> tileScreenPositions = new List<Vector3>();
	private GameObject currentSelectedUnit;
	public float refundFactor = 0.5f;
	private GameObject manaDropPrefab;
	void Awake () {
		Application.targetFrameRate = 30;
		gameObject.tag = "BattleGrid";
		Init ();
	}

	void OnValidate()
	{
		Init ();
	}

	public void Init()
	{		 
		heroes.Clear ();

		cellSize = new Vector2 (boxCollider.transform.localScale.x * boxCollider.size.x / cols, 
		                        boxCollider.transform.localScale.y * boxCollider.size.y / rows);

		mat.mainTextureScale = new Vector2 (cols, rows);

//		GameObject btnDropdown = GameObject.Find("btnDropdown");
//		dropdownList = btnDropdown.transform.FindChild("dropdownList").gameObject;
	}

	void Start()
	{
//		sceneController = GameObject.Find("GameScene").GetComponent<SceneController> ();
		tileMap = GameObject.Find ("TileMap").GetComponent<TileMap>();
		ts = tileMap.GetComponent<TileSelection> ();
//		defeatText = GameObject.Find ("DefeatText");
//		if (defeatText != null) defeatText.GetComponent<HUDText>().Add("Try to survive 3 wave of enemies...",Color.white,2);
//		initWaves ();
		initButtons ();
		startGame ();
		AP = max_ap;
		storedMana = max_ap;
		updateManaBar ();
//		resolveTileScreenPositions();
	}

//	private void resolveTileScreenPositions()
//	{
//		for (int ii = 0; ii < tileMap.TileCenters.Count; ++ii) 
//		{
//			Vector3 screenPos = Camera.main.WorldToScreenPoint(tileMap.TileCenters[ii]);
//			tileScreenPositions.Add(screenPos);
//		}
//	}

	private void initWaves()
	{


	}

	public object getTeamDetails()
	{
		List<object> teamDetails = new List<object>();

		for (int ii = 0; ii < 6; ++ii) 
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
		for (int ii = 0; ii < 6; ++ii) 
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
		for (int ii = 1; ii <= 6; ++ii) 
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
//		labelStageWave = GameObject.Find("labelStageWave");



//		GameObject manaDropObject = Resources.Load ("ManaDropPrefab") as GameObject;
//		manaDropPrefab = GameObject.Instantiate(manaDropObject,manaDropObject.transform.position,manaDropObject.transform.rotation) as GameObject;
//		manaDropPrefab.transform.parent = gameObject.transform;
//		manaDropPrefab.transform.position = new Vector3 (0f,2f,25f);
//		manaDropPrefab.SetActive(false);
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

	public void spawnEnemyUnit(string prefabName)
	{
		GameObject go = GameObject.Instantiate(unitPrefabs[prefabName]);
		go.transform.parent = transform;
		int attackRange = go.GetComponent<UnitBase>().getRange();
		int col = enemyTeamFrontLine + (attackRange-1);
		int row = -1;
		getEmptyCoord(ref col, ref row, -1);
		if (row != -1) 
		{
			go.GetComponent<GridObject>().AssignCoord(col, row);
			objects[0,row].Add (go.GetComponent<Unit>());
			Unit unit = go.GetComponent<Unit>();
			if(unit != null)
			{
				unit.enemy = true;
				unit.name = unit.character_id;
				unit.character_id = unit.character_id+System.DateTime.Now.Millisecond;
				unit.attackRange = unit.gameObject.GetComponent<UnitBase>().getRange();
				enemies.Add(unit);
				enemyTeamFrontLine = enemyTeamFrontLine<col+cols/2?enemyTeamFrontLine:col+cols/2;
				unit.BeginMovement();
			}
		}
		else
		{
			Destroy(go);
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

		AP = storedMana;
		if (manaBar != null)
		{
			manaBar.GetComponent<EnergyBar> ().SetValueMax((int)max_ap);
			updateManaBar ();
		}
		restartButton.SetActive (false);
		startButton.SetActive (true);
	}
	void Update()
	{
		if (!gameStarted) return;
		if (enemies.Count == 0)
		{
			gameOver(true);
			return;
		}
		if(heroes.Count == 0)
		{
			gameOver (false);
			return;
		}


	}

	private void gameOver(bool winning)
	{
		restartButton.SetActive (true);
	}
	public List<Unit> getUnitByRow(int row)
	{
	
		List<Unit> units = null;
		if (row < rows && row>=0) 
			units = objects [0, row];
		return units;
	}
	public void removeUnit(int row,Unit unit)
	{
		objects[0,row].Remove(unit);
	}

	public Vector3 coordToPosition(Vector2 coord)
	{
		Vector3 localPosition = new Vector3((coord.x +0.5f)/cols-0.5f, (coord.y +0.6f)/rows-0.6f, -0.5f + coord.y * 0.1f);
		return boxCollider.transform.localToWorldMatrix.MultiplyPoint (localPosition);
	}

	public Vector2 positionToCoord(Vector3 position)
	{
		Vector3 localPosition = boxCollider.transform.worldToLocalMatrix.MultiplyPoint (position); 
//		Debug.Log ("positionToCoord"+localPosition.x + ":"+localPosition.y);
		Vector2 coord = new Vector2 (Mathf.FloorToInt ((localPosition.x +0.5f) * cols), 
		                             Mathf.FloorToInt ((localPosition.y +0.6f) * rows));

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

		float relativeMana = AP / max_ap;
		manaBar.GetComponent<EnergyBar>().SetValueF(relativeMana);
	}
	public int getAP()
	{
		return Mathf.FloorToInt(AP);
	}
	public void addAP()
	{
		AP = Mathf.FloorToInt(AP + manaRegenUnit) > max_ap ? max_ap : AP + manaRegenUnit;
		updateManaBar ();
	}

	IEnumerator generateMana()
	{
		while(true)
		{
			addAP();
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
		ts.UnSelectUnit ();
		currentSelectedUnit = null;

		if(go)
		{
			Unit unit = go.GetComponent<Unit>();
			if(unit.enemy == false)
			   currentSelectedUnit = go;
			ts.SelectUnit (go.transform.position, unit.enemy, unit.attackRange);
		}
	}


	public void addManaFromDrop(float addMana)
	{
		AP = AP+(float)addMana>max_ap?max_ap:AP+(float)addMana;
		updateManaBar();
	}
	public void OnTap(TapGesture gesture)
	{
		if (tappedButtonCtrl == null)
		{
			ts.UnSelectUnit();
			return;
		}

		ts.HideDeployableQuads();

		Vector3 worldPosition = new Vector3(gesture.Position.x, gesture.Position.y, transform.position.z);
		worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
		Vector2 coord = positionToCoord(worldPosition + new Vector3(0,-0.5f,0f));
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
		unitToAdd.GetComponent<GridObject>().AssignCoord((int)coord.x,(int)coord.y);
		objects[0,(int)coord.y].Add (unitToAdd.GetComponent<Unit>());
		Unit unit = unitToAdd.GetComponent<Unit>();
		if(unit != null)
		{
			unit.character_id = "unit"+System.DateTime.Now.Millisecond;
			unit.attackRange = unit.gameObject.GetComponent<UnitBase>().getRange();
			unit.cost = (float)tappedButtonCtrl.getCost();
			heroes.Add(unit);
			unit.BeginMovement();
		}
		
		AP-=(float)tappedButtonCtrl.getCost();
		updateManaBar ();
		unitToAdd = null;
		
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
