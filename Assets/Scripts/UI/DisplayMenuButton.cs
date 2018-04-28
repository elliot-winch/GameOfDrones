using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenuButton : DamagableObject {

	public GameObject menu;

	public override void Hit (Vector3 hitPoint, Vector3 from, float amount)
	{
		base.Hit (hitPoint, from, amount);
	}

	protected override void Destroyed ()
	{
		GameManager.Instance.DisplayMenu (menu);

		base.Destroyed ();
	}
}
