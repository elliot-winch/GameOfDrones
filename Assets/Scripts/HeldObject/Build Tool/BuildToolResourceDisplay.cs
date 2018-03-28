using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * Perhaps have this activate when the object is picked up?
 */ 

[RequireComponent(typeof(ResourceManager))]
public class BuildToolResourceDisplay : MonoBehaviour {

	public Text resourceDisplay;

	public void UpdateDisplay(){

		resourceDisplay.text = ResourceManager.Instance.PlayerResources.ToString();
	}
}
