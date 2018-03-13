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

	private bool firing;
	protected bool moving;

	public override void Hit (float amount)
	{

		base.Hit (amount);

	}

	protected override void Start(){
		base.Start ();

		transform.position = EnemyManager.Instance.StartCube.RandomPositionInBounds;
		currentCube = EnemyManager.Instance.StartCube;

		StartCoroutine (MoveTowards (EnemyManager.Instance.EnemyDestination));
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

	IEnumerator MoveTowards(GameCube t){
		moving = true;

		this.destination = t;

		Vector3 randomDest = t.RandomPositionInBounds;

		float dist = Vector3.Distance (currentCube.Position, randomDest);
		float movePercentage = 0f;

		while (movePercentage < 1f) {
			movePercentage += (moveSpeed * Time.deltaTime) / dist;

			transform.position = Vector3.Lerp (currentCube.Position, randomDest, movePercentage);

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
}
