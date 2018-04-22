using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : EndSeekingEnemy {

	public GameObject projectile;

	//Rigidbody rb;

	protected override void OnFire (DamagableObject target)
	{
		base.OnFire (target);

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		Vector3 aimPos = target.transform.position;

		proj.GetComponent<Projectile> ().Launch (aimPos, this.attackDamage, gameObject, new string[] { "Friendly" });
		//play sound, animation etc.

	}
}
