using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;
	public float damage = 1f;

	public void Launch(Vector3 position){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched

		this.transform.LookAt (position);


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
			Debug.Log (col.collider.name + " hit for " + damage);
			col.collider.GetComponentInParent<DamagableObject> ().Hit (damage);
		}

		Destroy (gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (gameObject);
	}
}
