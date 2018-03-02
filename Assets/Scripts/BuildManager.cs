using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

	static BuildManager instance;

	public KeyCode buildKey;
	public KeyCode putDownKey;

	public GameObject[] buildables;

	List<GameCube> cubes;
	List<IPlaceable> placeables;

	Transform cubeParent;

	public static BuildManager Instance {
		get {
			return instance;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one BuildManager allowed");
		}

		instance = this;

		placeables = new List<IPlaceable> ();
		cubes = new List<GameCube> ();

	}

	public void AddPlaceable(IPlaceable p){
		placeables.Add (p);
	}

	public void RemovePlaceable(IPlaceable p){
		if (placeables.Contains (p)) {
			placeables.Remove (p);
		}
	}
}
