using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
	
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 4 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = Color.green;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		if (fps>30 && fps<45)
			style.normal.textColor = Color.yellow;
		else if (fps < 30)
			style.normal.textColor = Color.red;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}