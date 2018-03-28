using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Gun : HeldObject {

	protected Action onShoot;

	public float damage;
	public float rateOfFire;

	public GameObject projectile;

	private float rateOfFireTimer;
	private Transform barrel;

	//every coroutine that you launch should call firelock when it begins and fireunlock when it ends
	private int firingLock = 0;

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	protected override void Start(){

		rateOfFireTimer = 0f;
		barrel = transform.GetChild(0).Find ("EndOfBarrel");
	}

	#region HeldObject
	protected override void HandAttachedUpdate (Hand hand){

		base.HandAttachedUpdate (hand);

		HandControls hc = ControlsManager.Instance.GetControlsFromHand (hand);

		if (rateOfFireTimer > rateOfFire)
		{
			if (hc.TriggerPulled.Down)
			{
				Fire();
				//hand.controller.TriggerHapticPulse();
				rateOfFireTimer = 0f;
			}
		}
		else
		{
			rateOfFireTimer += Time.deltaTime;
		}
	}

	protected override void OnAttachedToHand(Hand hand)
	{
		base.OnAttachedToHand(hand);
	}

	protected override void OnDetachedFromHand (Hand hand)
	{
		base.OnDetachedFromHand (hand);

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

		GameObject proj = Instantiate (projectile, barrel.transform.position, Quaternion.identity);

		Collider projCol = proj.GetComponent<Collider> ();

		foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			Physics.IgnoreCollision (projCol, mc);
		}

		proj.GetComponent<Projectile> ().Launch (barrel.transform, new string[] { "Friendly" });

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
