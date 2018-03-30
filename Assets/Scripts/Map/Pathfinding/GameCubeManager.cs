using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCubeManager : MonoBehaviour {

	static GameCubeManager instance;

	ConceptualGrid grid;

	public static GameCubeManager Instance {
		get {
			return instance;
		}
	}

	public ConceptualGrid Grid {
		get {
			return grid;
		}
	}

	// Use this for initialization
	void Start () {
		if (instance != null) {
			Debug.LogError ("Error: Cannot have two GameCubeManagers");
		}

		instance = this;
	
		this.grid = new ConceptualGrid ();

		//Either spawn the cudes here with code

		//Or place by hand and find the cubes
		foreach (GameCube gc in transform.Find("GameCubes").GetComponentsInChildren<GameCube>()) {
			this.grid.AddCube (gc.transform.position, gc);
		}
	}
}

public class ConceptualGrid {

	static Vector3Int[] NeighbourDefinition = new Vector3Int[] {
		new Vector3Int(-1,  0,  0),
		new Vector3Int( 1,  0,  0),
		new Vector3Int( 0, -1,  0),
		new Vector3Int( 0,  1,  0),
		new Vector3Int( 0,  0, -1),
		new Vector3Int( 0,  0,  1),
	};

	Dictionary<int, Dictionary<int, Dictionary<int, GameCube>>> grid;

	public List<GameCube> AllCubes {
		get {
			List<GameCube> list = new List<GameCube> ();
				
			foreach(KeyValuePair<int, Dictionary<int, Dictionary<int, GameCube>>> kvx in this.grid){
				foreach(KeyValuePair<int, Dictionary<int, GameCube>> kvy in kvx.Value){
					foreach (KeyValuePair<int, GameCube> kvz in kvy.Value) {
						list.Add (kvz.Value);
					}
				}
			}

			return list;
		}
	}

	public ConceptualGrid(){

		this.grid = new Dictionary<int, Dictionary<int, Dictionary<int, GameCube>>> ();
	}

	public Vector3Int FloorVec3(Vector3 index){
		return new Vector3Int (Mathf.FloorToInt(index.x), Mathf.FloorToInt(index.y), Mathf.FloorToInt(index.z));
	}

	public void AddCube(Vector3 v, GameCube gc){

		Vector3Int index = FloorVec3 (v);

		if (this.grid.ContainsKey (index.x) == false) {
			this.grid [index.x] = new Dictionary<int, Dictionary<int, GameCube>> ();
		}

		if (this.grid [index.x].ContainsKey (index.y) == false) {
			this.grid [index.x] [index.y] = new Dictionary<int, GameCube> ();
		}

		if (this.grid [index.x][index.y].ContainsKey (index.z) == true) {
			Debug.LogWarning ("Warning: Replacing cube in ConceptualGrid");
		}

		this.grid [index.x] [index.y] [index.z] = gc;
	}

	public GameCube GetCubeAt(Vector3 v){

		Vector3Int index = FloorVec3 (v);

		if (this.grid.ContainsKey (index.x) == false) {
			return null;
		}

		if (this.grid [index.x].ContainsKey (index.y) == false) {
			return null;		
		}

		if (this.grid [index.x] [index.y].ContainsKey (index.z) == false) {
			return null;
		}

		return this.grid [index.x] [index.y] [index.z];
	}

	public List<GameCube> GetNeighbours(GameCube center){

		//Non - dynamic : conceptual grid is based on position, with one cube occupying one meter
		Vector3Int index = this.FloorVec3(center.Position);

		return GetNeighbours (center, index);
	}

	public List<GameCube> GetNeighbours(Vector3 v){
		GameCube center = this.GetCubeAt (v);

		return GetNeighbours (center, FloorVec3 (v));
	}

	private List<GameCube> GetNeighbours(GameCube center, Vector3Int index){

		if (center == null) {
			Debug.LogWarning ("Warning: Attempting to find neighbours of a non-existent GameCube");
			return null;
		}

		List<GameCube> n = new List<GameCube> ();

		foreach (Vector3Int vec in NeighbourDefinition) {

			GameCube gc = this.GetCubeAt (new Vector3 (index.x + vec.x, index.y + vec.y, index.z + vec.z));

			if (gc != null) {
				n.Add (gc);
			}
		}

		return n;
	}
}
