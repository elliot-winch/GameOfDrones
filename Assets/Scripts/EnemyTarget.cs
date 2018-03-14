using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : DamagableObject, IPlaceable {


	#region IPlaceable implementation
	GameCube cube;

	public GameCube Cube {
		get {
			return cube;
		}
		set {
			this.cube = value;

			transform.SetParent (cube.transform);
			transform.position = this.cube.Position;
		}
	}

	public int Cost {
		get {
			//enemyTarget cannot be placed
			return -1;
		}
	}

	#endregion

	protected override void Destroyed ()
	{
		EnemyManager.Instance.enabled = false;

		Debug.Log ("GameOver");
		//Game OVer!!!!
	}


}
