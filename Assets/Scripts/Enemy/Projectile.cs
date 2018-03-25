using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;
	public float damage = 1f;

	string[] layersToHit;

	public void Launch(Vector3 position, string[] layersToHit = null){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched

		this.transform.LookAt (position);

		this.layersToHit = layersToHit;

		Launch ();

	}

	public void Launch(Transform inLineWith, string[] layersToHit = null)
	{

		this.transform.forward = inLineWith.forward;

		this.layersToHit = layersToHit;

		Launch();

	}

	private void Launch(){

		GetComponent<Rigidbody> ().velocity = transform.forward * speed;

		StartCoroutine (DestroyOnDelay (10f));
	}

	void OnCollisionEnter(Collision col){

		//we have defined layers and this isn't one of them
		if (this.layersToHit != null && layersToHit.Contains(col.collider.tag) == false)
		{ 
			if (col.collider.GetComponentInParent<DamagableObject>() != null)
			{
				col.collider.GetComponentInParent<DamagableObject>().Hit(damage);
			}
		}

		Destroy (gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (gameObject);
	}
}
