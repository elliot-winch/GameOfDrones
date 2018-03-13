using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy {

	public GameObject projectile;

	//rotateSpeed is for cosmetics only. A drone will not delay firing to rotate
	public float rotateSpeed = 1f;

	Coroutine lookAt;

	protected override void Update(){
		base.Update ();

		if (currentTarget == null && lookAt == null && moving == true) {
			lookAt = StartCoroutine (SmoothLookAt (destination.transform));
		}
	}


	protected override void PreFire(DamagableObject target){
		base.PreFire (target);

		if (lookAt != null) {
			StopCoroutine (lookAt);
			lookAt = null;
		}

		lookAt = StartCoroutine (SmoothLookAt (target.transform));
	}

	protected override void OnFire (DamagableObject target)
	{
		base.OnFire (target);

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		Collider projCol = proj.GetComponent<Collider> ();

		foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			Physics.IgnoreCollision (projCol, mc);
		}

		proj.GetComponent<Projectile> ().Launch (target);
		//play sound, animation etc.

	}

	IEnumerator SmoothLookAt(Transform target){

		Quaternion targetRotation = Quaternion.LookRotation (target.position - transform.position);

		while (Quaternion.Angle (transform.rotation, targetRotation) > 1f) {
			Debug.Log (name + " looking at " + transform.rotation.eulerAngles);

			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
