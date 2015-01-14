using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour
{
	private Transform HPbar;
	private Transform HPCache;
	private float currentValue; 
	// Use this for initialization
	void Awake()
	{
		HPbar = transform.FindChild ("HPbar");
		HPCache = transform.FindChild ("HPcache");
	}
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	public Vector3 getSize()
	{
		return HPbar.GetComponent<Renderer> ().bounds.size;
	}
	public void SetValueF(float value)
	{
		currentValue = HPbar.localScale.x;
		HPbar.localScale = new Vector3 (value,1f,1f);
		if(currentValue>value)
			StartCoroutine("animateHPBar",value);
	}
	IEnumerator animateHPBar(float newValue)
	{
		while(currentValue>newValue)
		{
			currentValue = currentValue - 0.01f;
			HPCache.localScale = new Vector3 (currentValue,1f,1f);
			yield return new WaitForSeconds(0.1f);
		}
	}
}

