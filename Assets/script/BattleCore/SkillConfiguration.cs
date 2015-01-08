using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillConfiguration : MonoBehaviour {

	[System.Serializable]
	public class SkillData
	{
		public string _name;
		public string _sprite;
		public string _prefab;
		public string _type;
		public int _loopCount = 0;
		public float _duration;
		public float _timeBetweenLoop;
	}

	public List<SkillData> availableSkills;
}
