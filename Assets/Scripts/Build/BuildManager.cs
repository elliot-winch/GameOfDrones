using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

	static BuildManager instance;

	public int playerStartResources = 100;
	public KeyCode buildKey;
	public KeyCode putDownKey;

	public GameObject[] buildables;

	public GameObject previewObjPrefab;


	int playerResources;
	List<GameCube> cubes;
	List<IPlaceable> placeables;

	Transform cubeParent;

	public static BuildManager Instance {
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

			//change ui
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one BuildManager allowed");
		}

		instance = this;

		placeables = new List<IPlaceable> ();
		cubes = new List<GameCube> ();

		playerResources = playerStartResources;

	}

	public bool CanPlace(int id, GameCube gc){

		return gc.Locked == false && playerResources - buildables [id].GetComponent<IPlaceable>().Cost >= 0;
	}

	public void BuildPlaceable(int id, GameCube gc){

		if (CanPlace (id, gc)) {
			GameObject p = Instantiate (buildables [id]);

			//most of the spawning process is handled by the gamecube
			gc.Occupying = p;

			playerResources -= p.GetComponent<IPlaceable> ().Cost;

			placeables.Add (p.GetComponent<IPlaceable> ());
		}

	}

	public void RemovePlaceable(IPlaceable p){
		if (placeables.Contains (p)) {
			placeables.Remove (p);
		}
	}
}
