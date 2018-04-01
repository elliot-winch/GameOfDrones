using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy {

	public GameObject projectile;

	//rotateSpeed is for cosmetics only. A drone will not delay firing to rotate
	public float rotateSpeed = 1f;

	private Coroutine lookAt;

	Rigidbody rb;

	protected override void Start ()
	{
		base.Start ();

		rb = GetComponent<Rigidbody> ();
	}

	//Shooting overrides
	protected override void PreFire(DamagableObject target){
		base.PreFire (target);

		LookAt (target.transform.position);
	}

	protected override void OnFire (DamagableObject target)
	{
		base.OnFire (target);

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		Vector3 aimPos = target.transform.position;

		proj.GetComponent<Projectile> ().Launch (aimPos, gameObject, new string[] { "Friendly" });
		//play sound, animation etc.

	}

	protected override void Disengage ()
	{
		base.Disengage ();

		if (Firing == false) {
			LookAt (destinationPosition);
		}
	}
	//end shooting overrides

	public override void Hit (Vector3 hitDirection, float amount)
	{
		base.Hit (hitDirection, amount);

		PauseMovementTowardsTarget (true);

		rb.AddForce (hitDirection * 1f);

		//TODO a nice particle effect
		//a sound
	}

	//Private methods
	private void LookAt(Vector3 position){
		if (lookAt != null) {
			StopCoroutine (lookAt);
			lookAt = null;
		}

		lookAt = StartCoroutine (SmoothLookAt (position));
	}


	IEnumerator SmoothLookAt(Vector3 position){

		Quaternion targetRotation = Quaternion.LookRotation (position - transform.position);


		while (Quaternion.Angle (transform.rotation, targetRotation) > 1f) {

			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
