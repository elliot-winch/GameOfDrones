using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class Pistol : Gun {

	public float hapticDurationAsPercentageOfFireRate = 0.5f;
	public float hapticStrength = 0.5f;
	public float projectileSpeed = 40f;

	public AudioClip[] fireClips;

	ParticleSystem muzzleFlash;
	AudioSource audioSource;

	int lastPlayedIndex = -1;

	protected override void Awake()
	{
		base.Awake();

		audioSource = GetComponentInChildren<AudioSource> ();

		muzzleFlash = barrel.GetComponentInChildren<ParticleSystem> ();

	}

	protected override bool FireControlActivated (HandControls hc)
	{
		return hc.TriggerPulled.Down;
	}

	protected override void Fire (Hand hand)
	{
		base.Fire (hand);

		GameObject proj = Instantiate (projectile, barrel.transform.position, Quaternion.identity);

		proj.GetComponentInChildren<Projectile> ().Launch (barrel.transform, this.damage, this.projectileSpeed, gameObject, new string[] { "Enemy", "Detonator" });

		//Muzzle flash
		muzzleFlash.Play ();

		//Haptic feedback
		HapticPulseForTime (hand.controller, rateOfFire * hapticDurationAsPercentageOfFireRate, hapticStrength);

		//Audio
		int randomAudioFire = Random.Range(0, fireClips.Length);
		//The chance we play two audio clips back to back is 1 / (audioSources.Length ^ 2)
		if (randomAudioFire == lastPlayedIndex) {
			randomAudioFire = Random.Range(0, fireClips.Length);
		}
			
		audioSource.clip = fireClips [randomAudioFire];
		audioSource.Play ();

		lastPlayedIndex = randomAudioFire;

	}

}
