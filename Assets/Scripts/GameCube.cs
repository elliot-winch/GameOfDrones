using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class GameCube : MonoBehaviour {

	//temp!
	public GameObject cornerMarker;
	private List<GameObject> corners;

	private Vector3 position;
	private Vector3[] limits;

	private GameObject occupying;

	public Vector3 Position {
		get {
			return position;
		}
	}

	public Vector3[] Limits {
		get {
			return limits;
		}
	}

	public GameObject Occupying {
		get {
			return occupying;
		}
		set {
			if (value != null && value.GetComponent<IPlaceable> () == null) {
				Debug.Log ("Cannot set cube's gameobject to non-placeable object");
			}

			if (occupying == null && value != null) {

				occupying = value;
				SetUpOccupying (occupying);

			} else if (occupying != null && value == null) {
				DestroyOccupying ();
				occupying = null;
			} else if (occupying != null && value != null) {

				//check to see if turret type is the same
				DestroyOccupying ();
				occupying = value;
				SetUpOccupying (occupying);
			}

		}
	}

	void Start(){

		position = transform.position;

		Vector3 size = GetComponent<MeshCollider> ().bounds.size;

		limits = new Vector3[8];
		corners = new List<GameObject> ();

		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				for (int k = 0; k < 2; k++) {
					Vector3 corner = new Vector3 (position.x + size.x * (i - 1/2f), position.y + size.y * (j - 1/2f), position.z + size.z * (k - 1/2f));
					limits [i * 4 + j * 2 + k] = corner;

					corners.Add(Instantiate (cornerMarker, corner, Quaternion.identity, transform));
				}
			}
		}
	}

	void SetUpOccupying(GameObject pObj){
		BuildManager.Instance.AddPlaceable (occupying.GetComponent<IPlaceable> ());

		pObj.transform.parent = transform;
		pObj.transform.position = this.position;
	}

	void DestroyOccupying(){

		BuildManager.Instance.RemovePlaceable (occupying.GetComponent<IPlaceable> ());

		Destroy (occupying);
	}

	public void OnPointedAt(){
		foreach (GameObject g in corners) {
			g.GetComponent<MeshRenderer> ().material.color = Color.red;
		}
	}

	public void OnPointedAway(){
		foreach (GameObject g in corners) {
			g.GetComponent<MeshRenderer> ().material.color = Color.white;
		}
	}
}
