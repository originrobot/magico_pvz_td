using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class UnloadableParseObj : MonoBehaviour {

	private List<object> boardObjects = new List<object>();
	private bool bBoardDataDownloaded = false;
	private int currentBoardIdx = 0;

	private List<object> teamObjects = new List<object>();
	private bool bTeamObjectDownloaded = false;
	private int currentTeamIdx = 0;

	void Awake() 
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	public void LoadBoardDataAuthorScene()
	{
		Application.LoadLevel("FileWriterScene");
	}

	public void SelectNextBoard()
	{
		object boardData = GetNextBoardData();
		if (boardData == null) return;
		BattleGrid battleGrid = GameObject.Find("BattleGrid").GetComponent<BattleGrid>();
//		battleGrid.initWavesFromBoardData(boardData);
		battleGrid.ResetEnemyBoard();
	}

	private bool bShowTeamEditPanel = false;
	private GameObject teamEditPanel = null;
	private GameObject objUIRoot = null;
	private GameObject teamEditSVCtrlWidget = null;
	public void ToggleTeamEditPanel()
	{
		if (objUIRoot == null) 
		{
			objUIRoot = GameObject.Find("UI Root");
		}

		if (teamEditPanel == null) 
		{
			teamEditPanel = objUIRoot.transform.FindChild("teamEditPanel").gameObject;
		}

		if (teamEditSVCtrlWidget == null) 
		{
			teamEditSVCtrlWidget = objUIRoot.transform.FindChild("teamEditSVCtrlWidget").gameObject;
		}

		if (teamEditPanel != null && teamEditSVCtrlWidget != null) 
		{
			bShowTeamEditPanel = !bShowTeamEditPanel;
			teamEditPanel.SetActive(bShowTeamEditPanel);
			teamEditSVCtrlWidget.SetActive(bShowTeamEditPanel);
		}
	}

	private BattleGrid battleGrid = null;
	public void SaveCurrentTeam()
	{
		if (battleGrid == null) 
		{
			battleGrid = GameObject.Find("BattleGrid").GetComponent<BattleGrid>();
		}

		object teamDetails = battleGrid.getTeamDetails();
		string data = MiniJSON.Json.Serialize(teamDetails);
		ParseObject teamDetail = new ParseObject("PlayerTeam");
		teamDetail["data"] = data;
		teamDetail.SaveAsync();
	}

	public void SelectNextTeam()
	{
		if (bTeamObjectDownloaded == false || teamObjects.Count == 0) return;

		if (battleGrid == null) 
		{
			battleGrid = GameObject.Find("BattleGrid").GetComponent<BattleGrid>();
		}
		object result = teamObjects[currentTeamIdx++];
		battleGrid.applyTeamDetail(result);
		
		if (currentTeamIdx >= teamObjects.Count) currentTeamIdx = 0;
	}

	void Start()
	{
		GetBoardDataFromParse();
		GetTeamDataFromParse();
	}

	private void GetTeamDataFromParse()
	{
		bTeamObjectDownloaded = false;
		teamObjects.Clear();

		ParseQuery<ParseObject> query = ParseObject.GetQuery("PlayerTeam");
		query.FindAsync().ContinueWith(t =>
		{
			IEnumerable<ParseObject> datas = t.Result;
			foreach (ParseObject teamDetail in datas)
			{
				string data = teamDetail.Get<string>("data");
				object obj = MiniJSON.Json.Deserialize(data);
				teamObjects.Add(obj);
			}
			
			bTeamObjectDownloaded = true;
		});
	}

	private void GetBoardDataFromParse()
	{
		bBoardDataDownloaded = false;
		boardObjects.Clear();
		ParseQuery<ParseObject> query = ParseObject.GetQuery("BoardData");
		query.FindAsync().ContinueWith(t =>
		{
			IEnumerable<ParseObject> boardDatas = t.Result;
			foreach (ParseObject boardData in boardDatas)
			{
				string data = boardData.Get<string>("data");
				object obj = MiniJSON.Json.Deserialize(data);
				boardObjects.Add(obj);
			}

			query = ParseObject.GetQuery("HeroBoardData");
			query.FindAsync().ContinueWith(tHero =>
			{
				boardDatas = tHero.Result;
				foreach (ParseObject boardData in boardDatas)
				{
					string data = boardData.Get<string>("data");
					object obj = MiniJSON.Json.Deserialize(data);
					boardObjects.Add(obj);
				}

				//Debug.LogError("Both enemy and hero board data downloaded!");
				bBoardDataDownloaded = true;
			});
		});
	}

	public object GetNextBoardData()
	{
		if (bBoardDataDownloaded == false || boardObjects.Count == 0) return null;
		object result = boardObjects[currentBoardIdx++];

		if (currentBoardIdx >= boardObjects.Count) currentBoardIdx = 0;

		return result;
	}
}
