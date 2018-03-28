using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Valve.VR.InteractionSystem;

/*
 * This script should be attached to a hand, which will hold the HeldObject 
 * for the entirity of the game
 * 
 */ 

[RequireComponent(typeof(Hand))]
public class GrippedTool: MonoBehaviour {

	public HeldObject[] playerHeldPrefab;
	public GrippedToolIndex startTool;

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;
	private Hand hand;
	private GameObject prevHeld;

	void Start () {

		this.hand = this.GetComponent<Hand> ();

		this.SwitchGrippedObject (this.startTool);
	}

	public void SwitchGrippedObject(GrippedToolIndex index){

		if (this.prevHeld != null) {
			this.hand.DetachObject( this.prevHeld );

			// Call this to undo HoverLock
			this.hand.HoverUnlock( this.prevHeld.GetComponent<Interactable>() );

			Destroy (this.prevHeld);
		}

		GameObject playerHeld = Instantiate (this.playerHeldPrefab[(int)index].gameObject);

		this.hand.HoverLock( playerHeld.GetComponent<Interactable>() );

		// Attach this object to the hand
		this.hand.AttachObject( playerHeld, this.attachmentFlags );

		this.prevHeld = playerHeld;
	}
}

public enum GrippedToolIndex {
	BuildTool,
	Pistol
	//etc
}