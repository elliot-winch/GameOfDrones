using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSeekingEnemy : Enemy {

	protected DamagableObject currentTarget;

	private bool firing;
	private bool disengaging;

	private bool explicitTarget = false;
	private Coroutine lockPosition;
	private Coroutine fireCoroutine;

	protected AudioSource chargeAS;
	protected AudioSource fireAS;
	protected AudioSource destroyedAS;

	private Coroutine FireCoroutine {
		set {

			if (fireCoroutine != null) {
				StopCoroutine (fireCoroutine);
			}

			fireCoroutine = value;
		}
	}

	private Rigidbody rb;

	protected List<string> layersToHit;

	protected bool Firing {
		get {
			return firing;
		}
	}

	protected override void Start ()
	{
		base.Start ();

		layersToHit = new List<string> ();
		layersToHit.Add ("Friendly");

		chargeAS = gameObject.AddComponent<AudioSource> ();

		fireAS = gameObject.AddComponent<AudioSource> ();
		destroyedAS = gameObject.AddComponent<AudioSource> ();

		rb = GetComponent<Rigidbody> ();

	}

	//Search for targets if not firing. If there are no targets, disengage
	protected override void Update(){

		//Currently attacking
		if (firing == false) {
			//Needs a new target
			if (currentTarget == null) {
				Collider[] cols = Physics.OverlapSphere (transform.position, attackRange, LayerMask.GetMask("Friendly"));

				if (cols.Length > 0) {
					//decide which enemy to shoot - currently we just pick the first target found
					currentTarget = cols [0].GetComponentInParent<DamagableObject> ();

					FireCoroutine = StartCoroutine (Fire (currentTarget, cols[0]));
					disengaging = false;
				//No target found
				} else if (disengaging == false) {
					Disengage ();
					disengaging = true;
				}
			} 
		}
	}

	#region Moving
	protected override void MoveToNext ()
	{
		if (currentPathIndex + 1 >= pathToTarget.Count) {

			//lock position so drones dont float away
			lockPosition = StartCoroutine(LockPosition(transform.position));

			//a little unclean
			currentTarget = EnemyPathManager.Instance.EnemyDestination.Occupying.GetComponent<DamagableObject> ();
			explicitTarget = true;

			layersToHit.Add ("Target");

			FireCoroutine = StartCoroutine (Fire (currentTarget, currentTarget.GetComponentInChildren<Collider>()));

			Debug.Log (currentTarget.GetComponentInChildren<Collider> ());
			disengaging = false;

		} else {

			base.MoveToNext ();

			if (currentTarget == null) {
				LookAt (destinationPosition);
			}
		}
	}

	IEnumerator LockPosition(Vector3 lockedPosition){
		while (true) {
			transform.position = lockedPosition;

			yield return null;
		}
	}
	#endregion


	#region Firing
	private IEnumerator Fire(DamagableObject target, Collider c){
		firing = true;

		Track (c.transform);

		//the charge sound should finish just as the weapon fires
		if(target is GamePlayer){
			chargeAS.Play ();
		}

		yield return new WaitForSeconds(weaponChargeTime);

		if (c != null) {
			OnFire (c);
		} 
			
		firing = false;

		if (explicitTarget == false) {
			currentTarget = null;
		} else {
			FireCoroutine = StartCoroutine(Fire (currentTarget, c));
		}
	}


	protected virtual void OnFire(Collider target) {} 
	protected virtual void PreFire (Collider target) {}
	#endregion

	protected override void Destroyed ()
	{

		base.Destroyed ();

		Rigidbody rb = gameObject.AddComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;

		if(lockPosition != null){
			StopCoroutine(lockPosition);
		}

		if (fireCoroutine != null) {
			StopCoroutine (fireCoroutine);
		}

		fireAS.Stop ();
		chargeAS.Stop ();

		destroyedAS.clip = EnemyAudioManager.Instance.GetRandomDestroyedAudio ();
		destroyedAS.Play ();

		StartCoroutine (DestroyWithWait (3f));

		this.enabled = false;
	}
		

	IEnumerator DestroyWithWait(float delay){

		yield return new WaitForSeconds (delay);

		Destroy (gameObject);
	}
}
