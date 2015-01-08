using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GridObjectFixator : MonoBehaviour
{

		// Update is called once per frame
		void Update ()
		{
			if (Application.isPlaying == false) {
					Vector2 coord = BattleGrid.instance.positionToCoord (transform.position);
					coord.x = Mathf.Clamp(coord.x, 0, BattleGrid.instance.cols - 5);
					coord.y = Mathf.Clamp(coord.y, 0, BattleGrid.instance.rows - 1);
					this.transform.position = BattleGrid.instance.coordToPosition (coord);
//					Debug.Log("dafasdf"+transform.position.z);
				}
		}
}

