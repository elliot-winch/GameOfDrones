using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour {

	static EnemyPathManager instance;

	public GameObject enemyTargetPrefab;
	public GameObject enemyPathUIPrefab;

	private GameCube enemyDestination;
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

		//should be set in inspector
		enemyDestination = GameObject.Find ("TargetCube").GetComponent<GameCube>();

		if (enemyDestination == null) {
			Debug.LogWarning ("No enemy destination set!");
			return;
		}

		enemyDestination.Occupying = Instantiate (enemyTargetPrefab);
		enemyDestination.Locked = true;


		//ensure WaveManager starts before this in SEO
		startCubes = WaveManager.Instance.startCubes;
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

				return true;
			}
		}

		test.MoveCost = testMoveCost;

		//there is at least one path for each start cube
		return false;

	}

	#region Enemy Path UI
	List<GameObject> uiObjs;
	public void DisplayEnemyPathUI(){

		RemoveEnemyPathUI ();

		foreach (GameCube start in startCubes) {

			AStarPath path = new AStarPath (start, enemyDestination);

			if (path.IsComplete == false) {
				Debug.LogError ("Enemy Path Completely Blocked!");
			}

			GameCube current = path.GetNext();
			GameCube next = path.GetNext ();

			while (path.IsNext ()) {

				GameObject pathUIObj = Instantiate (enemyPathUIPrefab, current.transform);
				pathUIObj.name = "Enemy Path UI";

				pathUIObj.transform.position = current.transform.position;

				uiObjs.Add (pathUIObj);

				LineRenderer lr = pathUIObj.GetComponent<LineRenderer> ();

				lr.useWorldSpace = true;

				lr.SetPositions (new Vector3[] {
					current.transform.position,
					next.transform.position
				});

				current = next;
				next = path.GetNext ();
			}
		}
	}

	public void RemoveEnemyPathUI(){

		if(uiObjs != null){
			foreach (GameObject go in uiObjs) {
				Destroy (go);
			}
		}

		uiObjs = new List<GameObject> ();
	}
	#endregion
}
