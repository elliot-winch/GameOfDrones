using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class BuildTool : HeldObject {

	int currentID = 0;

	#region HeldObject
	GameCube lastPointedAt;
	protected override void OnHeld (Hand hand){
		RaycastHit hitInfo;

		if (Physics.Raycast (transform.position, transform.forward, out hitInfo)) {
			if (hitInfo.collider != null) {
				//we hit a collider
				GameCube cube = hitInfo.collider.GetComponent<GameCube> ();

				if (cube != null) {

					if (lastPointedAt != cube) {
						if (lastPointedAt != null) {
							lastPointedAt.OnPointedAway ();
						}

						cube.OnPointedAt ();
					}

					lastPointedAt = cube;

					if (Input.GetKeyDown (BuildManager.Instance.buildKey)) {

						//hitInfo.collider.GetComponent<GameCube> ();
						cube.Occupying = Instantiate(BuildManager.Instance.buildables[currentID]);
					}

					return;
				} 
			}
		} 

		//missed cube tihs frame, but last frame was hitting a box
		if (lastPointedAt != null) {
			lastPointedAt.OnPointedAway ();

			lastPointedAt = null;

		}
	}
	#endregion
}
