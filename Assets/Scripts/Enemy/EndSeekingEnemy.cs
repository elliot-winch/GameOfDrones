using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSeekingEnemy : Enemy {

	protected DamagableObject currentTarget;

	private bool firing;
	private bool disengaging;

	private Rigidbody rb;

	protected bool Firing {
		get {
			return firing;
		}
	}

	protected override void Start ()
	{
		base.Start ();

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

					StartCoroutine (Fire (currentTarget));
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
		base.MoveToNext ();

		if (currentTarget == null) {
			LookAt (destinationPosition);
		}
	}
	#endregion


	#region Firing
	private IEnumerator Fire(DamagableObject target){
		firing = true;

		Track (target.transform);

		yield return new WaitForSeconds(weaponChargeTime);

		if (target != null) {
			OnFire (target);
		} 

		currentTarget = null;

		firing = false;
	}


	protected virtual void OnFire(DamagableObject target) {} 
	protected virtual void PreFire (DamagableObject target) {}
	#endregion

	protected override void Destroyed ()
	{

		base.Destroyed ();

		rb.useGravity = true;
		rb.AddForce (10f * -transform.forward);

		StartCoroutine (DestroyWithWait (3f));
	}
		

	IEnumerator DestroyWithWait(float delay){

		yield return new WaitForSeconds (delay);

		Destroy (gameObject);
	}
}
