using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]

public abstract class HeldObject : MonoBehaviour {

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;

	private Sprite[] controlWheelActionIcons;

	ControlWheel controlWheel;

	protected virtual void Start(){
		controlWheel = GetComponent<ControlWheel> ();

		controlWheelActionIcons = new Sprite[2];

		controlWheelActionIcons[0] = Resources.Load<Sprite> ("Icons/teleportIcon");
		controlWheelActionIcons[1] = Resources.Load<Sprite> ("Icons/dropIcon");


	}

	protected void HandHoverUpdate( Hand hand ){ }


	//Called every Update() while this GameObject is attached to the hand 
	protected virtual void HandAttachedUpdate ( Hand hand) {
		HandControls hc = ControlsManager.Instance.GetControlsFromHand (hand);

		//Display control wheel when touched
		if (hc.TouchPadTouched.Down ) {
			controlWheel.DisplayControlWheel ();
		}

		//Highlight currently hovered over control
		//Do we want a more intense highlight for when we press over a sector?
		if (hc.TouchPadTouched.Held) {
			controlWheel.HighlightSectionAtLocation (hc.TouchPadLocation);
		}

		//Hie control wheel when not longer touched
		if (hc.TouchPadTouched.Up) {
			controlWheel.HideControlWheel ();
		}

		//On pressed, we active a section
		if (hc.TouchPadPressed.Up) {
			controlWheel.Select (hc.TouchPadLocation);
		}
	}


	protected virtual void OnAttachedToHand(Hand hand) { 

		ControlWheelSegment teleport = new ControlWheelSegment (() => {
			Debug.Log ("Teleport from Control Wheel");
		}, controlWheelActionIcons[0]);

		ControlWheelSegment drop = new ControlWheelSegment (() => {
			Debug.Log ("Drop from Control Wheel");
			// Detach this object from the hand



		}, controlWheelActionIcons[1]);

		controlWheel.AddControlWheelActions (new ControlWheelSegment[] {
			teleport, drop
		}, overwrite: true);

	}

	protected virtual void OnDetachedFromHand(Hand hand) {

		controlWheel.HideControlWheel ();
	}

}
