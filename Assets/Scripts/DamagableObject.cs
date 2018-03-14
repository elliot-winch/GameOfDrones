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
		}
	}

	protected virtual void Start(){
		health = startingHealth;
	}

	public virtual void Hit(float amount){

		health = Mathf.Max (0f, health - amount);

		if (health > 0f) {

			Damaged (amount);
		} else {
			Destroyed ();
		}
	}

	protected virtual void Damaged(float amount){
		
	}

	protected virtual void Destroyed(){
		Destroy (gameObject);
	}

	protected virtual void Update(){

	}
}
