using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	static EnemyManager instance;

	//potentially make this an array
	public DamagableObject enemyTarget;

	public static EnemyManager Instance {
		get {
			return instance;
		}
	}

	void Start(){
		if (instance != null) {
			Debug.LogWarning ("Only one GameManager allowed");
		}

		instance = this;
	}
}
