using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour {

	static EnemyPathManager instance;

	public GameObject enemyTargetPrefab;
	public GameObject enemyPathUIPrefab;
	public GameCube enemyDestination;
	public Transform enemyUIParent;

	private List<GameCube> startCubes;

	Dictionary<GameCube, List<GameCube>> currentPaths;
	Dictionary<GameCube, List<Enemy>> currentEnemiesByPath;
	Dictionary<GameCube, List<GameObject>> currentPathTrails;

	public static EnemyPathManager Instance {
		get {
			return instance;
		}
	}

	public GameCube EnemyDestination {
		get {
			return enemyDestination;
		}
	}
		
	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one EnemyManager allowed");
		}

		instance = this;

		if (enemyDestination == null) {
			Debug.LogWarning ("No enemy destination set!");
			return;
		}
	}

	public void OnGameStart (){
		enemyDestination.Occupying = Instantiate (enemyTargetPrefab);
		enemyDestination.Locked = true;

		startCubes = WaveManager.Instance.startCubes;

		foreach (GameCube gc in startCubes) {
			gc.Locked = true;
		}

		currentPaths = new Dictionary<GameCube, List<GameCube>> ();
		currentPathTrails = new Dictionary<GameCube, List<GameObject>> ();
		currentEnemiesByPath = new Dictionary<GameCube, List<Enemy>> ();

		foreach (GameCube startCube in startCubes) {
			CalcEnemyPath (startCube);
		}
	}

	/*
	 * THere should always be a path the enemies can take from (each) start cube to
	 * the enemy destination
	 */ 
	public bool WouldYieldNoPaths(GameCube test, float newMoveCost){

		// set test cube to Mathf.Infinity
		float testMoveCost = test.MoveCost;
		test.MoveCost = newMoveCost;

		foreach (GameCube start in startCubes) {

			AStarPath path = new AStarPath (start, enemyDestination);

			if (path.IsComplete == false) {
				test.MoveCost = testMoveCost;

				return false;
			}
		}

		test.MoveCost = testMoveCost;

		//there is at least one path for each start cube
		return true;

	}

	public void AddEnemyToPathManager(Enemy e){
		if (this.currentEnemiesByPath.ContainsKey (e.StartCube) == false) {
			this.currentEnemiesByPath [e.StartCube] = new List<Enemy> ();
		}

		this.currentEnemiesByPath [e.StartCube].Add (e);

		e.SetPath (currentPaths [e.StartCube]);

	}

	//just so we dont try to access a destroyed enemy
	public void RemoveEnemyFromPathManager(Enemy e){

		if (this.currentEnemiesByPath.ContainsKey (e.StartCube)) {
			if (this.currentEnemiesByPath [e.StartCube].Contains (e)) {
				this.currentEnemiesByPath [e.StartCube].Remove (e);
			}
		}
	}


	#region Enemy Path UI
	//when we add a new block, does it block the current path?
	public void ShouldRecalcPathBlocked(GameCube placedIn){

		//Since we change currentPaths in the foreach loop, we have to cache the paths as they are
		//so we can verify state

		Dictionary<GameCube, List<GameCube>> paths = new Dictionary<GameCube, List<GameCube>> ();

		foreach (KeyValuePair<GameCube, List<GameCube>> kvp in currentPaths) {
			paths [kvp.Key] = currentPaths [kvp.Key];
		}

		foreach (KeyValuePair<GameCube, List<GameCube>> path in paths) {
			if (path.Value.Contains (placedIn)) {
				CalcEnemyPath (path.Key);

				foreach (KeyValuePair<GameCube, List<Enemy>> enemies in currentEnemiesByPath) {
					foreach (Enemy e in enemies.Value) {
						e.SetPath (currentPaths [path.Key]);
					}
				}
			}
		}
	}

	//When we delete something, does it open a shorter path?
	public void ShouldRecalcPathRemoved(){

		Debug.Log ("Called");

		Dictionary<GameCube, List<GameCube>> paths = new Dictionary<GameCube, List<GameCube>> ();

		foreach (KeyValuePair<GameCube, List<GameCube>> kvp in currentPaths) {
			paths [kvp.Key] = currentPaths [kvp.Key];
		}

		foreach (KeyValuePair<GameCube, List<GameCube>> path in paths) {
			AStarPath newPath = new AStarPath(path.Key, enemyDestination);

			if (newPath.Length() < path.Value.Count) {
				CalcEnemyPath (path.Key, newPath);
			}
		}
	}

	public void RemoveAllEnemyPaths(){
		foreach (KeyValuePair<GameCube, List<GameCube>> path in currentPaths) {
			RemoveEnemyPathUI (path.Key);
		}
	}

	void CalcEnemyPath(GameCube startCube, AStarPath suppliedPath = null){

		RemoveEnemyPathUI (startCube);

		List<GameObject> pathTrails = new List<GameObject> ();
		List<GameCube> cachedPath = new List<GameCube> ();

		AStarPath path = (suppliedPath != null) ? suppliedPath : new AStarPath (startCube, enemyDestination);

		if (path.IsComplete == false) {
			Debug.LogError ("Enemy Path Completely Blocked!");
		}

		cachedPath.Add (startCube);
		while (path.IsNext ()) {
			cachedPath.Add (path.GetNext ());
		}
			
		for (float i = 0; i < cachedPath.Count; i++) {
			pathTrails.Add (SpawnEnemyPathUI (cachedPath, (int) i, pathTrails));
		}

		currentPaths [startCube] = cachedPath;
		currentPathTrails [startCube] = pathTrails;
	}

	public GameObject SpawnEnemyPathUI(List<GameCube> path, int index, List<GameObject> allTrails){

		GameObject pathUIObj = Instantiate (enemyPathUIPrefab, path[index].transform.position , Quaternion.identity, enemyUIParent);
		pathUIObj.name = "Enemy Path UI";

		pathUIObj.GetComponent<EnemyPathTrail> ().StartPath (path, index, allTrails);
	
		return pathUIObj;
	}
		
	void RemoveEnemyPathUI(GameCube startCube){

		if (currentPathTrails.ContainsKey(startCube) && currentPathTrails[startCube] != null) {
			foreach (GameObject go in currentPathTrails[startCube]) {
				Destroy (go);
			}
		}
	}
	#endregion
}
