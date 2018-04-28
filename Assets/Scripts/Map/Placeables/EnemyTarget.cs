using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class EnemyTarget : Wall {

	TextMeshPro tmp;
	AudioSource aSource;

	protected override void Start ()
	{
		base.Start ();

		tmp = GetComponentInChildren<TextMeshPro> ();

		aSource = GetComponentInChildren<AudioSource> ();

		tmp.text = CurrentHealth.ToString ();
	}

	public override void Hit (Vector3 hitPoint, Vector3 from, float amount)
	{
		base.Hit (hitPoint, from, amount);

		if (aSource.isPlaying == false) {
			aSource.Play ();
		}

		tmp.text = CurrentHealth.ToString ();
	}


	protected override void Destroyed ()
	{

		this.gameCube.Locked = true;

		GameManager.Instance.EndGame ();

		base.Destroyed ();
	}
}
