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
	private GrippedToolIndex prevGunHeld = GrippedToolIndex.Pistol;

	private int numGuns;

	void Start ()
	{
		numGuns = Enum.GetNames (typeof(GrippedToolIndex)).Length - 1;

		StartCoroutine(AttachToolsAfterDelay(1f));
	}

	IEnumerator AttachToolsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		this.hand = this.GetComponent<Hand>();

		this.SwitchGrippedObject(this.startTool);

		//After this, wave management will be run by the WaveManager

	}

	public void SwitchGrippedObject(GrippedToolIndex index){

		if (this.prevHeld != null) {
			this.hand.DetachObject( this.prevHeld );

			// Call this to undo HoverLock
			this.hand.HoverUnlock( this.prevHeld.GetComponent<Interactable>() );

			this.prevHeld.GetComponent<HeldObject> ().OnDestroy ();

			Destroy (this.prevHeld);

			if ((int)index >= (int)GrippedToolIndex.Pistol) {
				this.prevGunHeld = index;
			}
		}

		GameObject playerHeld = Instantiate (this.playerHeldPrefabs[(int)index].gameObject);

		this.hand.HoverLock( playerHeld.GetComponent<Interactable>() );

		// Attach this object to the hand
		this.hand.AttachObject( playerHeld, this.attachmentFlags );


		//Set Up Control Wheel Actions

		if (index == GrippedToolIndex.BuildTool) {
			playerHeld.GetComponent<ControlWheel> ().AddControlWheelAction (
				new ControlWheelSegment (
					name: "Switch",
					action: () => {
						SwitchGrippedObject (prevGunHeld);
					},
					icon: Resources.Load<Sprite>("Icons/swapIcon")
				));
		} else {
			ControlWheelSegment buildTool = new ControlWheelSegment (
					name: "Switch",
					action: () => {
						SwitchGrippedObject (GrippedToolIndex.BuildTool);
					},
					icon: Resources.Load<Sprite>("Icons/swapIcon")
			);
				
			ControlWheelSegment left = new ControlWheelSegment(
				name: "Change Gun Left",
				action: () =>
				{
					int newIndex = ((int)index + this.numGuns - 2) % this.numGuns + 1;

					SwitchGrippedObject((GrippedToolIndex)newIndex);

				}, 
				icon: Resources.Load<Sprite> ("Icons/left-arrow"),
				preferredPosition: ControlWheelSegment.PreferredPosition.Left
			);

			ControlWheelSegment right = new ControlWheelSegment(
				name: "Change Gun Right",
				action : () =>
				{
					int newIndex = ((int)index) % this.numGuns + 1;

					SwitchGrippedObject((GrippedToolIndex)newIndex);

				}, 
				icon: Resources.Load<Sprite> ("Icons/right-arrow"),
				preferredPosition: ControlWheelSegment.PreferredPosition.Right
			);

			playerHeld.GetComponent<ControlWheel> ().AddControlWheelActions(new ControlWheelSegment[] {
				left,
				buildTool,
				right,
			});
		}


		this.prevHeld = playerHeld;
	}
}

public enum GrippedToolIndex {
	BuildTool,
	Pistol,
	RapidFire
	//etc
}