using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Valve.VR.InteractionSystem;

public class BuildTool : HeldObject {

	public Canvas gameCubeActTimeCanvasPrefab;
	public GameObject[] buildables;
	public GameObject deleteModel;
	public int startBuildable = 0;

	private int currentID;

	Transform barrel;

	//BuiltToolStatsCanvas btStatsCanvas;
	BuildToolPreview btPreview;
	BuildToolProgressBar brProgressBar;

	List<IHeldUpdateable> heldUpdateables;

	public int CurrentID {
		get {
			return currentID;
		}
		set {
			if (value >= 0) {
				currentID = value;

				btPreview.DisplayModel (buildables [currentID]);
				//btStatsCanvas.FillStats (buildables[currentID].GetComponent<IPlaceable>());
			} else {
				currentID = value;

				btPreview.DisplayModel (deleteModel);
				//display remove UI
			}
		}
	}



	protected override void Awake(){
		base.Awake();

		barrel = transform.GetChild(0).Find ("EndOfBarrel");

		//btStatsCanvas = GetComponentInChildren<BuiltToolStatsCanvas> (true);
		btPreview = GetComponent<BuildToolPreview> ();
		brProgressBar = GetComponent<BuildToolProgressBar> ();

		ResourceManager.Instance.btrd =  GetComponentInChildren<BuildToolResourceDisplay> (true);

		heldUpdateables = new List<IHeldUpdateable> ();

		heldUpdateables.Add (btPreview);
		//heldUpdateables.Add (btStatsCanvas);

		currentID = startBuildable;

		//Act on Cube UI


		//Control Wheel Actions
		ControlWheelSegment left = new ControlWheelSegment(
			name: "Change Buildable Left",
			action: () =>
	   			{
					//Mod doesn't work with negative numbers
					int newID = currentID - 1;
					if (newID < -1){
						this.CurrentID = buildables.Length - 1;
					} else {
						this.CurrentID = newID;
					}
					
	   			}, 
			icon: Resources.Load<Sprite> ("Icons/left-arrow"),
			preferredPosition: ControlWheelSegment.PreferredPosition.Left);

		ControlWheelSegment right = new ControlWheelSegment(
			name: "Change Buildable Right",
			action : () =>
			{
					//Mod doesn't work with negative numbers
					int newID = currentID + 1;
					if (newID >= buildables.Length){
						this.CurrentID = -1;
					} else {
						this.CurrentID = newID;
					}
					
				}, 
			icon: Resources.Load<Sprite> ("Icons/right-arrow"),
			preferredPosition: ControlWheelSegment.PreferredPosition.Right);

		controlWheel.AddControlWheelActions(new ControlWheelSegment[] {
			left,
			right
			});
	}

	#region HeldObject
	GameObject lastPointedAtOccupying; //this needs to be cached to avoid the bug where continuouslt holding down keeps building
	GameCube lastPointedAt;
	bool built; //to stop continuously holding the trigger to mean continuously acting on a block
	protected override void HandAttachedUpdate (Hand hand){
		base.HandAttachedUpdate (hand);

		HandControls hc = ControlsManager.Instance.GetControlsFromHand (hand);

		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldUpdate ();
		}

		//Raycasting comes last as returning form the function is an option

		RaycastHit hitInfo;

		if (Physics.Raycast (barrel.transform.position, barrel.transform.forward, out hitInfo, Mathf.Infinity)) {
			if (hitInfo.collider != null) {
				//we hit a collider
				GameCube cube = hitInfo.collider.GetComponent<GameCube> ();

				if (cube != null) {

					//When we look at a different cube or the cube we look at changes
					if (lastPointedAt != cube || lastPointedAtOccupying != cube.Occupying) {
						brProgressBar.ActOnCubeTimer = 0f;

						if (lastPointedAt != null) {
							lastPointedAt.OnPointedAway ();
						}

						if (currentID >= 0) {
							cube.OnPointedAt (buildables [currentID].GetComponent<IPlaceable> (), this);
						} else {
							cube.OnPointedAt (null, this);
						}
					}

					//Called every frame that we point to the cube
					lastPointedAt = cube;
					lastPointedAtOccupying = cube.Occupying;

					if (hc.TriggerPulled.Any && built == false) {

						brProgressBar.ActOnCubeTimer += Time.deltaTime;

						if (brProgressBar.ActOnCubeTimer >= brProgressBar.holdToActTime) {
							ActOnCube (cube);
							brProgressBar.ActOnCubeTimer = 0f;
							built = true;
						}
					} else {
						brProgressBar.ActOnCubeTimer = Mathf.Max(0f, brProgressBar.ActOnCubeTimer - (Time.deltaTime * 2f));
					}

					if (hc.TriggerPulled.Up) {
						built = false;
					}

					return;
				} 
			}
		} 

		//missed cube tihs frame, but last frame was hitting a box
		if (lastPointedAt != null) {
			lastPointedAt.OnPointedAway ();

			lastPointedAt = null;

			brProgressBar.ActOnCubeTimer = 0f;
		}
	}

	protected override void OnAttachedToHand (Hand hand)
	{
		base.OnAttachedToHand (hand);

		this.CurrentID = this.CurrentID;

		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldStart ();
		}

	}

	protected override void OnDetachedFromHand (Hand hand)
	{
		base.OnDetachedFromHand (hand);


		//Update other components when held
		foreach(IHeldUpdateable hu in heldUpdateables){
			hu.HeldEnd ();
		}
	} 
	#endregion //HeldObject

	#region Build Functionality
	public void ActOnCube(GameCube gc){

		if (gc.CurrentlyPointingAt == this) {
			//If placing
			if (currentID >= 0) {

				IPlaceable placeable = buildables [currentID].GetComponent<IPlaceable> ();

				GameCube.PlacementError pe = gc.CanPlace (placeable);

				if (pe == GameCube.PlacementError.None) {
					GameObject p = Instantiate (buildables [currentID]);

					//most of the spawning process is handled by the gamecube
					gc.Occupying = p;

					//spend resources
					ResourceManager.Instance.Spend (p.GetComponent<IPlaceable> ().BuildCost);

					EnemyPathManager.Instance.ShouldRecalcPathBlocked (gc);

				} else {
					//failure cases
					Debug.Log (pe.ToString ());
				}
			} else {
			
				GameCube.RemoveError re = gc.CanRemove ();

				if (re == GameCube.RemoveError.None) {

					ResourceManager.Instance.AddResources (gc.Occupying.GetComponent<IPlaceable> ().BuildCost);

					gc.Occupying = null;

					EnemyPathManager.Instance.ShouldRecalcPathRemoved ();

				} else {
					//failure case
					Debug.Log (re.ToString ());
				}
			}
		} else {
			Debug.Log ("Another build tool is pointed at this cube first!");
		}

	}

	#endregion
}
