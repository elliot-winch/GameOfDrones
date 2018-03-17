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


	public string Name {
		get {
			return "Enemy Target";
		}
	}

	public int Cost {
		get {
			return -1;
		}
	}

	public string CostStat {
		get {
			return "-";
		}
	}

	public string HealthStat {
		get {
			return CurrentHealth.ToString ();
		}
	}

	public string DamageStat {
		get {
			return "-";
		}
	}

	public string RangeStat {
		get {
			return "-";
		}
	}

	public string RateOfFireStat {
		get {
			return "-";
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
