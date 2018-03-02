using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public abstract class HeldObject : MonoBehaviour {

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;


	private void HandHoverUpdate( Hand hand ){

		if (hand.currentAttachedObject != gameObject) {

			hand.HoverLock (GetComponent<Interactable> ());

			hand.AttachObject (gameObject, attachmentFlags);
		}
	}


	//Called every Update() while this GameObject is attached to the hand 
	private void HandAttachedUpdate ( Hand hand) {

		if (Input.GetKeyDown(BuildManager.Instance.putDownKey) ) {
			Debug.Log ("Outting down");
			hand.DetachObject (gameObject);

			hand.HoverUnlock (GetComponent<Interactable> ());
		}

		//does this need to be fed hand?
		OnHeld (hand);

	
	}

	abstract protected void OnHeld (Hand hand);
}
