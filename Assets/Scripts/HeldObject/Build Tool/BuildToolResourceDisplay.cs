using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildToolResourceDisplay : MonoBehaviour {

	Text text;

	void Awake(){
		text = GetComponent<Text> ();
	}

	public void UpdateDisplay(int amount){

		text.text = amount.ToString ();
	}

}
