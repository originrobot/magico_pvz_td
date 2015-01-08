using UnityEngine;
using System.Collections;

public class SkillEffectController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

	}
	void OnEnded()
	{
		transform.parent.gameObject.GetComponent<Unit> ().OnEndEffect ();//todo: refactor it!
	}
	// Update is called once per frame
	void Update ()
	{

	}
}

