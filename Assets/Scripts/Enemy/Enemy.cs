using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamagableObject {

	public override void Hit (float amount)
	{
		base.Hit (amount);

		Debug.Log ("ow " + amount);
	}
}
