using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : EndSeekingEnemy {

	public GameObject projectile;
	public float projectileSpeed;

	protected override void OnFire (Collider target)
	{
		base.OnFire (target);

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		Vector3 aimPos = target.bounds.center;

		fireAS.clip = EnemyAudioManager.Instance.GetRandomShootAudio ();
		fireAS.Play ();

		proj.GetComponent<Projectile> ().Launch (aimPos, this.attackDamage, this.projectileSpeed, gameObject, layersToHit.ToArray());
		//play sound, animation etc.

	}
}
