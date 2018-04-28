using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : EndSeekingEnemy {

	public GameObject projectile;
	public float projectileSpeed;
	public float separationSpeed;
	List<Transform> barrels;

	protected override void Start ()
	{
		base.Start ();

		//NB only searches top level childrens
		barrels = new List<Transform>();
		foreach (Transform t in transform) {
			if (t.name == "Barrel") {
				barrels.Add (t);
			}
		}
	}


	protected override void OnFire (Collider target)
	{
		base.OnFire (target);

		StartCoroutine (Triple(target));
		//play sound, animation etc.

	}

	IEnumerator Triple(Collider target){

		foreach (Transform t in barrels) {

			GameObject proj = Instantiate (projectile, t.position, Quaternion.identity);

			Vector3 aimPos = target.bounds.center;

			proj.GetComponent<Projectile> ().Launch (aimPos, this.attackDamage, this.projectileSpeed, gameObject, layersToHit.ToArray());

			fireAS.clip = EnemyAudioManager.Instance.GetRandomShootAudio ();
			fireAS.Play ();

			yield return new WaitForSeconds (separationSpeed);
		}
	}
}
