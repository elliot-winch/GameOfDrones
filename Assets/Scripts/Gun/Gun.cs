using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Gun : LaserHeldObject {

	protected Action onShoot;

	public float damage;
	public float rateOfFire;

	public GameObject projectile;

	private float rateOfFireTimer;
	private bool firing = false;
	private Transform barrel;

	//every coroutine that you launch should call firelock when it begins and fireunlock when it ends
	private int firingLock = 0;

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	protected override void Start(){
		base.Start ();

		rateOfFireTimer = rateOfFire;
		barrel = transform.Find ("EndOfBarrel");
	}

	#region HeldObject
	protected override void HandAttachedUpdate (Hand hand){
		base.HandAttachedUpdate (hand);

		if (rateOfFireTimer > rateOfFire && firingLock <= 0) {
			if (Input.GetKeyDown(GunManager.Instance.fireKey)) {
				Fire ();
			} 
		} else {
			rateOfFireTimer += Time.deltaTime;
		}
	}
	#endregion

	#region Fire
	/* Raycast Firing - not cool
	protected virtual void Fire(){
		rateOfFireTimer = 0f;

		if (onShoot != null) {
			onShoot ();
		}

		RaycastHit hitInfo;

		if(Physics.Raycast(transform.position, transform.forward, out hitInfo)){
			if(hitInfo.collider != null){
				//we hit a collider
				if (hitInfo.collider.GetComponentInParent<DamagableObject> () != null) {
					hitInfo.collider.GetComponentInParent<DamagableObject> ().Hit (10);
				}

			}
		}
	}*/


	protected virtual void Fire(){
		firing = true;

		GameObject proj = Instantiate (projectile, barrel.transform.position, Quaternion.identity);

		Collider projCol = proj.GetComponent<Collider> ();

		foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			Physics.IgnoreCollision (projCol, mc);
		}

		proj.GetComponent<Projectile> ().Launch (barrel.transform);

		firing = false;
	}

	//This should be called at the beginning of every coroutine associated with Fire
	public void FireLock(){
		firingLock++;
	}

	public void FireUnlock(){
		firingLock--;
	}
	#endregion
}
