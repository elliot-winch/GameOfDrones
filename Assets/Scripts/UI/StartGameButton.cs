using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : DamagableObject {

	public override void Hit (Vector3 hitPoint, Vector3 hitDirection, float amount)
	{
		base.Hit (hitPoint, hitDirection, amount);
	}

	protected override void Destroyed ()
	{
		Debug.Log ("Start Game");
		GameManager.Instance.StartGame ();

		base.Destroyed ();
	}
}
