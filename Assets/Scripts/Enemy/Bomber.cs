using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Enemy {

	public float explodeTime; 

	static Dictionary<Bomber, GameCube> currentlyExploding;

	protected override void MoveToNext(){

		if (currentlyExploding == null) {
			//Lazy init
			currentlyExploding = new Dictionary<Bomber, GameCube> ();
		}

		if (currentlyExploding.ContainsValue (currentCube) == false) {
			foreach(GameCube gc in GameCubeManager.Instance.Grid.GetNeighbours (currentCube)) {
				if (gc.Occupying != null) {
					StartCoroutine(Explode ());

					currentlyExploding [this] = gc;
					return;
				}
			}
		}
			
		base.MoveToNext ();
	}
		
	IEnumerator Explode(){

		yield return new WaitForSeconds (explodeTime);

		base.Destroyed ();

		Debug.Log (Physics.OverlapSphere (transform.position, attackRange));

		foreach(Collider col in Physics.OverlapSphere (transform.position, attackRange)){

			Debug.Log (col);

			if (col.GetComponentInParent<DamagableObject> () != null && col.gameObject.tag != "Enemy") {

				col.GetComponentInParent<DamagableObject> ().Hit (col.transform.position, transform, attackDamage);
			}
		}

		currentlyExploding.Remove (this);

		Destroy (gameObject);
	}

}