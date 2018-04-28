using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

	static ResourceManager instance;

	public int playerStartResources = 100;
	[Range(0, 1)]
	public float reimburseRate = 1.0f;
	[HideInInspector]
	public BuildToolResourceDisplay btrd;

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
		private set {
			playerResources = value;

			if (btrd != null) {
				
				btrd.UpdateDisplay (value);
			}
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one BuildManager allowed");
		}

		instance = this;

		PlayerResources = playerStartResources;
	}

	public void OnGameStart(){
		PlayerResources = playerStartResources;
	}

	public bool CanSpend(int amount){
		return playerResources - amount >= 0;
	}

	public void Spend(int amount){

		if (CanSpend (amount)) {
			PlayerResources -= amount;
		} else {
			Debug.LogWarning ("Attempted to spend resources you don't have!");
		}

	}

	public void AddResources(int amount){

		PlayerResources += amount;
	}
}

