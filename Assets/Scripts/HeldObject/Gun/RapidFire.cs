using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class RapidFire : Gun {


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


		muzzleFlash = barrel.GetChild(0).GetComponent<ParticleSystem> ();

	}

	protected override bool FireControlActivated (HandControls hc)
	{
		return hc.TriggerPulled.Any;
	}

	protected override void Fire (Hand hand)
	{
		GameObject proj = Instantiate (projectile, new Vector3(barrel.transform.position.x, barrel.transform.position.y, barrel.transform.position.z + projectile.GetComponent<Collider>().bounds.extents.z), Quaternion.identity);

		proj.GetComponentInChildren<Projectile> ().Launch (barrel.transform, this.damage, this.projectileSpeed, gameObject, new string[] { "Enemy", "Detonator"  });

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
