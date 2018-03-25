using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

	static GunManager instance;

	public static GunManager Instance {
		get {
			return instance;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one GunManager allowed");
		}

		instance = this;
	}
}
