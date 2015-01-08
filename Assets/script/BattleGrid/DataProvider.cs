using UnityEngine;
using System.Collections;
using System.IO;
using Parse;

public class DataProvider
{
	private static DataProvider _instance = new DataProvider();
	private string fileName = "boardData.json";
	private DataProvider()
	{
	}
	public static DataProvider getInstance()
	{
		return _instance;
	}
	public void SavaData(object obj)
	{
		string data = MiniJSON.Json.Serialize (obj);
#if !UNITY_IOS && !UNITY_ANDROID
		SaveContents (data);
#endif

		// save the data to Parse
		ParseQuery<ParseObject> query = ParseObject.GetQuery("BoardDataCount");
		query.FirstAsync().ContinueWith(t =>
		{
			ParseObject boardDataCountObj = t.Result;
			int boardDataCount = boardDataCountObj.Get<int>("count");
			boardDataCountObj["count"] = ++boardDataCount;
			boardDataCountObj.SaveAsync();

			ParseObject boardDataObj = new ParseObject("BoardData");
			boardDataObj["data"] = data;
			boardDataObj.SaveAsync();
		});
	}
	public object loadData()
	{
		string data = ReadContents ();
		object obj = MiniJSON.Json.Deserialize (data);
		return obj;
	}
	private string ReadContents()
	{
		//string ret;
		TextAsset bindata= Resources.Load("boardData") as TextAsset;
		return bindata.text;
		/*
		using(StreamReader sr = new StreamReader(fileName))
		{
			ret = sr.ReadToEnd();
			sr.Close();
		}
		
		return ret;
		*/
	}
	
	private void SaveContents(string text)
	{
		using(StreamWriter sw = new StreamWriter(fileName))
		{
//			Debug.Log("text to write is: " + text);
			sw.Write(text);
			sw.Close();
		}
	}  
}

