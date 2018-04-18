using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	GameObject currentMenu;

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
	}

	public void StartGame(){

		if (currentMenu != null) {
			Destroy (currentMenu);
		}

		WaveManager.Instance.OnGameStart ();
		EnemyPathManager.Instance.OnGameStart ();
		ResourceManager.Instance.OnGameStart ();

		//can place turrets
		gameRunning = true;
	}

	public void EndGame(){

		//Destory all towers
		foreach (GameCube gc in GameCubeManager.Instance.Grid.AllCubes) {
			gc.Occupying = null;
		}

		//Destroy all enemies
		foreach (Transform e in WaveManager.Instance.enemyParent) {
			Destroy (e.gameObject);
		}

		EnemyPathManager.Instance.RemoveAllEnemyPaths ();

		//Display start menu
		DisplayMenu (startMenu);

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
