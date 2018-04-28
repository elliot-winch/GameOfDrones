using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	static GameManager instance;

	public static GameManager Instance {
		get {
			return instance;
		}
	}

	public Vector3 menuPos;
	public Vector3 menuRotation;
	public GameObject startMenu;
	public GameObject reviewMenu;

	GameObject currentMenu;

	private AudioSource endGameSource;

	bool gameRunning;

	public bool GameRunning {
		get {
			return gameRunning;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogError ("Cannot have two GameManagers");
		}

		instance = this;

		DisplayMenu (startMenu);

		gameRunning = false;

		//third AS is game end
		endGameSource = GetComponents<AudioSource> () [2];
	}


	// For 2D debug
	void Update(){

		if (Input.GetKeyDown (KeyCode.B) && gameRunning == false) {
			GameManager.Instance.StartGame ();
		}
	}

	public void StartGame(){

		if (currentMenu != null) {
			Destroy (currentMenu);
		}

		EnemyPathManager.Instance.OnGameStart ();
		WaveManager.Instance.OnGameStart ();
		ResourceManager.Instance.OnGameStart ();

		//can place turrets
		gameRunning = true;
	}

	public void EndGame(){

		endGameSource.Play ();

		//Destory all towers
		foreach (GameCube gc in GameCubeManager.Instance.Grid.AllCubes) {
			gc.Locked = false;
			gc.Occupying = null;
		}

		//Destroy all enemies
		foreach (Transform e in WaveManager.Instance.enemyParent) {
			Destroy (e.gameObject);
		}

		EnemyPathManager.Instance.RemoveAllEnemyPaths ();
		WaveManager.Instance.OnGameEnd ();


		DisplayMenu (reviewMenu);
		currentMenu.transform.Find ("ScoreText").GetComponent<Text> ().text = "Score: " + ResourceManager.Instance.PlayerResources;

		//means you cannot place turrets
		gameRunning = false;
	}

	public void DisplayMenu(GameObject menu){

		if (currentMenu != null) {
			Destroy (currentMenu);
		}

		currentMenu = Instantiate (menu);
		currentMenu.transform.position = menuPos;
		currentMenu.transform.rotation = Quaternion.Euler(menuRotation);

	}

	public void Quit(){
		Application.Quit ();
	}
}
