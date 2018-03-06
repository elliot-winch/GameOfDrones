using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	static MapManager instance;

	public GameObject gameCubePrefab; 

	List<GameCube> cubes;

	//to be read in from actual file
	//string fileData = "-3.0,0.5,2.5:7,3,1";

	public static MapManager Instance {
		get {
			return instance;
		}
	}

	//Returns a copy of cubes!
	public List<GameCube> Cubes {
		get {
			return new List<GameCube>(cubes);
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one Map Manager allowed");
		}

		instance = this;

		//Transform cubeParent = transform.Find ("GameCubes");

		cubes = new List<GameCube> ();
		/*
		string[] data = fileData.Split ("\n".ToCharArray());

		for(int i = 0; i < data.Length; i++){

			string[] tripData = data [i].Split (":".ToCharArray());

			if (tripData.Length > 2 || tripData.Length == 0) {
				Debug.LogWarning ("Data on line: " + i + " is incorrect. Skipping...");
				continue;
			}

			string[] posData = tripData[0].Split (",".ToCharArray());

			if (posData.Length != 3) {
				Debug.LogWarning ("Position data on line: " + i + " is incorrect. Skipping...");
				continue;
			}

			try { 
				Vector3 pos = new Vector3 (float.Parse (posData [0]), float.Parse(posData [1]), float.Parse(posData [2]));

				GameObject cube = Instantiate(gameCubePrefab, pos, Quaternion.identity, cubeParent);
				cubes.Add(cube.GetComponent<GameCube>());

				if (tripData.Length == 2) {
					string[] extendData = tripData[1].Split(",".ToCharArray());

					if (extendData.Length != 3) {
						Debug.LogWarning ("Extension data on line: " + i + " is incorrect. Skipping...");
						continue;
					}

					Debug.Log(int.Parse (extendData [0]));
					Debug.Log(int.Parse (extendData [1]));
					Debug.Log(int.Parse (extendData [2]));

					cube.GetComponent<GameCube>().SpawnAdditonals(int.Parse (extendData [0]), int.Parse(extendData [1]),  int.Parse(extendData [2]));
				}
			} catch {
				Debug.LogWarning ("Failed to parse at line: " + i + ". Skipping...");
			}
		}*/
	}

	public void AddCube(GameCube g){
		cubes.Add (g);
	}
}
