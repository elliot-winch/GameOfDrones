using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;
/*
 * This class is to be attached to the build tool, and manages the preview
 * 
 */ 
[RequireComponent(typeof(BuildTool))]
public class BuildToolPreview : MonoBehaviour, IHeldUpdateable {

	public Material previewMaterial;
	public Transform previewTransform;
	public float previewScale;
	public float previewRotateSpeed;

	GameObject previewObj = null;

	public void DisplayModel(GameObject placeablePrefab){

		if (previewObj != null) {
			RemovePreviewBuild();
		}

		previewObj = Instantiate (placeablePrefab, previewTransform);

		previewObj.transform.localPosition = Vector3.zero;
		previewObj.transform.localRotation = Quaternion.identity;

		previewObj.transform.localScale = previewObj.transform.lossyScale *  previewScale;

		//preview obj shouldnt shot at enemies!
		if (previewObj.GetComponent<Turret> () != null) {
			previewObj.GetComponent<Turret> ().enabled = false;
		}

		foreach (MeshRenderer mr in previewObj.GetComponentsInChildren<MeshRenderer>()) {
			mr.material = previewMaterial;
		}
	}

	void RemovePreviewBuild(){
		//animate object leaving, for now destroy
		Destroy (previewObj);
	}

	#region IHeldUpdatable
	public void HeldStart(){


	}

	public void HeldUpdate(){

		//Preview spin
		if (previewObj != null) {
			previewObj.transform.Rotate(new Vector3(0, Time.deltaTime * previewRotateSpeed, 0));
		}
	}

	public void HeldEnd(){

		RemovePreviewBuild ();
	}
	#endregion
}
