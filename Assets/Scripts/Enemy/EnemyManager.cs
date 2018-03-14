using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	static EnemyManager instance;

	public Enemy[] enemyPrefabs;
	public GameObject enemyTargetPrefab;

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
		
	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one EnemyManager allowed");
		}

		instance = this;

		enemyDestination = GameObject.Find ("TargetCube").GetComponent<GameCube>();

		if (enemyDestination == null) {
			Debug.LogWarning ("No enemy destination set!");
			return;
		}

		enemyDestination.Occupying = Instantiate (enemyTargetPrefab);
		enemyDestination.Locked = true;
	}


}
