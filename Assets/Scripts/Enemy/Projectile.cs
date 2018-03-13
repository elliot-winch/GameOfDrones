using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;
	public float damage = 1f;

	public void Launch(DamagableObject towards){

		this.transform.LookAt (towards.transform.position);

		Launch ();
	}

	public void Launch(Transform inLineWith){

		this.transform.forward = inLineWith.forward;

		Launch ();

	}

	private void Launch(){

		GetComponent<Rigidbody> ().velocity = transform.forward * speed;

		StartCoroutine (DestroyOnDelay (10f));
	}

	void OnCollisionEnter(Collision col){

		if (col.collider.GetComponentInParent<DamagableObject> () != null) {
			col.collider.GetComponentInParent<DamagableObject> ().Hit (damage);
		}

		Destroy (gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (gameObject);
	}
}
