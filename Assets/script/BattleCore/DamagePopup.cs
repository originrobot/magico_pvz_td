using UnityEngine;
using System.Collections;

public class DamagePopup : MonoBehaviour {
	
	//目标位置
	private Vector3 mTarget;
	//屏幕坐标
	private Vector3 mScreen;
	//伤害数值
	public int Value;
	
	//文本宽度
	public float ContentWidth=100;
	//文本高度
	public float ContentHeight=50;
	
	//GUI坐标
	private Vector2 mPoint;
	
	//销毁时间
	public float FreeTime=2F;

	public const int DAMAGE = 1;
	public const int HEAL = 2;
	public const int REFUND = 3;
	public int displayType = 1;
	public bool critical = false;
	void Start () 
	{
		//获取目标位置
		mTarget=transform.position;
		//获取屏幕坐标
		mScreen= Camera.main.WorldToScreenPoint(mTarget);
		//将屏幕坐标转化为GUI坐标
		mPoint=new Vector2(mScreen.x,Screen.height-mScreen.y);
		//开启自动销毁线程
		StartCoroutine("Free");
	}
	
	void Update()
	{
		//使文本在垂直方向山产生一个偏移
		float delta = Time.deltaTime - 0.01f;
		delta = delta > 0 ? delta : 0f;
		transform.Translate(Vector3.up * 0.5F * delta);
		//重新计算坐标
		mTarget=transform.position;
		//获取屏幕坐标
		mScreen= Camera.main.WorldToScreenPoint(mTarget);
		//将屏幕坐标转化为GUI坐标
		mPoint=new Vector2(mScreen.x,Screen.height-mScreen.y);
	}
	
	void OnGUI()
	{
		//保证目标在摄像机前方
		if(mScreen.z>0)
		{
			//内部使用GUI坐标进行绘制
			GUIStyle style = new GUIStyle();
			if(critical)
			{
				style.fontSize = 30*Screen.width/960;
				style.fontStyle = FontStyle.Bold;
			}
			else
			{
				style.fontSize = 20*Screen.width/960;
				style.fontStyle = FontStyle.Normal;
			}
			style.alignment = TextAnchor.UpperLeft;

			if (displayType==DAMAGE) style.normal.textColor = Color.red;
			else if(displayType==REFUND) style.normal.textColor = Color.blue;
			else style.normal.textColor = Color.green;
			GUI.Label(new Rect(mPoint.x,mPoint.y,ContentWidth,ContentHeight),Value.ToString(),style);
		}
	}
	
	IEnumerator Free()
	{
		yield return new WaitForSeconds(FreeTime);
		Destroy(this.gameObject);
	}
}