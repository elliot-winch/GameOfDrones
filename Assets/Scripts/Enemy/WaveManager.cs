using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Wave Manager concerns itself only with waves of enemies: what a wave consists of, and when each enemy in that wave is spawned.
 * 
 * WaveManagerEditor collects the public variable info in a nicely organised fashion
 */ 

public class WaveManager : MonoBehaviour {

	static WaveManager instance;

	//Collected in editor
	public float fastEnemySpeed;
	public float attackingEnemyRange;
	public Transform enemyParent;

	[SerializeField]
	public List<GameCube> startCubes;
	[SerializeField]
	public List<Enemy> enemyPrefabs;

	[SerializeField]
	public List<Wave> data;


	private int currentWave = 0;
	private int enemiesLeftToSpawn = 0;

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

		if (startCubes.Count == 0) {
			Debug.LogWarning ("No starting cubes set!");
			return;
		}
	}

	public void OnGameStart(){

		currentWave = 0;

		AddNewWaveButton ();
	}

	//for 2d debugging
	void Update(){
		if (Input.GetKeyDown (KeyCode.U)) {
			RunNextWave ();
		}
	}


	public void RunNextWave(){

		if (GameManager.Instance.GameRunning == false) {
			Debug.Log ("Cannot start wave if not in game");
			return;
		}


		if (enemyParent.childCount <= 0 && enemiesLeftToSpawn <= 0) {

			RemoveNewWaveButton ();

			if (data.Count > currentWave) {

				enemiesLeftToSpawn = 0;

				foreach (EnemyGroup g in data[currentWave].groups) {

					//NB an enemy cannot be fast and attacking!

					if (g.enemy == EnemyType.Drone || g.enemy == EnemyType.Triple) {

						List<int> allEnemyIndicies = new List<int> ();
						List<int> fastEnemyIndicies = new List<int> ();
						List<int> attackingEnemyIndicies = new List<int> ();
						for (int i = 0; i < g.number; i++) {
							allEnemyIndicies.Add (i);
						}

						for (int i = 0; i < g.numFastEnemies; i++) {
							if (allEnemyIndicies.Count <= 0) {
								Debug.Log ("WaveManager Warning: Run out of enemies to randomly allocate 'increased speed' to");
							}

							int r = Random.Range (0, allEnemyIndicies.Count);
							fastEnemyIndicies.Add (allEnemyIndicies [r]);
							allEnemyIndicies.RemoveAt (r);
						}

						for (int i = 0; i < g.numAttackingEnemies; i++) {
							if (allEnemyIndicies.Count <= 0) {
								Debug.Log ("WaveManager Warning: Run out of enemies to randomly allocate 'increased range' to");
							}

							int r = Random.Range (0, allEnemyIndicies.Count);
							attackingEnemyIndicies.Add (allEnemyIndicies [r]);
							allEnemyIndicies.RemoveAt (r);
						}

						enemiesLeftToSpawn += g.number;
						StartCoroutine (GroupCoroutine (g, fastEnemyIndicies, attackingEnemyIndicies));
					} else {
						StartCoroutine (GroupCoroutine (g, new List<int>(), new List<int>()));
					}
				}
			}
		} else {
			Debug.Log ("Cannot start wave as wave is already running!");
		}
	}
		
	IEnumerator GroupCoroutine(EnemyGroup group, List<int> randomFastIndicies, List<int> randomAttackingIndicies){

		yield return new WaitForSeconds (group.initialDelay);

		for (int i = 0; i < group.number; i++) {
	
			yield return new WaitForSeconds (group.timeDelay);

			//spawn enemy
			GameObject enemyObj = Instantiate(enemyPrefabs[(int)group.enemy].gameObject, enemyParent);

			enemyObj.transform.position = startCubes [group.startCubeIndex].RandomPositionInBounds;

			Enemy e = enemyObj.GetComponent<Enemy> ();

			if (randomFastIndicies.Contains (i)) {
				e.moveSpeed = this.fastEnemySpeed;
			}

			if (randomAttackingIndicies.Contains (i)) {
				e.attackRange = this.attackingEnemyRange;
			}

			//start pathfinding
			e.Begin (startCubes [group.startCubeIndex]);

			EnemyPathManager.Instance.AddEnemyToPathManager (e);

			enemiesLeftToSpawn--;
		}
	}

	public void CouldEndWave(){
		/*
			 * Why does this check to see if there is one enemy left rather than zero? 
			 * Good question!
			 * When we check to see if the wave is over, the enemy which is about to be destoryed is checking.
			 * This means that enemy is still childed to the enemyParent, so there will be one (but only one)
			 * enemy to check for
			 */ 
		if (enemyParent.childCount <= 1 && enemiesLeftToSpawn <= 0) {
			Debug.Log("Cleared wave " + currentWave);

			currentWave++;

			AddNewWaveButton ();

			if (currentWave >= data.Count) {
				currentWave = 0;
			}

			//some kinda of visual i guess
		}
	}

	private void AddNewWaveButton(){

		ControlWheelSegment startWave = new ControlWheelSegment (
            name : "Start Wave",
            action: () => {
				WaveManager.Instance.RunNextWave ();
			}, 
	        icon: Resources.Load<Sprite> ("Icons/Drone Icon")
	    );

		//Wheels in the future
		ControlWheel.RegisterOnCreate ((self) => {
			self.AddControlWheelAction( startWave );
		});

		//Currently equipped
		foreach (ControlWheel cw in Resources.FindObjectsOfTypeAll<ControlWheel> ()) {
			cw.AddControlWheelAction ( startWave );
		}
	}

	private void RemoveNewWaveButton(){

		ControlWheel.ResetOnCreate ();

		foreach (ControlWheel cw in Resources.FindObjectsOfTypeAll<ControlWheel> ()) {
			cw.RemoveControlWheelAction ("Start Wave");
		}
	}
}

[System.Serializable]
public class Wave {
	public List<EnemyGroup> groups;
}

[System.Serializable]
public class EnemyGroup {
	public EnemyType enemy;
	public int number;
	public float timeDelay;
	public float initialDelay;
	public int startCubeIndex;
	public int numFastEnemies;
	public int numAttackingEnemies;

	/*
	public EnemyGroup(){

		this.enemy = EnemyType.Drone;
		this.number = 1;
		this.timeDelay = 0f;
		this.initialDelay = 0f;
		this.startCubeIndex = 0;
	}

	public EnemyGroup(EnemyType enemy, int number, float timeDelay, float initialDelay, int startCubeIndex){

		this.enemy = enemy;
		this.number = number;
		this.timeDelay = timeDelay;
		this.initialDelay = initialDelay;
		this.startCubeIndex = startCubeIndex;
	}*/
}

public enum EnemyType {
	Drone,
	Triple,
	Bomber
}