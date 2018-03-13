using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamagableObject {

	public float attackRange = 1f;
	public float attackDamage = 1f;
	public float moveSpeed = 1f;
	public float weaponChargeTime = 0.2f;

	protected DamagableObject currentTarget;
	protected GameCube currentCube;
	protected GameCube destination;
	protected Vector3 destinationPosition;

	private bool firing;
	private bool moving;

	protected bool Firing {
		get {
			return firing;
		}
	}

	protected bool Moving {
		get {
			return moving;
		}
	}

	private AStarPath pathToTarget;

	public override void Hit (float amount){

		base.Hit (amount);

	}

	protected override void Start(){
		base.Start ();

		transform.position = EnemyManager.Instance.StartCube.RandomPositionInBounds;
		currentCube = EnemyManager.Instance.StartCube;

		pathToTarget = new AStarPath (currentCube, EnemyManager.Instance.EnemyDestination);

		StartCoroutine (Move ());

	}

	protected override void Update(){
		
		//Attacking
		Collider[] cols = Physics.OverlapSphere (transform.position, attackRange, LayerMask.GetMask("Friendly"));

		if (firing == false) {
			if (currentTarget == null) {
				if (cols.Length > 0) {
					//decide which enemy to shoot - currently we just pick the first target found
					currentTarget = cols [0].GetComponentInParent<DamagableObject> ();
					StartCoroutine(Fire (currentTarget));
				}
			} 
		}
	}

	IEnumerator Move(){

		while (pathToTarget.Length() > 0) {

			if (moving == false) {
				StartCoroutine (MoveTowards (pathToTarget.GetNext ()));
				PostMove ();

			}

			yield return null;
		}
	}

	IEnumerator MoveTowards(GameCube t){
		moving = true;

		this.destination = t;

		destinationPosition = t.RandomPositionInBounds;
		Vector3 startPos = transform.position;

		float dist = Vector3.Distance (startPos, destinationPosition);
		float movePercentage = 0f;

		while (movePercentage < 1f) {
			movePercentage += (moveSpeed * Time.deltaTime) / dist;

			transform.position = Vector3.Lerp (startPos, destinationPosition, movePercentage);

			yield return null;
		}

		currentCube = t;

		moving = false;
	}

	private IEnumerator Fire(DamagableObject target){
		firing = true;

		PreFire (target);

		yield return new WaitForSeconds (weaponChargeTime);

		OnFire(target);

		currentTarget = null;

		firing = false;
	}

	protected virtual void OnFire(DamagableObject target){

	}

	protected virtual void PreFire(DamagableObject target){

	}

	protected virtual void PostMove(){

	}
}
