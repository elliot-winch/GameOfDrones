using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour {

	static EnemyAudioManager instance;

	public static EnemyAudioManager Instance {
		get {
			return instance;
		}
	}

	public AudioClip[] shootClips;
	public AudioClip[] damagedClips;
	public AudioClip[] destroyedClips;
	public AudioClip chargeClip;

	void Start(){

		if (instance != null) {
			Debug.LogError ("Should not have more than one EnemyAudioManager");
		}

		instance = this;
	}

	public AudioClip GetRandomShootAudio(){

		return shootClips [Random.Range (0, shootClips.Length)];
	}

	public AudioClip GetRandomHitAudio(){

		return damagedClips [Random.Range (0, damagedClips.Length)];
	}

	public AudioClip GetRandomDestroyedAudio(){

		return destroyedClips [Random.Range (0, destroyedClips.Length)];
	}
}
