using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;
	public float damage = 1f;

	string[] layersToHit;
	Transform rootParent;

	void Start(){
		Transform t = transform;
		while(t.parent != null){
			t = t.parent;
		}

		rootParent = t;
	}

	public void Launch(Vector3 position, GameObject dontCollideWith, string[] layersToHit = null){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched

		this.transform.LookAt (position);

		this.layersToHit = layersToHit;

		Launch (dontCollideWith);

	}

	public void Launch(Transform inLineWith, GameObject dontCollideWith, string[] layersToHit = null)
	{

		this.transform.forward = inLineWith.forward;

		this.layersToHit = layersToHit;

		Launch(dontCollideWith);

	}

	private void Launch(GameObject dontCollideWith){

		foreach (Collider c in dontCollideWith.GetComponentsInChildren<Collider>()) {
			foreach (Collider mc in GetComponentsInChildren<Collider>()) {
				Physics.IgnoreCollision (c, mc);
			}
		}

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

		Destroy (rootParent.gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (rootParent.gameObject);
	}
}
