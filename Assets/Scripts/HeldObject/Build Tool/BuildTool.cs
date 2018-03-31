using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class BuildTool : HeldObject {

	public GameObject[] buildables;

	Transform barrel;

	//BuiltToolStatsCanvas btStatsCanvas;
	BuildToolPreview btPreview;
	BuildToolResourceDisplay btResourceDisplay;

	List<IHeldUpdateable> heldUpdateables;

	private uint currentID ;
	public uint CurrentID {
		get {
			return currentID;
		}
		set {
			if(value >= 0){
				currentID = value;

				btPreview.PreviewBuildable (buildables[currentID]);
				//btStatsCanvas.FillStats (buildables[currentID].GetComponent<IPlaceable>());

			}
		}
	}

	protected override void Awake(){
		base.Awake();

		barrel = transform.GetChild(0).Find ("EndOfBarrel");

		//btStatsCanvas = GetComponentInChildren<BuiltToolStatsCanvas> (true);
		btPreview = GetComponent<BuildToolPreview> ();
		btResourceDisplay = GetComponentInChildren<BuildToolResourceDisplay> (true);

		heldUpdateables = new List<IHeldUpdateable> ();

		heldUpdateables.Add (btPreview);
		//heldUpdateables.Add (btStatsCanvas);

		//Control Wheel Actions
		ControlWheelSegment left = new ControlWheelSegment(() =>
	   {
		   this.CurrentID = (uint)((currentID + 1) % buildables.Length);
	   }, 
			icon: Resources.Load<Sprite> ("Icons/left-arrow"),
			preferredPosition: 1);

		ControlWheelSegment right = new ControlWheelSegment(() =>
		{
			this.CurrentID = (uint)((currentID - 1) % buildables.Length);
		}, 
			icon: Resources.Load<Sprite> ("Icons/right-arrow"),
			preferredPosition: 3);

		controlWheel.AddControlWheelActions(new ControlWheelSegment[] {
			left,
			right
			});
	}

	#region HeldObject
	GameCube lastPointedAt;
	protected override void HandAttachedUpdate (Hand hand){
		base.HandAttachedUpdate (hand);

		HandControls hc = ControlsManager.Instance.GetControlsFromHand (hand);

		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldUpdate ();
		}

		//TODO change so that left goes left, right goes right
		/*
		if (hc.TouchPadPressed.Down)
		{
			this.CurrentID = (currentID + 1) % buildables.Length;

			if (hand.controller != null) {
				hand.controller.TriggerHapticPulse ();
			}
		}*/

		//Raycasting comes last as returning form the function is an option

		RaycastHit hitInfo;

		if (Physics.Raycast (barrel.transform.position, barrel.transform.forward, out hitInfo, Mathf.Infinity)) {
			if (hitInfo.collider != null) {
				//we hit a collider
				GameCube cube = hitInfo.collider.GetComponent<GameCube> ();

				if (cube != null) {

					if (lastPointedAt != cube) {
						if (lastPointedAt != null) {
							lastPointedAt.OnPointedAway ();
						}

						cube.OnPointedAt ( buildables [currentID].GetComponent<IPlaceable> () );
					}

					lastPointedAt = cube;

					if (hc.TriggerPulled.Up) {

						//can place is chwcked in the manager
						BuildPlaceable (cube);
					}

					return;
				} 
			}
		} 

		//missed cube tihs frame, but last frame was hitting a box
		if (lastPointedAt != null) {
			lastPointedAt.OnPointedAway ();

			lastPointedAt = null;
		}
	}

	uint lastIDSelected = 0;
	protected override void OnAttachedToHand (Hand hand)
	{
		base.OnAttachedToHand (hand);

		this.CurrentID = lastIDSelected;

		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldStart ();
		}

	}

	protected override void OnDetachedFromHand (Hand hand)
	{
		base.OnDetachedFromHand (hand);

		Debug.Log ("Detaching build tool");

		lastIDSelected = this.CurrentID;

		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldEnd ();
		}
	} 
	#endregion //HeldObject

	#region Build Functionality
	public void BuildPlaceable(GameCube gc){

		IPlaceable placeable = buildables [currentID].GetComponent<IPlaceable> ();

		GameCube.PlacementError pe = gc.CanPlace (placeable);

		//null, ie there is no error
		if (pe == GameCube.PlacementError.None) {
			GameObject p = Instantiate (buildables [currentID]);

			//most of the spawning process is handled by the gamecube
			gc.Occupying = p;

			//spend resources
			ResourceManager.Instance.PlayerResources -= p.GetComponent<IPlaceable> ().BuildCost;
			btResourceDisplay.UpdateDisplay ();

			//check path
			EnemyPathManager.Instance.ShouldRecalcPath(gc);
		//failure cases
		} else {

		}
	}

	public void RemovePlaceable(IPlaceable p){
		//TODO: removing placeable
	}

	#endregion
}
