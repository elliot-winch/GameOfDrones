using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : Wall {
	
	protected override void Destroyed ()
	{
		this.gameCube.Locked = false;

		GameManager.Instance.EndGame ();

		base.Destroyed ();
	}
}
