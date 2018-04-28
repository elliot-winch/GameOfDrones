using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : DamagableObject {

	public override void Hit (Vector3 hitPoint, Vector3 from, float amount)
	{
		ResourceManager.Instance.Spend (20);
	}
}
