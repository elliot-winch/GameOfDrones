using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Pistol : Gun {

	public float hapticDurationAsPercentageOfFireRate = 0.5f;
	public float hapticStrength = 0.5f;

	ParticleSystem muzzleFlash;
	AudioSource[] audioSources;

	int lastPlayedIndex = -1;

	protected override void Awake()
	{
		base.Awake();

		audioSources = GetComponentsInChildren<AudioSource> ();

		muzzleFlash = barrel.GetChild(0).GetComponent<ParticleSystem> ();

	}

	protected override bool FireControlActivated (HandControls hc)
	{
		return hc.TriggerPulled.Down;
	}

	protected override void Fire (Hand hand)
	{
		GameObject proj = Instantiate (projectile, new Vector3(barrel.transform.position.x, barrel.transform.position.y, barrel.transform.position.z + projectile.GetComponent<Collider>().bounds.extents.z), Quaternion.identity);

		proj.GetComponentInChildren<Projectile> ().Launch (this.damage, barrel.transform, gameObject, new string[] { "Enemy" });

		//Muzzle flash
		muzzleFlash.Play ();

		//Haptic feedback
		HapticPulseForTime (hand.controller, rateOfFire * hapticDurationAsPercentageOfFireRate, hapticStrength);

		//Audio
		int randomAudioSource = Random.Range(0, audioSources.Length);
		//The chance we play two audio clips back to back is 1 / (audioSources.Length ^ 2)
		if (randomAudioSource == lastPlayedIndex) {
			randomAudioSource = Random.Range(0, audioSources.Length);
		}

		audioSources[randomAudioSource].Play();

		lastPlayedIndex = randomAudioSource;

		Debug.Log (randomAudioSource);
	}

}
