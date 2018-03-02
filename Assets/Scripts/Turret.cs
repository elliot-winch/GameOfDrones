using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : DamagableObject, IPlaceable {

	public float range;
	public float damage;

	private GameCube cube;

	//should this be enemy or damagable object?
	private Enemy currentTarget;
	private Action onShoot;

	public GameCube Cube {
		get {
			return cube;
		} set {
			this.cube = value;

			transform.SetParent (cube.transform);
			transform.position = this.cube.Position;
		}
	}

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	void Update(){

		Collider[] cols = Physics.OverlapSphere (transform.position, range, LayerMask.GetMask("Enemy"));

		if (currentTarget == null) {
			if (cols.Length > 0) {

				//decide which enemy to shoot

				cols [0].GetComponent<Enemy> ().Hit (10);

			}
		} else {

		}
	}

	void Shoot(){

		onShoot ();

	}
}
