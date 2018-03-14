using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour {

	static BulletTimeManager instance;

	public float sloMoFactor = 0.4f;

	public static BulletTimeManager Instance {
		get {
			return instance;
		}
	}

	void Start(){

		if (instance != null) {
			Debug.LogError ("Error: Cannot have two BulletTimeManagers");
		}

		instance = this;

	}

	void EnterSlowMo(){
		Time.timeScale = sloMoFactor;
	}

	void ExitSlowMo(){
		Time.timeScale = 1f;

	}
}
