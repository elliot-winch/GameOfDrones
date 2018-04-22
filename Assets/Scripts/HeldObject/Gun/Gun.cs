using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public abstract class Gun : HeldObject {

	public float damage;
	public float rateOfFire;

	public GameObject projectile;

	private float rateOfFireTimer;
	public Transform barrel;

	private Recoiler recoiler;

	protected override void Awake(){
		base.Awake();

		rateOfFireTimer = 0f;

		recoiler = GetComponentInChildren<Recoiler> ();
	}

	#region HeldObject
	protected override void HandAttachedUpdate (Hand hand){

		base.HandAttachedUpdate (hand);

		if (rateOfFireTimer > rateOfFire)
		{
			if (FireControlActivated(ControlsManager.Instance.GetControlsFromHand (hand)))
			{
				Fire(hand);
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


	protected abstract bool FireControlActivated (HandControls hc);

	protected virtual void Fire (Hand hand) {

		recoiler.Recoil ();
	}
}
