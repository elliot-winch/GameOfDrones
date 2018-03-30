using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : DamagableObject, IPlaceable {

	GameCube gameCube = null;

	GameCube IPlaceable.Cube {
		get {
			return gameCube;
		}
		set {
			this.gameCube = value;

			transform.SetParent (gameCube.transform);
			transform.position = this.gameCube.Position;

			if (this.gameCube != null) {
				this.gameCube.MoveCost = Mathf.Infinity;

				EnemyPathManager.Instance.DisplayEnemyPathUI ();
			}
		}
	}

	public int cost = 10;

	public int Cost {
		get {
			return 10;
		}
	}
}
