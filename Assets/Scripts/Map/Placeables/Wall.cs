using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : DamagableObject, IPlaceable {

	public int buildCost = 10;
	public float moveCost = Mathf.Infinity;

	GameCube gameCube;

	GameCube IPlaceable.Cube {
		get {
			return gameCube;
		}
		set {
			this.gameCube = value;

			transform.SetParent (gameCube.transform);
			transform.position = this.gameCube.Position;

			if (this.gameCube != null) {
				this.gameCube.MoveCost = moveCost;

			}
		}
	}

	public virtual int BuildCost {
		get {
			return buildCost;
		}
	}

	public virtual float MoveCost {
		get {
			return moveCost;
		}
	}
}
