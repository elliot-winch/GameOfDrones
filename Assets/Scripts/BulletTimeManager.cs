using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour {

	static BulletTimeManager instance;

	public float sloMoFactor = 0.4f;
	public float sloMoTransitionTime = 0.3f; //real time

	bool inSloMo = false;

	List<Collider> projectiles;

	Coroutine sloMoTransition;

	private Coroutine SloMoTransition {
		set {
			if (sloMoTransition != null) {
				StopCoroutine (sloMoTransition);
			}
			sloMoTransition = value;
		}
	}

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

		SloMoTransition = StartCoroutine(SmoothTimeChange(Time.timeScale, sloMoFactor));
	}


	void ExitSlowMo(){
		inSloMo = false;

		SloMoTransition = StartCoroutine (SmoothTimeChange (Time.timeScale, 1f));
	}

	IEnumerator SmoothTimeChange(float f, float to){

		List<AudioSource> audio = Object.FindObjectsOfType<AudioSource> ().ToList();


		for (int i = audio.Count - 1; i >= 0; i--) {
			if (audio [i].name == "Music") {
				audio.RemoveAt (i);
			}
		}

		float timer = 0f;

		while (timer <= sloMoTransitionTime) {

			float ts = Mathf.Lerp (f, to, timer / sloMoTransitionTime);
		
			ScaleTime (ts, audio);

			timer += Time.deltaTime;

			yield return null;
		}

		ScaleTime (to, audio);
	}

	void ScaleTime(float ts, List<AudioSource> audio){
		Time.timeScale = ts;
		foreach (AudioSource aud in audio) {
			if (aud != null) {
				//aud.pitch = (1f / 3f) * ts + (2f / 3f);
				aud.pitch = ts;
			}
		}
	}
}
