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
	private BuildTool currentlyPointingAt = null;

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

	public BuildTool CurrentlyPointingAt {
		get {
			return currentlyPointingAt;
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

		MoveCost = 1f;

		EnemyPathManager.Instance.ShouldRecalcPathRemoved ();
	}
	#endregion

	#region Building UI
	public enum PlacementError {
		None,
		GameNotStarted,
		CubeIsOccupied,
		NotEnoughResources,
		CannotTotallyBlockEnemies
	}

	/*
	public enum RepairError {
		None,
		NotEnoughResources,
		OnFullHealth
	}
	*/
	public enum RemoveError
	{
		None,
		GameNotStarted,
		CubeIsLocked, 
		NothingToRemove
	}

	public PlacementError CanPlace(IPlaceable placeable){

		//Order here is important!
		if (GameManager.Instance.GameRunning == false) {
			return PlacementError.GameNotStarted;
		}

		if (this.Occupying != null || this.Locked == true) {
			return PlacementError.CubeIsOccupied;
		}
	

		if (ResourceManager.Instance.CanSpend(placeable.BuildCost) == false) {
			return PlacementError.NotEnoughResources;
		}

		if (EnemyPathManager.Instance.WouldYieldNoPaths (this, placeable.MoveCost) == false) {
			return PlacementError.CannotTotallyBlockEnemies;
		}


		return PlacementError.None;
	}

	/*
	public RepairError CanRepair(){

		//already checked to see if there is something occupying the space
		return RepairError.None;
	}
	*/
	public RemoveError CanRemove(){

		if (GameManager.Instance.GameRunning == false) {
			return RemoveError.GameNotStarted;
		}

		if (Locked == true) {
			return RemoveError.CubeIsLocked;
		}

		if (this.Occupying == null) {
			return RemoveError.NothingToRemove;
		}

		return RemoveError.None;
	}

	public bool OnPointedAt(IPlaceable placeable, BuildTool bt){

		if (currentlyPointingAt == null) {
			currentlyPointingAt = bt;

		//Place
			if (placeable != null) {

				PlacementError pe = CanPlace (placeable);

				if (pe == PlacementError.None) {
			
					PositiveBuildUI (placeable);
					return true;
				} else {
					NegativeBuildUI (placeable, pe);
					return false;
					//display some ui based on error
				}
			} else {
				//Remove UI

				RemoveError re = CanRemove ();

				Debug.Log (re);

				if (re == RemoveError.None) {
					PositiveRemoveUI ();
					return true;
				} else {
					NegativeRemoveUI (re);
					return false;
				}
			}
		}

		return false;
	}

	public void OnPointedAway(){
		Reset (cubeMat);

		if(inRangeMats != null){
			foreach (Material m in inRangeMats) {
				Reset (m);
			}
		}

		currentlyPointingAt = null;
	}

	List<Material> inRangeMats;
	void PositiveBuildUI(IPlaceable placeable){

		cubeMat.SetColor ("_RimColor", Color.green);
		cubeMat.SetFloat ("_RimPower", 0.1f);

		if (placeable is IRangedPlaceable) {

			IRangedPlaceable placeableTurret = (IRangedPlaceable)placeable;

			inRangeMats = new List<Material> ();

			Collider[] cols = Physics.OverlapSphere (transform.position, placeableTurret.Range, LayerMask.GetMask ("GameCube"));

			foreach (Collider c in cols) {
			
				if (c.GetComponent<GameCube> () == null) {
					Debug.LogWarning ("GameCube Warning: Detect non-GameCube in range wth GameCube tag");
					continue;
				}


				Material m = c.GetComponent<MeshRenderer> ().material;

				//Dont want to modify current cube
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


	void PositiveRemoveUI(){

		cubeMat.SetColor ("_RimColor", Color.green);
		cubeMat.SetFloat ("_RimPower", 0.1f);

	}

	void NegativeRemoveUI(RemoveError re){

		cubeMat.SetColor ("_RimColor", Color.red);
		cubeMat.SetFloat ("_RimPower", 0.1f);

	}

	void Reset(Material m){
		m.SetColor ("_RimColor", originalColor);
		m.SetFloat ("_RimPower", 3f);
	}
	#endregion Building UI

	
}
