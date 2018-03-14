using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Turns out there are built in Unity Time variables that are more robust (becuase the affect physics too)
 * This is the fully feautred class. BulletTimeManager will abstract over the Unity Time variables 
public class TimeManager : MonoBehaviour {

	static TimeManager instance;

	public float slowMoFactor = 0.5f;

	float modifiedDeltaTime;
	float currentTimeFactor;



	public static TimeManager Instance {
		get {
			return instance;
		}
	}

	public float ModifiedDeltaTime {
		get {
			return modifiedDeltaTime;
		}
	}

	void Start(){

		if (instance != null) {
			Debug.LogError ("Error: Cannot have two time managers");
		}

		instance = this;

		EnterSlowMo ();
	}

	void Update(){
		modifiedDeltaTime = currentTimeFactor * Time.deltaTime;

	}

	public void EnterSlowMo(){
		currentTimeFactor = slowMoFactor;
	}

	public void DepartSlowMo(){
		currentTimeFactor = 1f;
	}

	//Replaces yeild retun new WaitForSeconds
	public Coroutine WaitForModifiedSeconds(float realSeconds){
		return StartCoroutine (WaitForModifiedSecondsCoroutine (realSeconds));
	}
		
	IEnumerator WaitForModifiedSecondsCoroutine(float realSeconds){

		float gameTime = 0f;

		while (gameTime < realSeconds) {
			gameTime += modifiedDeltaTime;

			yield return null;
		}
	}
}
*/
