using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public abstract class HeldObject : MonoBehaviour {

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;


	private void HandHoverUpdate( Hand hand ){

		//temp
		if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && Input.GetKeyDown( BuildManager.Instance.putDownKey ) ) )
		{
			if ( hand.currentAttachedObject != gameObject )
			{
				// Call this to continue receiving HandHoverUpdate messages,
				// and prevent the hand from hovering over anything else
				hand.HoverLock( GetComponent<Interactable>() );

				// Attach this object to the hand
				hand.AttachObject( gameObject, attachmentFlags );
			}
			else
			{
				// Detach this object from the hand
				hand.DetachObject( gameObject );

				// Call this to undo HoverLock
				hand.HoverUnlock( GetComponent<Interactable>() );
			}
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
