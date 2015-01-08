using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour
{
	public float fadeSpeed = 2f;          // Speed that the screen fades to and from black.
	public float cameraMoveDistance = 1.5f;
	public GameObject[] backgrounds;
	private float cameraMoveDelta;
	private bool sceneStarting = false;      // Whether or not the scene is still fading in.
	private bool sceneEnding = false;
	private Camera myCamera;
	private GUITexture myGUITexture;
	private int backGroundIdx = 0;
	private GameObject currentBackground;

	void Awake ()
	{
		myGUITexture = GetComponent<GUITexture> ();
		// Set the texture so that it is the the size of the screen and covers it.
		myGUITexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		myCamera = Camera.main;
		cameraMoveDelta = cameraMoveDistance / fadeSpeed;

	}
	// Use this for initialization
	void Start ()
	{

	}
	
	void Update ()
	{
		// If the scene is starting...
		if(sceneStarting)
			// ... call the StartScene function.
			StartSceneTransition();
		else if(sceneEnding)
			EndSceneTransition ();
	}
	

	private void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		myGUITexture.color = Color.Lerp(myGUITexture.color, Color.clear, fadeSpeed * Time.deltaTime);
		myCamera.transform.position +=new Vector3 (cameraMoveDelta* Time.deltaTime,0f,0f);
	}
	
	
	private void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		myGUITexture.color = Color.Lerp(myGUITexture.color, Color.black, fadeSpeed * Time.deltaTime);
		myCamera.transform.position +=new Vector3 (cameraMoveDelta* Time.deltaTime,0f,0f);
	}
	
	
	private void StartSceneTransition()
	{
		// Fade the texture to clear.

		FadeToClear();
		
		// If the texture is almost clear...
		if(Mathf.FloorToInt(myCamera.transform.position.x)==0)
		{
			// ... set the colour to clear and disable the GUITexture.
			myGUITexture.color = Color.clear;
			myGUITexture.enabled = false;
			
			// The scene is no longer starting.
			sceneStarting = false;
//			BattleGrid.instance.sceneTransitionFinished();
		}
	}
	
	public void StartScene()
	{
		myCamera.transform.position=new Vector3 (-1*cameraMoveDistance,-2.4f,19f);
		backGroundIdx = Random.Range (0, backgrounds.Length);
		currentBackground = backgrounds[backGroundIdx];
		for (int i=0; i<backgrounds.Length;i++)
		{
			if(i!=backGroundIdx)
				backgrounds[i].SetActive (false);
			else 
				backgrounds[i].SetActive (true);
		}
		sceneStarting = true;
	}
	public void EndScene()
	{
		// Make sure the texture is enabled.
		myGUITexture.enabled = true;
		sceneEnding = true;
	}
	private void EndSceneTransition ()
	{
		// Start fading towards black.
		FadeToBlack();
		
		// If the screen is almost black...
		if (myGUITexture.color.a >= 0.95f)
		{
			changeBackground();
			sceneEnding = false;
			StartScene();
		}
	}
	private void changeBackground()
	{
		backGroundIdx++;
		backGroundIdx = backGroundIdx > backgrounds.Length - 1 ? 0 : backGroundIdx;

		currentBackground.SetActive (false);
		currentBackground = backgrounds [backGroundIdx];
		currentBackground.SetActive (true);
	}

}

