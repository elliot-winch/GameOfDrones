using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamagableObject {

	public int resourcesGainedOnDestroyed = 5;
	public float attackRange = 1f;
	public float attackDamage = 1f;
	public float weaponChargeTime = 0.2f;

	public float moveSpeed = 1f;
	//rotateSpeed is for cosmetics only. A drone will not delay firing to rotate
	public float rotateSpeed = 1f;
	public GameObject onHitParticleSystem;

	private GameCube startCube;
	protected GameCube currentCube;
	protected GameCube destination;
	protected Vector3 destinationPosition;

	protected List<GameCube> pathToTarget;
	protected int currentPathIndex;
	protected Coroutine moveCoroutine;
	protected Coroutine lookAt;

	protected AudioSource hitAS;

	public GameCube StartCube {
		get {
			return startCube;
		}
	}

	protected override void Start(){
		base.Start ();

		hitAS = gameObject.AddComponent<AudioSource> ();

		foreach (Collider col1 in GetComponentsInChildren<Collider>()) {
			foreach (Collider col2 in GetComponentsInChildren<Collider>()) {

				if (col1 != col2) {
					Physics.IgnoreCollision (col1, col2);
				}
			}
		}
	}

	//Begin - like start but with parameters. Called by WaveManager
	public void Begin(GameCube startCube){
		//Pathfinding is managed here
		this.startCube = startCube;
		this.currentCube = startCube;

	}


	#region Pathfinding
	public void SetPath(List<GameCube> path){
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


	protected virtual void MoveToNext(){

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


			movePercentage += (moveSpeed * Time.deltaTime) / (dist * t.MoveCost);

			transform.position = Vector3.Lerp (startPos, destinationPosition, movePercentage);

			yield return null;

		}

		currentCube = t;

		MoveToNext ();

	}
	#endregion


	#region Looking Rotation
	//When there is no targt, a target is searched for and not found.
	protected virtual void Disengage(){
		LookAt (destinationPosition);
	}


	//Private methods
	protected void LookAt(Vector3 position){
		if (lookAt != null) {
			StopCoroutine (lookAt);
			lookAt = null;
		}

		lookAt = StartCoroutine (SmoothLookAt (position));
	}

	//Private methods
	protected void Track(Transform t){
		if (lookAt != null) {
			StopCoroutine (lookAt);
			lookAt = null;
		}

		lookAt = StartCoroutine (SmoothTrack (t));
	}


	IEnumerator SmoothLookAt(Vector3 position){

		Quaternion targetRotation = Quaternion.LookRotation (position - transform.position);

		while (Quaternion.Angle (transform.rotation, targetRotation) > 1f) {
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}

	IEnumerator SmoothTrack(Transform t){

		while (true) {

			Quaternion targetRotation = Quaternion.LookRotation (t.position - transform.position);

			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}
	#endregion


	#region Hit
	public override void Hit (Vector3 hitPoint, Vector3 hitFrom, float amount){

		base.Hit (hitPoint, hitFrom, amount);

		//FIXME!
		hitAS.clip = EnemyAudioManager.Instance.GetRandomDestroyedAudio ();
		hitAS.Play ();

		//Particle Effect
		if (onHitParticleSystem != null) {
			GameObject peObj = Instantiate (onHitParticleSystem, transform);
			peObj.transform.position = hitPoint;
			peObj.transform.LookAt (hitFrom);

			peObj.GetComponent<ParticleSystem> ().Play ();
		}

	}


	protected override void Destroyed ()
	{
		ResourceManager.Instance.AddResources (resourcesGainedOnDestroyed);

		EnemyPathManager.Instance.RemoveEnemyFromPathManager (this);

		transform.parent = WaveManager.Instance.enemyDeadParent;

		WaveManager.Instance.CouldEndWave ();

		hitAS.Stop ();


		if (lookAt != null) {
			StopCoroutine (lookAt);
		}

		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}

		//Enemy doesn't call it's super! THe enemy needs to be destroyed by hand in subclasses!
	}
	#endregion
}
