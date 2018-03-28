using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

	static ResourceManager instance;

	public int playerStartResources = 100;

	int playerResources;

	public static ResourceManager Instance {
		get {
			return instance;
		}
	}

	public int PlayerResources {
		get {
			return playerResources;
		}
		set {
			playerResources = value;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one BuildManager allowed");
		}

		instance = this;

		PlayerResources = playerStartResources;

	}

}
