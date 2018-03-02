using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Gun : HeldObject {

	protected Action onShoot;

	public float damage;
	public float rateOfFire;

	private float rateOfFireTimer;

	//every coroutine that you launch should call firelock when it begins and fireunlock when it ends
	private int firingLock = 0;

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	private void Start(){
		rateOfFireTimer = rateOfFire;
	}

	#region HeldObject
	protected override void OnHeld (Hand hand)
	{
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
