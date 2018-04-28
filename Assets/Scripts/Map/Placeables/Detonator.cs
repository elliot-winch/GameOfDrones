using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : Wall, IRangedPlaceable {

	public float rechargeTime = 5f;
	public float explodeRadius = 1f;

	public float Range { get { return explodeRadius; } } 

	public float explodeDamage = 15f;

	bool explodable = true;

	private AudioSource explodeAS;

	protected override void Start ()
	{
		base.Start ();

		explodeAS = GetComponent<AudioSource>();
	}

	IEnumerator Regrow(){

		transform.localScale = Vector3.zero;
		float timer = 0f;

		while (timer < rechargeTime) {

			transform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one, timer / rechargeTime);

			timer += Time.deltaTime;

			yield return null;
		}

		explodable = true;
	}

	public override void Hit (Vector3 hitPoint, Vector3 from, float amount)
	{
		if (explodable) {
			base.Hit (hitPoint, from, amount);
		}
	}

	protected override void Destroyed ()
	{
		explodeAS.Play ();

		foreach(Collider col in Physics.OverlapSphere (transform.position, explodeRadius, LayerMask.GetMask("Enemy"))){

			if (col.GetComponentInParent<DamagableObject> () != null) {

				col.GetComponentInParent<Enemy> ().Hit (col.transform.position, transform.position, explodeDamage);
			}
		}

		explodable = false;
		CurrentHealth = startingHealth;

		StartCoroutine (Regrow ());
	}
}
