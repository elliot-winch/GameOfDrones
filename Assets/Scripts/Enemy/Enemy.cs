using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamagableObject {

	public float attackRange = 1f;

	protected DamagableObject currentTarget;

	public override void Hit (float amount)
	{
		base.Hit (amount);

		Debug.Log ("ow " + amount);
	}

	void Update(){
		//move here too

		if (currentTarget == null) {
			GetTargets ();
		}
	}

	void GetTargets(){
		Collider[] cols = Physics.OverlapSphere (transform.position, attackRange, LayerMask.GetMask("Friendly"));

		if (currentTarget == null) {
			if (cols.Length > 0) {

				//decide which enemy to shoot

				cols [0].GetComponentInParent<DamagableObject> ().Hit (1);

			}
		} else {
			//shoot!
		}
	}
}
