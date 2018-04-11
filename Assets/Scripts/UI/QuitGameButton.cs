using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameButton : DamagableObject {

	public override void Hit (Vector3 hitPoint, Vector3 hitDirection, float amount)
	{
		base.Hit (hitPoint, hitDirection, amount);

		Debug.Log ("Quit Hit once");
	}


	protected override void Destroyed ()
	{
		GameManager.Instance.Quit ();

		base.Destroyed ();
	}
}
