using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Valve.VR.InteractionSystem;
/* Place on collider on the back of the player's head*/

public class Switcher : MonoBehaviour {

	public HeldObject[] switchBetween;

	void OnTriggerEnter(Collider col){

		HeldObject ho = col.gameObject.GetComponent<HeldObject> ();
		Hand hand = col.transform.GetComponentInParent<Hand> ();

		if (ho != null && hand != null) {
			hand.DetachObject (col.gameObject, false);

			Destroy (col.gameObject);

			if (ho is Gun) {
				Debug.Log ("Switched to build tool");

				GameObject go = Instantiate (switchBetween [1].gameObject);

				hand.AttachObject (go);

			} else if (ho is BuildTool) {
				Debug.Log ("Switched to gun");

				GameObject go = Instantiate (switchBetween [0].gameObject);

				hand.AttachObject (go);

			}
		}
	}
}
