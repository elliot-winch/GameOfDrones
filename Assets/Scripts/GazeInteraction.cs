using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteraction : MonoBehaviour {

	public float delay = 1f;
	Action onGaze;
	Action onLookAway;

	Coroutine coroutine;

	public Action OnGaze {
		get {
			return onGaze;
		}
	}

	public Action OnLookAway {
		get {
			return onLookAway;
		}
	}

	public void RegisterOnGaze(Action callback){
		onGaze += callback;
		Debug.Log (gameObject.name + "Registered "  + onGaze);
	}

	public void RegisterOnLookAway(Action callback){
		onLookAway += callback;
	}

	public void LookAt(){
		coroutine = StartCoroutine (Gaze ());
	}

	IEnumerator Gaze(){

		Debug.Log ("Starting gaze");
		Debug.Log (onGaze);

		yield return new WaitForSeconds (delay);

		Debug.Log (onGaze);

		if (onGaze != null) {
			onGaze ();
		}
	}

	public void LookAway(){
		if (coroutine != null) {
			StopCoroutine (coroutine);
		}

		if (onLookAway != null) {
			onLookAway ();
		}
	}
}