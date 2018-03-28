using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSelector : HeldObject {

	protected override void Awake(){

		base.Awake();

		if (GetComponent<ControlWheel> () != null) {

			GetComponent<ControlWheel>().AddControlWheelAction(new ControlWheelSegment (() => {
				Debug.Log ("Test");
			}, null));
		}

		//temp
		GetComponent<ControlWheel>().DisplayControlWheel();
	}
}
