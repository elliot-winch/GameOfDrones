using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public abstract class HeldObject : MonoBehaviour {

	protected ControlWheel controlWheel;

	public ControlWheel ControlWheel {
		get {
			return controlWheel;
		}
	}

	protected virtual void Awake(){
		controlWheel = GetComponent<ControlWheel> ();

	}

	protected void HandHoverUpdate( Hand hand ){ }


	//Called every Update() while this GameObject is attached to the hand 
	bool isDisplaying = false; 
	protected virtual void HandAttachedUpdate ( Hand hand) {
		HandControls hc = ControlsManager.Instance.GetControlsFromHand (hand);

		//Highlight currently hovered over control
		//Do we want a more intense highlight for when we press over a sector?

		//Why not use Down? Becuase when we switch weapons, down is already called
		//but the control wheel isn't displayed
		if (hc.TouchPadTouched.Held || hc.TouchPadPressed.Held) {
			controlWheel.HighlightSectionAtLocation (hc.TouchPadLocation);

			if (isDisplaying == false)
			{
				controlWheel.DisplayControlWheel();
				isDisplaying = true;
			}
		}

		//Hide control wheel when not longer touched
		if (hc.TouchPadTouched.Up) {
			controlWheel.HideControlWheel ();
			isDisplaying = false;
		}

		//On pressed, we active a section
		if (hc.TouchPadPressed.Up) {
			controlWheel.Select (hc.TouchPadLocation);
		}
	}


	protected virtual void OnAttachedToHand(Hand hand) {

	}

	protected virtual void OnDetachedFromHand(Hand hand) {

		controlWheel.HideControlWheel ();
	}

	protected IEnumerator HapticPulseForTime(SteamVR_Controller.Device controller, float time, float strengthPercentage ){

		float timer = 0f;

		while (timer < time) {

			controller.TriggerHapticPulse ((ushort)Mathf.Lerp (0, 3999, strengthPercentage));

			timer += Time.deltaTime;
			yield return null;
		}

	}

	public virtual void OnDestroy() {

	}
}
