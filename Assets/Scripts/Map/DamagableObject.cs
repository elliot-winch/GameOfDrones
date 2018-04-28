using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour {

	public float startingHealth;
	float health;

	public float CurrentHealth {
		get {
			return health;
		} set {

			 if (health > value) {
				Debug.LogWarning ("DamagableObject Warning: Should not be setting health by CurrentHealth is you mean to damage something in game");
			}

			health = value;
		}
	}

	protected virtual void Start(){
		health = startingHealth;
	}

	public virtual void Hit(Vector3 hitPoint, Vector3 from, float amount){

		health = Mathf.Max (0f, health - amount);

		if (health <= 0f) {
			
			Destroyed ();
		}
	}

	protected virtual void Destroyed(){
	}

	protected virtual void Update(){

	}
}
