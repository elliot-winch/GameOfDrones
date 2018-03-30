using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun {

	protected override void Fire ()
	{
		GameObject proj = Instantiate (projectile, barrel.transform.position, Quaternion.identity);

		proj.GetComponentInChildren<Projectile> ().Launch (barrel.transform, gameObject, new string[] { "Friendly" });
	}

	protected override void Awake()
	{
		base.Awake();
	}
}
