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
	private bool movementPaused;
	private bool disengaging;

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

	public override void Hit (Vector3 hitDirection, float amount){

		Debug.Log (name + " hit for " + amount);

		base.Hit (hitDirection, amount);

	}

	protected override void Start(){
		base.Start ();
	}

	//Begin - like start but with parameters. Called by WaveManager
	public void Begin(GameCube startCube, GameCube target){
		//Pathfinding is managed here
		currentCube = startCube;

		pathToTarget = new AStarPath (startCube, target);

		StartCoroutine (Move ());

	}

	protected override void Update(){
		
		//Attacking
		if (firing == false) {
			if (currentTarget == null) {
				Collider[] cols = Physics.OverlapSphere (transform.position, attackRange, LayerMask.GetMask("Friendly"));

				if (cols.Length > 0) {
					//decide which enemy to shoot - currently we just pick the first target found
					currentTarget = cols [0].GetComponentInParent<DamagableObject> ();
					StartCoroutine (Fire (currentTarget));
					disengaging = false;
				} else {

					if (disengaging == false) {
						Disengage ();
						disengaging = true;
					}
				}
			} 
		}
	}

	IEnumerator Move(){

		if (pathToTarget.IsComplete == false) {
			Debug.LogError ("Enemy Error: Path could not be found to target!");
			yield break;
		}

		while (pathToTarget.IsNext()) {

			if (moving == false) {
				 StartCoroutine (MoveTowards (pathToTarget.GetNext ()));
			}

			yield return null;
		}
	}

	IEnumerator MoveTowards(GameCube t, Vector3? positionInCube = null){
		moving = true;

		this.destination = t;

		if (positionInCube.HasValue ) {
			destinationPosition = positionInCube.Value;
		} else {
			destinationPosition = t.RandomPositionInBounds;
		}

		Vector3 startPos = transform.position;

		float dist = Vector3.Distance (startPos, destinationPosition);
		float movePercentage = 0f;

		while (movePercentage < 1f) {

			if (movementPaused == false) {

				movePercentage += (moveSpeed * Time.deltaTime) / dist;

				transform.position = Vector3.Lerp (startPos, destinationPosition, movePercentage);
			}

			yield return null;

		}

		currentCube = t;

		moving = false;
	}

	public void PauseMovementTowardsTarget(bool pause){

		this.movementPaused = pause;

		//This will be true when the enemy has reached the target, so we will need to restart the coroutine to get the enemy back to where it was
		if (pause == false && pathToTarget.IsNext () == false) {
			StartCoroutine(MoveTowards(currentCube, destinationPosition));
		}
	}

	private IEnumerator Fire(DamagableObject target){
		firing = true;

		PreFire (target);

		yield return new WaitForSeconds(weaponChargeTime);

		if (target != null) {
			OnFire (target);
		} 

		currentTarget = null;

		firing = false;
	}


	protected override void Destroyed ()
	{
		WaveManager.Instance.CouldEndWave ();

		base.Destroyed();
	}

	//To be overriden
	protected virtual void OnFire(DamagableObject target){

	}

	protected virtual void PreFire(DamagableObject target){

	}

	//When there is no targt, a target is searched for and not found.
	protected virtual void Disengage(){

	}


}
