using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Attach to the model of the gun, not the main prefab
 * 
 */ 

public class Recoiler : MonoBehaviour {

	public float maxRecoilTime;
	public float recoilSpeed;
	public float recoilUpSpeedLoss;

	public Transform rotateAbout;

	private Coroutine recoilCoroutine;

	void Start(){
		maxRecoilTime /= 2;
	}

	public void Recoil(){

		if (recoilCoroutine != null) {
			StopCoroutine (recoilCoroutine);
		}

		recoilCoroutine = StartCoroutine (RecoilCoroutine());
	}


	IEnumerator RecoilCoroutine(){

		float maxAngle = maxRecoilTime * recoilSpeed;
		float speed = recoilSpeed;
		float currentTime = 0f;

		while (currentTime <= maxRecoilTime) {

			float angle = Mathf.Lerp (0f, maxAngle, recoilSpeed * currentTime);

			transform.RotateAround(rotateAbout.position, transform.right, -Mathf.Sqrt(angle));

			currentTime += Time.deltaTime;
			//speed -= (recoilUpSpeedLoss * recoilUpSpeedLoss);

			Debug.Log (speed);

			yield return null;
		}

		currentTime = 0f;

		while (currentTime <= maxRecoilTime) {

			Debug.Log ("rotating forward");

			float angle = Mathf.Lerp (0f, maxAngle, recoilSpeed * currentTime);

			transform.RotateAround(rotateAbout.position, transform.right, Mathf.Sqrt(angle));

			currentTime += Time.deltaTime;
			//speed -= (recoilUpSpeedLoss * recoilUpSpeedLoss);

			Debug.Log (speed);


			yield return null;
		}
	}
}
