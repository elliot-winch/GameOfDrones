using System;
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

	public HeldObject[] playerHeldPrefabs;
	public GrippedToolIndex startTool;

	private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & Hand.AttachmentFlags.SnapOnAttach & Hand.AttachmentFlags.DetachOthers;
	private Hand hand;
	private GameObject prevHeld;

	void Start ()
	{
		StartCoroutine(AttachToolsAfterDelay(1f));
	}

	IEnumerator AttachToolsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		this.hand = this.GetComponent<Hand>();

		this.SwitchGrippedObject(this.startTool);
	}

	public void SwitchGrippedObject(GrippedToolIndex index){

		if (this.prevHeld != null) {
			this.hand.DetachObject( this.prevHeld );

			// Call this to undo HoverLock
			this.hand.HoverUnlock( this.prevHeld.GetComponent<Interactable>() );

			Destroy (this.prevHeld);
		}

		GameObject playerHeld = Instantiate (this.playerHeldPrefabs[(int)index].gameObject);

		this.hand.HoverLock( playerHeld.GetComponent<Interactable>() );

		// Attach this object to the hand
		this.hand.AttachObject( playerHeld, this.attachmentFlags );


		//Set Up Control Wheel Actions

		List<ControlWheelSegment> segs = new List<ControlWheelSegment>();

		foreach(GrippedToolIndex switchTo in Enum.GetValues(typeof(GrippedToolIndex)))
		{
			if(switchTo == index)
			{
				continue;
			}

			segs.Add( new ControlWheelSegment(() =>
		   {
			   SwitchGrippedObject(switchTo);
		   }, null));
		}

		playerHeld.GetComponent<ControlWheel>().AddControlWheelActions(segs.ToArray());

		this.prevHeld = playerHeld;
	}
}

public enum GrippedToolIndex {
	BuildTool,
	Pistol
	//etc
}