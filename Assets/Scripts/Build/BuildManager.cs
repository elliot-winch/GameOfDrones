using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour {

	static BuildManager instance;

	public int playerStartResources = 100;

	public GameObject[] buildables;

	public BuildTool buildTool;
	public Canvas placeableStatsCanvas;
	private Text resourceDisplay;

	int playerResources;
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

			resourceDisplay.text = playerResources.ToString ();
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one BuildManager allowed");
		}

		instance = this;

		placeables = new List<IPlaceable> ();

		//this may need to change if we add more ui text elements to the build tool at initialisation
		resourceDisplay = buildTool.GetComponentInChildren<Text> ();

		PlayerResources = playerStartResources;


	}

	public bool CanPlace(int id, GameCube gc){

		return gc.Locked == false && playerResources - buildables [id].GetComponent<IPlaceable>().Cost >= 0;
	}

	public void BuildPlaceable(int id, GameCube gc){

		if (CanPlace (id, gc)) {
			GameObject prefab = buildables [id];
			GameObject p = Instantiate (prefab);

			//most of the spawning process is handled by the gamecube
			gc.Occupying = p;

			PlayerResources -= p.GetComponent<IPlaceable> ().Cost;

			placeables.Add (p.GetComponent<IPlaceable> ());

			if (prefab.GetComponent<GazeInteraction> () != null) {
				GazeInteraction gi = p.GetComponent<GazeInteraction> ();
				GazeInteraction toCopy = prefab.GetComponent<GazeInteraction> ();

				gi.RegisterOnGaze (toCopy.OnGaze);
				gi.RegisterOnLookAway (toCopy.OnLookAway);
				gi.delay = toCopy.delay;
			}
		}

	}

	public void RemovePlaceable(IPlaceable p){
		if (placeables.Contains (p)) {
			placeables.Remove (p);
		}
	}
}
