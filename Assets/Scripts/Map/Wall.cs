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

	public int Cost {
		get {
			return cost;
		}
	}
	#endregion



}
