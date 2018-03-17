using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IPlaceable {

	public int cost = 10;

	#region IPlaceable implementation
	GameCube gameCube = null;

	GameCube IPlaceable.Cube {
		get {
			return gameCube;
		}
		set {
			this.gameCube = value;

			if (this.gameCube != null) {
				this.gameCube.MoveCost = Mathf.Infinity;
			}
		}
	}

	public string Name {
		get {
			return "Wall";
		}
	}

	public int Cost {
		get {
			return 10;
		}
	}

	public string CostStat {
		get {
			return cost.ToString();
		}
	}

	public string HealthStat {
		get {
			return "-"; //walls should be damageable at some point
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



}
