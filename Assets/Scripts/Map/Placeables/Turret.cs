using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Wall {

	public float range = 3f;
	public float damage = 1f;
	public float weaponChargeTime = 0.2f;

	public float rotateSpeed = 1f; //cosmetic
	public GameObject projectile;

	private GameCube cube;

	private bool firing = false;

	private DamagableObject currentTarget;
	private Action onShoot;
	private Coroutine rotating;

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	protected override void Update(){

		//Attacking
		Collider[] cols = Physics.OverlapSphere (transform.position, range, LayerMask.GetMask("Enemy"));

		if (firing == false) {
			if (currentTarget == null) {
				if (cols.Length > 0) {
					//decide which enemy to shoot - currently we just pick the first target found
					currentTarget = cols [0].GetComponentInParent<DamagableObject> ();
					StartCoroutine(Fire ());

				}
			} 
		}


	}

	IEnumerator Fire(){
		firing = true;

		if(rotating != null){
			StopCoroutine (rotating);
		}

		rotating = StartCoroutine(SmoothLookAt (currentTarget.transform));

		yield return new WaitForSeconds(weaponChargeTime);

		if (currentTarget == null) {
			//the target has been destroyed before this got a chance to shoot at it
			firing = false;
			yield break;
		}

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		proj.GetComponent<Projectile> ().Launch (currentTarget.transform.position, gameObject, new string[] { "Enemy" });

		currentTarget = null;

		firing = false;
	}


	IEnumerator SmoothLookAt(Transform target){

		Debug.Log ("Coroutine enter for turret");
		Quaternion targetRotation = Quaternion.LookRotation (target.position - transform.position);

		while (Quaternion.Angle (transform.rotation, targetRotation) > 1f) {

			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
