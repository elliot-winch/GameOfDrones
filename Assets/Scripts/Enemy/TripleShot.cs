using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : EndSeekingEnemy {

	public GameObject projectile;
	public Transform[] barrels;

	protected override void OnFire (DamagableObject target)
	{
		base.OnFire (target);

		foreach (Transform t in barrels) {

			GameObject proj = Instantiate (projectile, t.position, Quaternion.identity);

			Vector3 aimPos = target.transform.position;

			proj.GetComponent<Projectile> ().Launch (aimPos, this.attackDamage, gameObject, new string[] { "Friendly" });
		}

		//play sound, animation etc.

	}
}
