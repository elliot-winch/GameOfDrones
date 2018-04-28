using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/*
 * The Wave Manager concerns itself only with waves of enemies: what a wave consists of, and when each enemy in that wave is spawned.
 * 
 * WaveManagerEditor collects the public variable info in a nicely organised fashion
 */ 

public class WaveManager : MonoBehaviour {

	static WaveManager instance;

	public TextMeshPro[] waveText;

	//Collected in editor
	public float fastEnemySpeed;
	public float attackingEnemyRange;
	public Transform enemyParent;
	public Transform enemyDeadParent;

	List<ControlWheelSegment> defaultActions;

	[SerializeField]
	public List<GameCube> startCubes;
	[SerializeField]
	public List<Enemy> enemyPrefabs;

	[SerializeField]
	public List<Wave> data;


	private int currentWave = 0;
	private int enemiesLeftToSpawn = 0;

	private AudioSource waveStartAudio;
	private AudioSource waveEndAudio;


	public static WaveManager Instance {
		get {
			return instance;
		}
	}

	public int CurrentWave {
		get {
			return currentWave;
		}
		set {
			currentWave = value;

		}
	}

	public List<ControlWheelSegment> DefaultActions {
		get {
			return defaultActions;
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

		defaultActions = new List<ControlWheelSegment> ();

		waveStartAudio = GetComponents<AudioSource> ()[0];
		waveEndAudio = GetComponents<AudioSource> ()[1];

	}

	public void OnGameStart(){

		CurrentWave = 0;

		AddNewWaveButton ();

		foreach (TextMeshPro tm in waveText) {
			tm.text = "BUILD";
		}
	}

	public void OnGameEnd(){
		foreach (TextMeshPro tm in waveText) {
			tm.text = "";
		}
	}

	//for 2d debugging
	void Update(){
		if (Input.GetKeyDown (KeyCode.U)) {
			RunNextWave ();
		}

		if (Input.GetKeyDown (KeyCode.K)) {

			foreach (DamagableObject obj in enemyParent.GetComponentsInChildren<DamagableObject>()) {
				if(obj.GetComponent<DamagableObject>() != null){
					obj.GetComponent<DamagableObject> ().Hit (Vector3.zero, Vector3.zero, 10000);

				}
			}
		}
	}


	public void RunNextWave(){

		if (GameManager.Instance.GameRunning == false) {
			Debug.Log ("Cannot start wave if not in game");
			return;
		}

		Debug.Log ("Running next wave");
		if (enemyParent.childCount <= 0 && enemiesLeftToSpawn <= 0) {
			//UI
			string waveString = currentWave.ToString ();

			if (currentWave < 10) {
				waveString = "0" + waveString;
			}

			foreach (TextMeshPro tm in waveText) {
				tm.text = waveString;
			}
			//END UI

			Debug.Log ("Removing new wave button");
			RemoveNewWaveButton ();

			if (data.Count > currentWave) {

				StartCoroutine (RunNextWaveCoroutine ());
			}
		} else {
			Debug.Log ("Cannot start wave as wave is already running!");
		}
	}

	IEnumerator RunNextWaveCoroutine(){

		waveStartAudio.Play ();

		yield return new WaitForSeconds (waveStartAudio.clip.length);

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
		if (enemyParent.childCount <= 0 && enemiesLeftToSpawn <= 0) {
			Debug.Log("Cleared wave " + currentWave);

			CurrentWave++;

			AddNewWaveButton ();

			waveEndAudio.Play ();

			if (currentWave >= data.Count) {
				CurrentWave = 0;
			}

			foreach (TextMeshPro tm in waveText) {
				tm.text = "BUILD";
			}
		}
	}

	private void AddNewWaveButton(){

		ControlWheelSegment startWave = new ControlWheelSegment (
            name : "Start Wave",
            action: () => {
				WaveManager.Instance.RunNextWave ();
			}, 
	        icon: Resources.Load<Sprite> ("Icons/Drone Icon"),
			preferredPosition: ControlWheelSegment.PreferredPosition.Down
	    );


		//Currently equipped
		foreach (GameObject go in GrippedTool.CurrentlyHeldTools) {
			go.GetComponentInChildren<ControlWheel>().AddControlWheelAction ( startWave );
		}

		defaultActions.Add (startWave);
	}

	private void RemoveNewWaveButton(){

		defaultActions.Clear ();

		foreach (GameObject go in GrippedTool.CurrentlyHeldTools) {
			go.GetComponentInChildren<ControlWheel>().RemoveControlWheelAction ("Start Wave");
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