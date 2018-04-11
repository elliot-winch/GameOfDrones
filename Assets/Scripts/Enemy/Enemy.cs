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
	private bool disengaging;

	private GameCube startCube;
	//to be set by manager depending on the start cube assigned in begin
	private List<GameCube> pathToTarget;
	private int currentPathIndex;
	private Coroutine moveCoroutine;

	protected bool Firing {
		get {
			return firing;
		}
	}

	public GameCube StartCube {
		get {
			return startCube;
		}
	}

	public override void Hit (Vector3 hitPoint, Vector3 hitDirection, float amount){

		base.Hit (hitPoint, hitDirection, amount);

	}

	protected override void Start(){
		base.Start ();
	}

	//Begin - like start but with parameters. Called by WaveManager
	public void Begin(GameCube startCube){
		//Pathfinding is managed here
		this.startCube = startCube;
		this.currentCube = startCube;

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

	public void SetPath(List<GameCube> path){
		Debug.Log ("Setting path");

		this.pathToTarget = path;

		int currentCubeIndex = FindPathIndex (this.currentCube, path);

		if (currentCubeIndex >= 0) {
			this.currentPathIndex = currentCubeIndex;
			MoveToNext ();
			return;
		}

		List<GameCube> openSet = new List<GameCube> () {
			currentCube
		};

		while (openSet.Count > 0) {

			List<GameCube> snapshot = new List<GameCube> ();

			foreach (GameCube gc in openSet) {
				snapshot.Add (gc);
			}

			foreach (GameCube gc in snapshot) {
				openSet.Remove (gc);

				foreach (GameCube n in GameCubeManager.Instance.Grid.GetNeighbours (gc)) {

					int posIndex = FindPathIndex (n, path);

					if (posIndex >= 0) {
						this.currentPathIndex = posIndex;
						MoveToNext ();
						return;
					} else {
						openSet.Add (n);
					}
				}
			}
		}
	}

	//Helper for SetPath
	int FindPathIndex(GameCube gc, List<GameCube> path){
		for (int i = 0; i < path.Count; i++) {
			if (path [i] == gc) {
				return i;
			}
		}
		return -1;
	}


	void MoveToNext(){

		if (pathToTarget == null) {
			Debug.LogError ("Enemy Error: Path provided was null");
			return;
		}

		if (currentPathIndex + 1 >= pathToTarget.Count) {
			Debug.Log ("Enemy has reached end of path");
			return;
		}


		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}
			
		currentPathIndex++;
		moveCoroutine = StartCoroutine (MoveTowards (pathToTarget[currentPathIndex]));

	}

	IEnumerator MoveTowards(GameCube t, Vector3? positionInCube = null){

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


			movePercentage += (moveSpeed * Time.deltaTime) / dist;

			transform.position = Vector3.Lerp (startPos, destinationPosition, movePercentage);

			yield return null;

		}

		currentCube = t;

		MoveToNext ();

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
		EnemyPathManager.Instance.RemoveEnemyFromPathManager (this);

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
