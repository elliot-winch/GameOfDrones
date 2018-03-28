using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : DamagableObject, IPlaceable {

	public int cost = 10;
	public float attackRange = 1f;
	public float damage = 1f;
	public float weaponChargeTime = 0.2f;

	public float rotateSpeed = 1f; //cosmetic
	public GameObject projectile;

	private GameCube cube;

	private bool firing = false;

	//should this be enemy or damagable object?
	private DamagableObject currentTarget;
	private Action onShoot;

	#region IPlaceable Implementation
	public GameCube Cube {
		get {
			return cube;
		} set {
			this.cube = value;

			transform.SetParent (cube.transform);
			transform.position = this.cube.Position;
		}
	}

	public string Name {
		get {
			return "Standard Turret";
		}
	}

	public int Cost {
		get {
			return cost;
		}
	}

	public string CostStat {
		get {
			return cost.ToString();
		}
	}

	public string HealthStat {
		get {
			return CurrentHealth.ToString ();
		}
	}

	public string DamageStat {
		get {
			return damage.ToString();
		}
	}

	public string RangeStat {
		get {
			return attackRange.ToString();
		}
	}

	public string RateOfFireStat {
		get {
			return weaponChargeTime.ToString();
		}
	}

	#endregion IPlacable

	public void RegisterOnShootCallback(Action callback){
		onShoot += callback;
	}

	protected override void Update(){

		//Attacking
		Collider[] cols = Physics.OverlapSphere (transform.position, attackRange, LayerMask.GetMask("Enemy"));

		if (firing == false) {
			if (currentTarget == null) {
				if (cols.Length > 0) {
					//decide which enemy to shoot - currently we just pick the first target found
					currentTarget = cols [0].GetComponentInParent<DamagableObject> ();
					StartCoroutine(Fire ());

				}
			} 
		}
	}

	IEnumerator Fire(){
		firing = true;

		StartCoroutine(SmoothLookAt (currentTarget.transform));

		yield return new WaitForSeconds(weaponChargeTime);

		if (currentTarget == null) {
			//the target has been destroyed before this got a chance to shoot at it
			firing = false;
			yield break;
		}

		GameObject proj = Instantiate (projectile, transform.position, Quaternion.identity);

		Collider projCol = proj.GetComponent<Collider> ();

		//to prevent the case where something was firing at an object that is destroyed before the projectile is launched

		foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>()) {
			Physics.IgnoreCollision (projCol, mc);
		}

		proj.GetComponent<Projectile> ().Launch (currentTarget.transform.position, new string[] { "Enemy" });

		currentTarget = null;

		firing = false;
	}


	IEnumerator SmoothLookAt(Transform target){

		Debug.Log ("Coroutine enter for turret");
		Quaternion targetRotation = Quaternion.LookRotation (target.position - transform.position);

		while (Quaternion.Angle (transform.rotation, targetRotation) > 1f) {

			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
