using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	static EnemyManager instance;

	public GameObject enemyPrefab;
	public GameObject enemyTargetPrefab;
	public float secondsPerDrone = 8f;

	private GameCube enemyDestination;
	private GameCube startCube;

	public static EnemyManager Instance {
		get {
			return instance;
		}
	}

	public GameCube EnemyDestination {
		get {
			return enemyDestination;
		}
	}

	public GameCube StartCube {
		get {
			return startCube;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one EnemyManager allowed");
		}

		instance = this;

		startCube = GameObject.Find ("StartCube").GetComponent<GameCube> ();

		if (startCube == null) {
			Debug.LogWarning ("No starting cube set!");
			return;
		}

		enemyDestination = GameObject.Find ("TargetCube").GetComponent<GameCube>();

		if (enemyDestination == null) {
			Debug.LogWarning ("No enemy destination set!");
			return;
		}

		enemyDestination.Occupying = Instantiate (enemyTargetPrefab);
		enemyDestination.Locked = true;

		StartCoroutine (SpawnEnemies ());
	}
		
	IEnumerator SpawnEnemies(){

		while (true) {
			yield return new WaitForSeconds (secondsPerDrone);

			Instantiate (enemyPrefab, startCube.Position, Quaternion.identity, transform);
		}
	}

}
