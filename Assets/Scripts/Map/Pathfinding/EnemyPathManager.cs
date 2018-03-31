using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour {

	static EnemyPathManager instance;

	public GameObject enemyTargetPrefab;
	public GameObject enemyPathUIPrefab;
	public GameCube enemyDestination;
	public Transform enemyUIParent;
	public float cubesBetweenPathUI = 5f; //the number of UI elements per cube


	private List<GameCube> startCubes;

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

		enemyDestination.Occupying = Instantiate (enemyTargetPrefab);
		enemyDestination.Locked = true;


		//ensure WaveManager starts before this in SEO
		startCubes = WaveManager.Instance.startCubes;

		foreach (GameCube gc in startCubes) {
			gc.Locked = true;
		}

		StartCoroutine (StartWithDelay (0.1f));
	}

	IEnumerator StartWithDelay(float delay){
		yield return new WaitForSeconds (delay);

		currentPaths = new Dictionary<GameCube, List<GameCube>> ();
		currentPathTrails = new Dictionary<GameCube, List<GameObject>> ();

		foreach (GameCube startCube in startCubes) {
			CalcEnemyPath (startCube);
		}
	}

	/*
	 * THere should always be a path the enemies can take from (each) start cube to
	 * the enemy destination
	 */ 
	public bool WouldYieldNoPaths(GameCube test){

		// set test cube to Mathf.Infinity
		float testMoveCost = test.MoveCost;
		test.MoveCost = Mathf.Infinity;

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

	#region Enemy Path UI
	Dictionary<GameCube, List<GameObject>> currentPathTrails;
	Dictionary<GameCube, List<GameCube>> currentPaths;

	public void ShouldRecalcPath(GameCube placedIn){

		//Since we change currentPaths in the foreach loop, we have to cache the paths as they are
		//so we can verify state

		Dictionary<GameCube, List<GameCube>> paths = new Dictionary<GameCube, List<GameCube>> ();

		foreach (KeyValuePair<GameCube, List<GameCube>> kvp in currentPaths) {
			paths [kvp.Key] = currentPaths [kvp.Key];
		}

		foreach (KeyValuePair<GameCube, List<GameCube>> path in paths) {
			if (path.Value.Contains (placedIn)) {
				CalcEnemyPath (path.Key);
			}
		}
	}

	public void CalcEnemyPath(GameCube startCube){

		RemoveEnemyPathUI (startCube);

		List<GameObject> pathTrails = new List<GameObject> ();
		List<GameCube> cachedPath = new List<GameCube> ();

		AStarPath path = new AStarPath (startCube, enemyDestination);

		if (path.IsComplete == false) {
			Debug.LogError ("Enemy Path Completely Blocked!");
		}

		cachedPath.Add (startCube);
		while (path.IsNext ()) {
			cachedPath.Add (path.GetNext ());
		}
			
		for (float i = 0; i < cachedPath.Count; i++) {
			//FIXME: uneven spacing due to casting
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
		
	public void RemoveEnemyPathUI(GameCube startCube){

		if (currentPathTrails.ContainsKey(startCube) && currentPathTrails[startCube] != null) {
			foreach (GameObject go in currentPathTrails[startCube]) {
				Destroy (go);
			}
		}
	}
	#endregion
}
