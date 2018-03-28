using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

	static WaveManager instance;

	public int[] enemiesPerWave; 

	int waveNumber = 0;
	List<List<IncomingEnemy>> waveInfo;
	private bool waveRunning;

	public static WaveManager Instance {
		get {
			return instance;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one WaveManager allowed");
		}
			
		instance = this;

		//this needs to be more intelligent
		GameCube startCube = GameObject.Find ("StartCube").GetComponent<GameCube> ();

		if (startCube == null) {
			Debug.LogWarning ("No starting cube set!");
			return;
		}

		waveInfo = new List<List<IncomingEnemy>> ();

		//read from file in future - or take arguments in inspector

		for (int i = 0; i < enemiesPerWave.Length; i++) {
			waveInfo.Add (new List<IncomingEnemy> ());

			for (int j = 0; j < enemiesPerWave [i]; j++) {
				waveInfo [i].Add (new IncomingEnemy (EnemyManager.Instance.enemyPrefabs [0], Mathf.Sqrt (j * 2f), startCube));
			}
		}

	}

	void Update(){
		//temp - will have proper key to start wave / start on a countdown
		//if (Input.GetKeyDown (KeyCode.Return)) {
		//	StartWave ();
		//}
	}

	void StartWave(){
		if (waveRunning == false) {
			StartCoroutine (WaveCoroutine (waveNumber));

			waveNumber++;
		}
	}

	IEnumerator WaveCoroutine(int num){
		waveRunning = true;

		List<IncomingEnemy> wave = waveInfo [num];

		while (wave.Count > 0) {
			for (int i = wave.Count - 1; i >= 0; i--) {
				IncomingEnemy ie = wave [i];

				ie.timeDelay -= Time.deltaTime;
				wave [i] = ie;

				if (ie.timeDelay <= 0f) {
					Instantiate(ie.Enemy, ie.StartCube.RandomPositionInBounds, Quaternion.identity, transform);
					wave.RemoveAt (i);
				}
			}

			yield return null;
		}

		waveRunning = false;
	}
}

struct IncomingEnemy {

	Enemy enemy;
	public float timeDelay;
	GameCube startCube;

	public Enemy Enemy {
		get {
			return enemy;
		}
	}

	public GameCube StartCube {
		get {
			return startCube;
		}
	}

	public IncomingEnemy(Enemy enemy, float timeDelay, GameCube startCube){

		this.enemy = enemy;
		this.timeDelay = timeDelay;
		this.startCube = startCube;
	}
}