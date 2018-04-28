using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Enemy {

	private AudioSource chargeAS;
	private AudioSource explodeAS;

	private Coroutine exploding;

	static Dictionary<Bomber, GameCube> currentlyExploding;

	protected override void MoveToNext(){

		if (currentlyExploding == null) {
			//Lazy init
			currentlyExploding = new Dictionary<Bomber, GameCube> ();
		}

		if (currentlyExploding.ContainsValue (currentCube) == false) {
			foreach(GameCube gc in GameCubeManager.Instance.Grid.GetNeighbours (currentCube)) {
				if (gc.Occupying != null) {
					if (exploding == null) {
						exploding = StartCoroutine (Explode ());
					}

					LookAt (gc.Occupying.transform.position);

					currentlyExploding [this] = gc;
					return;
				}
			}
		}

		AudioSource[] sources = GetComponents<AudioSource> ();
		chargeAS = sources [0];
		explodeAS = sources [1];
			
		base.MoveToNext ();

		LookAt (destinationPosition);
	}

	protected override void Destroyed ()
	{
		if (exploding == null) {
			
			exploding = StartCoroutine (Explode ());
		}
	}
		
	IEnumerator Explode(){

		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}

		chargeAS.Play ();

		yield return new WaitForSeconds (weaponChargeTime);
			
		base.Destroyed ();

		foreach(Collider col in Physics.OverlapSphere (transform.position, attackRange)){

			if (col.GetComponentInParent<DamagableObject> () != null && col.gameObject.tag != "Enemy") {

				col.GetComponentInParent<DamagableObject> ().Hit (col.transform.position, transform.position, attackDamage);
			}
		}

		chargeAS.Stop ();
		explodeAS.Play ();

		currentlyExploding.Remove (this);

		//Hide...
		transform.GetChild (0).gameObject.SetActive (false);

		//until sound is played
		yield return new WaitForSeconds (explodeAS.clip.length);

		Destroy (gameObject);
	}

}