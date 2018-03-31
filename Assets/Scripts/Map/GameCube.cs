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
	private Vector3 size;

	private GameObject occupying;
	private bool locked = false;

	Material cubeMat;
	Color originalColor;

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
				//this case sohuld have been dealt with before building
				Debug.Log ("Cannot replace current occupying");
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

		//For higlhighting
		cubeMat = GetComponent<MeshRenderer>().material;

		originalColor = cubeMat.GetColor ("_RimColor");

		/*
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
		*/

		//To prevent infinite recursion
		int right = extendRight;
		int up = extendUp;
		int forward = extendForward;

		extendRight = 0;
		extendUp = 0;
		extendForward = 0;

		SpawnAdditonals (up, right, forward);

	}

	//Helper for start. Spawns more cubes
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

	#region Spawning Placeable
	void SetUpOccupying(GameObject pObj){

		pObj.GetComponent<IPlaceable> ().Cube = this;
	}

	void DestroyOccupying(){
		Destroy (occupying);
	}
	#endregion

	#region Building UI
	public enum PlacementError {
		None,
		CubeIsLocked,
		NotEnoughResources,
		CannotTotallyBlockEnemies
	}

	public PlacementError CanPlace(IPlaceable placeable){

		if (Locked == true) {
			return PlacementError.CubeIsLocked;
		}

		if (ResourceManager.Instance.PlayerResources - placeable.BuildCost < 0) {
			return PlacementError.NotEnoughResources;
		}

		if (EnemyPathManager.Instance.WouldYieldNoPaths (this) == false) {
			return PlacementError.CannotTotallyBlockEnemies;
		}

		return PlacementError.None;
	}

	public void OnPointedAt(IPlaceable placeable){
		PlacementError pe = CanPlace (placeable);

		if ( pe == PlacementError.None) {
			
			PositiveBuildUI (placeable);

		} else {
			NegativeBuildUI (placeable, pe);

			//display some ui based on error
		}


	}

	public void OnPointedAway(){
		Reset (cubeMat);

		if(inRangeMats != null){
			foreach (Material m in inRangeMats) {
				Reset (m);
			}
		}
	}

	List<Material> inRangeMats;
	void PositiveBuildUI(IPlaceable placeable){

		cubeMat.SetColor ("_RimColor", Color.green);
		cubeMat.SetFloat ("_RimPower", 0.1f);

		if (placeable is Turret) {

			Turret placeableTurret = (Turret)placeable;

			inRangeMats = new List<Material> ();

			Collider[] cols = Physics.OverlapSphere (transform.position, placeableTurret.range, LayerMask.GetMask ("GameCube"));

			foreach (Collider c in cols) {
			
				if (c.GetComponent<GameCube> () == null) {
					Debug.LogWarning ("GameCube Warning: Detect non-GameCube in range wth GameCube tag");
					continue;
				}


				Material m = c.GetComponent<MeshRenderer> ().material;

				//Dont want to modifiy current cube
				if (m == cubeMat) {
					continue;
				}

				m.SetColor ("_RimColor", Color.yellow);
				m.SetFloat ("_RimPower", 0.1f);

				inRangeMats.Add (m);
			}
		}
	}

	void NegativeBuildUI(IPlaceable placeable, PlacementError pe){

		cubeMat.SetColor ("_RimColor", Color.red);
		cubeMat.SetFloat ("_RimPower", 0.1f);

		Debug.Log (pe);
	}

	void Reset(Material m){
		m.SetColor ("_RimColor", originalColor);
		m.SetFloat ("_RimPower", 3f);
	}
	#endregion Building UI

	
}
