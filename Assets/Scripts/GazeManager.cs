using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.XR;

/*
 * This is a manager class for Gaze Interactions.
 * 
 * Attach this componenet to the main camera, as this is where the rays are shot out from
 * 
 * We call LookAt() only once, not every frame!
 */ 

public class GazeManager : MonoBehaviour {

	static GazeManager instance;

	public static GazeManager Instance {
		get {
			return instance;
		}
	}

	//Assiging onGazes to prefabs here

	//This is for looking at an bject and its stats appearing over your hand
	Canvas statsCanvas;
	void Start(){

		if (instance != null) {
			Debug.LogError ("Error: Cannot have two Gaze Managers");
		}

		instance = this;

		//for testing, though checking the hand exists is a good idea
		GameObject playerVRObject = GameObject.Find("Player").transform.Find ("SteamVRObjects").gameObject;
		Transform hand;

		if (playerVRObject.activeSelf == true) {
			hand = playerVRObject.transform.Find ("Hand1");
		} else {
			hand = GameObject.Find ("Player").transform.Find ("NoSteamVRFallbackObjects").Find("FallbackHand");

		}
			

		foreach (GameObject go in BuildManager.Instance.buildables) {
			GazeInteraction gi = go.GetComponent<GazeInteraction> ();

			if(gi != null){

				assignFunctions (gi, go.GetComponent<IPlaceable> (), hand);

			}
		}
	}

	//Helper for start
	void assignFunctions(GazeInteraction gi, IPlaceable place, Transform hand){

		Debug.Log (gi);
		gi.delay = 1f;

		gi.RegisterOnGaze ( () => {

			Debug.Log("Gazed at");

			statsCanvas = Instantiate(BuildManager.Instance.placeableStatsCanvas, Vector3.zero, Quaternion.identity, hand);

			statsCanvas.transform.localPosition = new Vector3(0f, 0.2f, 0.1f);

			statsCanvas.GetComponent<StatsCanvas>().FillStats(place);
				
		});

		gi.RegisterOnLookAway (() => {
			Debug.Log("Gazed away");
			Destroy(statsCanvas.gameObject);
		});

	}
		

	GazeInteraction savedLookedAt;
	void Update(){

		RaycastHit hitInfo;

		Debug.DrawRay (transform.position, transform.forward, Color.red, 3f);

		//if the rays hits any gameobject
		if (Physics.Raycast (transform.position, transform.forward, out hitInfo, 20f, LayerMask.GetMask("Friendly"))) {

			//if the ray hits an object with an associated gaze interaction
			if (hitInfo.transform.GetComponentInParent<GazeInteraction> () != null) {
				Debug.Log ("Is interactable");

				//store the gaze interaction component of this frame
				GazeInteraction currentlyLookedAt = hitInfo.transform.GetComponentInParent<GazeInteraction> ();

				//if the gaze interaction is not the same as the one looked at last frame 
				if (savedLookedAt != currentlyLookedAt) {

					//look away form the other gaze interaction if it was not null
					if (savedLookedAt != null) {
						savedLookedAt.LookAway ();
					}
					//look at the current object
					currentlyLookedAt.LookAt ();
				}

				//discard the previous frame's gaze interaction and save this one
				savedLookedAt = currentlyLookedAt;

				return;
			}
		}

		//if we were looking at a gaze interaction in the last frame, but now we are not, we need to look away
		if (savedLookedAt != null) {
			savedLookedAt.LookAway ();
			//clear the saved gaze interaction, since we only want to call saved look at once
			savedLookedAt = null;
		}
	}
}

