using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour {

	static BulletTimeManager instance;

	public float sloMoFactor = 0.4f;

	bool inSloMo = false;

	List<Collider> projectiles;

	public static BulletTimeManager Instance {
		get {
			return instance;
		}
	}

	void Start(){

		if (instance != null) {
			Debug.LogError ("Error: Cannot have two BulletTimeManagers");
		}

		instance = this;

		projectiles = new List<Collider>();

	}

	void LateUpdate(){

		for(int i = projectiles.Count - 1; i >= 0; i--){
			if (projectiles[i] == null) {
				projectiles.RemoveAt (i);
			}
		}

		if (projectiles.Count > 0 && inSloMo == false) {
			EnterSlowMo ();
		}

		if (projectiles.Count <= 0 && inSloMo == true) {
			ExitSlowMo ();
		}
	}

	void OnTriggerEnter(Collider col){
		if (IsValidProjectile(col)){

			projectiles.Add (col);
		}
	}


	void OnTriggerExit(Collider col){

		projectiles.Remove (col);
	}

	bool IsValidProjectile(Collider col){
		return col.GetComponent<Projectile> () != null && (
		    	col.GetComponent<Projectile> ().LayersToHit == null
		    || (col.GetComponent<Projectile> ().LayersToHit != null && col.GetComponent<Projectile> ().LayersToHit.Contains ("Friendly")));
				
	}
		
	void EnterSlowMo(){
		inSloMo = true;
		Time.timeScale = sloMoFactor;
	}

	void ExitSlowMo(){
		inSloMo = false;
		Time.timeScale = 1f;

	}
}
