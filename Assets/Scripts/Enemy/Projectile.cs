using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	private float damage;

	private Vector3 firedFrom;

	string[] layersToHit;
	Transform rootParent;

	public string[] LayersToHit {
		get {
			return layersToHit;
		}
	}

	void Start(){
		Transform t = transform;
		while(t.parent != null){
			t = t.parent;
		}

		rootParent = t;
	}

	public void Launch(Vector3 position, float damage, float speed, GameObject launcher, string[] layersToHit = null){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched
	
		this.transform.LookAt (position);

		Launch(damage, speed, launcher, layersToHit);
	}

	public void Launch(Transform inLineWith, float damage, float speed,  GameObject launcher, string[] layersToHit = null)
	{

		this.transform.forward = inLineWith.forward;

		Launch(damage, speed, launcher, layersToHit);

	}

	private void Launch(float damage, float speed, GameObject launcher, string[] layersToHit){

		this.damage = damage;
		this.layersToHit = layersToHit;
		this.firedFrom = launcher.transform.position;


		foreach (Collider c in launcher.GetComponentsInChildren<Collider>()) {
			foreach (Collider mc in GetComponentsInChildren<Collider>()) {
				Physics.IgnoreCollision (c, mc);

			}
		}

		GetComponent<Rigidbody> ().velocity = transform.forward * speed;

		StartCoroutine (DestroyOnDelay (10f));
	}

	void OnCollisionEnter(Collision col){

		//we have defined layers and this isn't one of them
		if (this.layersToHit == null || (this.layersToHit != null && layersToHit.Contains(LayerMask.LayerToName(col.gameObject.layer))))
		{ 
			

			DamagableObject dObj = col.collider.GetComponentInParent<DamagableObject> ();

			if (dObj != null && dObj.enabled)
			{
				dObj.Hit(col.contacts[0].point, this.firedFrom, this.damage);
			}
		}

		Destroy (rootParent.gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (rootParent.gameObject);
	}
}
