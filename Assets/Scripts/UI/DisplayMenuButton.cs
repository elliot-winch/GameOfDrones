using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenuButton : DamagableObject {

	public GameObject menu;

	public override void Hit (Vector3 hitPoint, Transform hitFrom, float amount)
	{
		base.Hit (hitPoint, hitFrom, amount);
	}

	protected override void Destroyed ()
	{
		GameManager.Instance.DisplayMenu (menu);

		base.Destroyed ();
	}
}
