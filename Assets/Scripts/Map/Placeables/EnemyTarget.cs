using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : Wall {
	
	protected override void Destroyed ()
	{
		EnemyPathManager.Instance.enabled = false;

		Debug.Log ("GameOver");
	}
}
