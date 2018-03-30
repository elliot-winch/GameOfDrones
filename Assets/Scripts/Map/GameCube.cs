using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GameCube : MonoBehaviour {

	//temp!
	public GameObject cornerMarker;
	private List<GameObject> corners;

	//these integers will be the number of other game cubes we spawn based on the position of this cube
	//place the OG cube in the bottom left corner
	public int extendRight;
	public int extendUp;
	public int extendForward;

	private Vector3 position;
	private Vector3[] limits;
	private Vector3 size;

	private GameObject occupying;
	private bool locked = false;

	private float moveCost = 1f;

	#region Getters and Setter
	public Vector3 Position {
		get {
			return position;
		}
	}

	public Vector3 RandomPositionInBounds {
		get {
			return position + new Vector3 (Random.Range (-size.x / 2, size.x / 2), Random.Range (-size.y / 2, size.y / 2), Random.Range (-size.z / 2, size.z / 2));
		}
	}

	public Vector3[] Limits {
		get {
			return limits;
		}
	}

	public bool Locked {
		get {
			return locked;
		}
		set {
			locked = value;
		}
	}

	public GameObject Occupying {
		get {
			return occupying;
		}
		set {
			if (value != null && value.GetComponent<IPlaceable> () == null) {
				Debug.Log ("Cannot set cube's gameobject to non-placeable object");
				return;
			}

			if (locked) {
				Debug.Log ("Cannot replace current occupying");
				//tell user this info
				Destroy(value);
				return;
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

	public float MoveCost {
		get {
			return moveCost;
		}
		set {
			moveCost = value;
		}
	}

	#endregion

	void Start(){

		position = transform.position;

		size = GetComponent<BoxCollider> ().bounds.size;

		//Set Corners
		limits = new Vector3[8];
		corners = new List<GameObject> ();

		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				for (int k = 0; k < 2; k++) {
					Vector3 corner = new Vector3 (position.x + 0.9f * size.x * (i - 1/2f), position.y + 0.9f *  size.y * (j - 1/2f), position.z +  0.9f * size.z * (k - 1/2f));
					limits [i * 4 + j * 2 + k] = corner;

					corners.Add(Instantiate (cornerMarker, corner, Quaternion.identity, transform));
				}
			}
		}

		//TO prevent infinite recursion
		int right = extendRight;
		int up = extendUp;
		int forward = extendForward;

		extendRight = 0;
		extendUp = 0;
		extendForward = 0;

		SpawnAdditonals (up, right, forward);

	}

	void SetUpOccupying(GameObject pObj){

		pObj.GetComponent<IPlaceable> ().Cube = this;
	}

	void DestroyOccupying(){
		Destroy (occupying);
	}

	public void OnPointedAt(bool validPlacement){
		Color c;

		if (validPlacement) {
			c = Color.green;
		} else {
			c = Color.red;
		}

		foreach (GameObject g in corners) {
			g.GetComponent<MeshRenderer> ().material.color = c;
		}
	}

	public void OnPointedAway(){
		foreach (GameObject g in corners) {
			g.GetComponent<MeshRenderer> ().material.color = Color.white;
		}
	}

	//Helper for start

	public void SpawnAdditonals(int up, int right, int forward){

		for (int i = 0; i < right; i++) {
			for (int j = 0; j < up; j++) {

				for (int k = 0; k < forward; k++) {

					if (i == 0 && j == 0 && k== 0) {
						continue;
					}

					Instantiate (gameObject, new Vector3 (position.x + size.x * i, position.y + size.y * j, position.z + size.z * k), transform.rotation, transform.parent);
				}
			}
		}
	}
}
