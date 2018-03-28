using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	static WeaponManager instance;

	public HeldObject[] allHeldObjects;

	public static WeaponManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Start()
	{
		{
			if(instance != null)
			{
				Debug.LogError("Cannot be two Weapon Managers");
			}

			instance = this;
		}
	}
}
