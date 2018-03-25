using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public abstract class HeldObject : MonoBehaviour {

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;

	protected void HandHoverUpdate( Hand hand ){

		//temp
		if ( hand.GetStandardInteractionButtonDown() )
		{
			if ( hand.currentAttachedObject != gameObject )
			{
				// Call this to continue receiving HandHoverUpdate messages,
				// and prevent the hand from hovering over anything else
				hand.HoverLock( GetComponent<Interactable>() );

				// Attach this object to the hand
				hand.AttachObject( gameObject, attachmentFlags );
			}
			/*
			else
			{
				// Detach this object from the hand
				hand.DetachObject( gameObject );

				// Call this to undo HoverLock
				hand.HoverUnlock( GetComponent<Interactable>() );
			}*/
		}
	}


	//Called every Update() while this GameObject is attached to the hand 
	protected virtual void HandAttachedUpdate ( Hand hand) {

		if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			hand.DetachObject (gameObject);

			hand.HoverUnlock (GetComponent<Interactable> ());

		}
	}

	protected virtual void Start() { }


	protected virtual void OnAttachedToHand(Hand hand) {
		hand.GetComponentInChildren<ControllerHoverHighlight>().enabled = false;
	}


	protected virtual void OnDetachedFromHand(Hand hand) {
		hand.GetComponentInChildren<ControllerHoverHighlight>().enabled = true;

	}

}
