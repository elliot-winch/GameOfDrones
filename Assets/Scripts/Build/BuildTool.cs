using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class BuildTool : LaserHeldObject {

	public float previewScale = 0.15f;
	public float previewRotateSpeed = 100f;
	public float previewAlpha = 0.3f;
	public Material previewMat;

	Transform barrel;
	Transform previewArea;

	StatsCanvas btui;

	private int currentID ;
	public int CurrentID {
		get {
			return currentID;
		}
		set {
			if(value >= 0){
				currentID = value;

				StartPreviewBuildable ();
			}
		}
	}

	protected override void Start(){
		base.Start ();

		barrel = transform.Find ("EndOfBarrel");
		previewArea = transform.Find ("BuildPreview");

		btui = GetComponentInChildren<StatsCanvas> (true);
		btui.gameObject.SetActive (false);
	}

	#region HeldObject
	GameCube lastPointedAt;
	protected override void HandAttachedUpdate (Hand hand){
		base.HandAttachedUpdate (hand);

		//Preview spin
		if (previewObj != null) {
			previewObj.transform.Rotate(new Vector3(0, Time.deltaTime * previewRotateSpeed, 0));
		}

		//temp code - this needs to be VR friendly!!
		if (Input.GetKeyDown (KeyCode.R)) {
			this.CurrentID = (currentID + 1) % BuildManager.Instance.buildables.Length;
		} 

		//Raycasting coms last as returning form the function is an option

		RaycastHit hitInfo;

		if (Physics.Raycast (barrel.transform.position, barrel.transform.forward, out hitInfo)) {
			if (hitInfo.collider != null) {
				//we hit a collider
				GameCube cube = hitInfo.collider.GetComponent<GameCube> ();

				if (cube != null) {

					if (lastPointedAt != cube) {
						if (lastPointedAt != null) {
							lastPointedAt.OnPointedAway ();
						}

						cube.OnPointedAt ( BuildManager.Instance.CanPlace(currentID, cube) );
					}

					lastPointedAt = cube;

					if (Input.GetKeyDown (BuildManager.Instance.buildKey)) {

						//can place is chwcked in the manager
						BuildManager.Instance.BuildPlaceable (currentID, cube);
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

	int lastIDSelected = 0;
	protected override void OnAttachedToHand (Hand hand)
	{
		base.OnAttachedToHand (hand);

		btui.gameObject.SetActive (true);

		this.CurrentID = lastIDSelected;

	}

	protected override void OnDetachedFromHand (Hand hand)
	{
		base.OnDetachedFromHand (hand);

		lastIDSelected = this.CurrentID;

		btui.gameObject.SetActive (false);

		RemovePreviewBuild ();

	} 
	#endregion

	#region Building Functionality

	GameObject previewObj = null;
	void StartPreviewBuildable(){

		if (previewObj != null) {
			RemovePreviewBuild();
		}


		previewObj = Instantiate (BuildManager.Instance.buildables[currentID], previewArea);

		previewObj.transform.localPosition = Vector3.zero;
		previewObj.transform.localRotation = Quaternion.identity;

		previewObj.transform.localScale = previewObj.transform.lossyScale *  previewScale;

		//preview obj shouldnt shot at enemies!
		if (previewObj.GetComponent<Turret> () != null) {
			previewObj.GetComponent<Turret> ().enabled = false;
		}

		foreach (MeshRenderer mr in previewObj.GetComponentsInChildren<MeshRenderer>()) {
			mr.material = previewMat;
		}

		btui.FillStats (currentID);
	}

	void RemovePreviewBuild(){
		//animate object leaving, for now destroy
		Destroy (previewObj);
	}


	#endregion
}
