using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;

	private float damage;
	private GameObject launcher;

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

	public void Launch(Vector3 position, float damage, GameObject launcher, string[] layersToHit = null){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched
	
		this.transform.LookAt (position);

		Launch(damage, launcher, layersToHit);
	}

	public void Launch(Transform inLineWith, float damage,  GameObject launcher, string[] layersToHit = null)
	{

		this.transform.forward = inLineWith.forward;

		Launch(damage, launcher, layersToHit);

	}

	private void Launch(float damage, GameObject launcher, string[] layersToHit){

		this.damage = damage;
		this.launcher = launcher;
		this.layersToHit = layersToHit;


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
		if (this.layersToHit != null && layersToHit.Contains(LayerMask.LayerToName(col.gameObject.layer)))
		{ 
			if (col.collider.GetComponentInParent<DamagableObject>() != null)
			{
				col.collider.GetComponentInParent<DamagableObject>().Hit(col.contacts[0].point, this.launcher.transform, this.damage);
			}
		}

		Destroy (rootParent.gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (rootParent.gameObject);
	}
}
