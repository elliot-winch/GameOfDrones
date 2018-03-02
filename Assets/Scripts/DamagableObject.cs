using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour {

	public float startingHealth;
	float health;

	Action onDamaged;
	Action onDestroyed;

	public float CurrentHealth {
		get {
			return health;
		}
	}

	public void RegisterOnDamagedCallback(Action callback){
		onDamaged += callback;
	}

	public void RegisterOnDestroyCallback(Action callback){
		onDestroyed += callback;
	}

	void Start(){
		health = startingHealth;
	}

	public virtual void Hit(float amount){

		health = Mathf.Max (0f, health - amount);

		if (health > 0f) {

			if (onDamaged != null) {
				onDamaged ();
			}
		} else {
			if (onDestroyed != null) {
				onDestroyed ();
			}

			Destroy (gameObject);
		}
	}

}
