using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : DamagableObject {

	protected override void Destroyed ()
	{
		Debug.Log ("Game over, man!");
	}
}
