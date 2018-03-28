using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is to divide the touch pad into n sections, with arbitrary 
 * actions for each section
 * 
 */ 

[RequireComponent(typeof(HeldObject))]
public class ControlWheel : MonoBehaviour {

	public float farRadius = 1f;
	public float nearRadius = 0.5f;
	public float outwardExtension = 0.1f;
	public int totalSteps = 60;
	public float iconDistance = 0.7f;
	public float highlightedScale = 1.3f;
	public Material displaySegmentMaterial;
	public Transform parent;

	List<ControlWheelSegment> cwActions;
	List<Vector2> dividingVectors;
	List<GameObject> displaySegments;

	void Awake(){

		displaySegments = new List<GameObject> ();

	}

	public void AddControlWheelActions(ControlWheelSegment[] segs, bool overwrite = false){
		if (cwActions == null || overwrite == true) {
			cwActions = new List<ControlWheelSegment> ();
		}

		for (int i = 0; i < segs.Length; i++) {
			cwActions.Add (segs [i]);
		}

		CreateControlWheel ();
	}

	/**
	 * Used to add new actions to the wheel. Should not add more than 6 but no formal limit
	 * 
	 */ 
	public void AddControlWheelAction(ControlWheelSegment cwa, bool overwrite = false){
		if (cwActions == null) {
			cwActions = new List<ControlWheelSegment> ();
		}

		cwActions.Add (cwa);

		CreateControlWheel ();
	}

	private void CreateControlWheel(){

		foreach (GameObject seg in displaySegments) {
			Destroy (seg);
		}

		displaySegments = new List<GameObject> ();
		dividingVectors = new List<Vector2> ();

		int steps = (int)(totalSteps / cwActions.Count);
		for (int i = 0; i < cwActions.Count; i++) {
			float angle = 2 * Mathf.PI * i / cwActions.Count; 
			//shift angle to top of circle (ie by 90 degrees)
			angle += Mathf.PI /2;

			//Shift by half of the size of one area
			angle -= Mathf.PI * 2 / (cwActions.Count * 2);

			displaySegments.Add( CreateSegmentObject(angle, 2 * Mathf.PI / cwActions.Count, steps));
			AddIconToSegment(displaySegments[i], cwActions[i].Icon, angle, 2 * Mathf.PI / cwActions.Count);

			dividingVectors.Add(new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)));
		}
	}

	/**
	 * Used to show the action icons and the segments that trigger them. Will be called when the touchpad is pressed
	 */ 
	public void DisplayControlWheel(){

		foreach (GameObject seg in displaySegments) {
			seg.SetActive (true);
		}
	}

	public void HideControlWheel(){
		DelightSection ();

		foreach (GameObject seg in displaySegments) {
			seg.SetActive (false);
		}
	}

	/**
	 * Creates the segments to show where each action is mapped to on the touchpad.
	 */ 
	private GameObject CreateSegmentObject(float startAngle, float angleDist, int steps){

		GameObject seg = new GameObject ();
		seg.name = "Control Wheel Segment";

		Vector3[] verts = new Vector3[(steps + 1) * 2];

		for (int i = 0; i <= steps; i++) {
			float angle = startAngle + ((float)i / steps) * angleDist;

			float s = Mathf.Sin (angle);
			float c = Mathf.Cos (angle);

			verts[i * 2] = 		new Vector3(farRadius * c, 0f, farRadius * s);
			//verts[i * 2 + 1] = 	new Vector3(farRadius * c, 0f, farRadius * s);
			verts[i * 2 + 1] = 	new Vector3(nearRadius * c, 0f, nearRadius * s);
			//verts[i * 2 + 3] = 	new Vector3(nearRadius * c, 0f, nearRadius * s);

		}

		int[] triangles = new int[steps * 6 ];

		int vertexIndex = 0;
		//i is triangle point index
		for (int i = 0; i < triangles.Length; i += 6) {

			triangles [i] = 	vertexIndex;
			triangles [i + 1] = vertexIndex + 1;
			triangles [i + 2] = vertexIndex + 2;

			triangles [i + 3] = vertexIndex + 1;
			triangles [i + 4] = vertexIndex + 3;
			triangles [i + 5] = vertexIndex + 2;

			vertexIndex += 2;
		}

		Vector3[] normals = new Vector3[verts.Length];

		for (int i = 0; i < normals.Length; i++) {
			normals [i] = Vector3.up;
		}

		Mesh m = new Mesh ();
		m.vertices = verts;
		m.triangles = triangles;
		m.normals = normals;

		seg.AddComponent<MeshFilter> ().sharedMesh = m;
		seg.AddComponent<MeshRenderer> ().material = displaySegmentMaterial;

		seg.SetActive (false);
		seg.transform.SetParent (parent);
		seg.transform.localScale = new Vector3 (1f, 1f, 1f);
		seg.transform.localRotation = Quaternion.identity;

		float halfAngle = startAngle + angleDist / 2f;
		seg.transform.localPosition = new Vector3 ( outwardExtension * Mathf.Cos(halfAngle), 0f, outwardExtension * Mathf.Sin(halfAngle));

		return seg;
	}
		
	void AddIconToSegment(GameObject seg, Sprite icon, float startAngle, float totalAngleDist){
		float halfAngle = startAngle + totalAngleDist / 2f;

		GameObject iconGo = new GameObject ();
		iconGo.name = "Action Icon";
		iconGo.AddComponent<SpriteRenderer> ().sprite = icon;

		iconGo.transform.SetParent (seg.transform);
		iconGo.transform.Rotate (new Vector3 (90f, 0f, 0f));
		iconGo.transform.localScale = new Vector3 (1f, 1f, 1f);
		iconGo.transform.localPosition = new Vector3 ( iconDistance * Mathf.Cos(halfAngle), 0.1f, iconDistance * Mathf.Sin(halfAngle));
	}

	#region Highlighting
	GameObject prevHighlight;
	public void HighlightSectionAtLocation(Vector2 location){

		DelightSection ();

		int sectorNum = sector(location);

		if (sectorNum >= 0 && sectorNum < cwActions.Count) {
			displaySegments [sectorNum].transform.localScale = new Vector3 (highlightedScale, highlightedScale, highlightedScale);

			prevHighlight = displaySegments [sectorNum];
		}
	}

	void DelightSection(){

		if (prevHighlight != null) {
			prevHighlight.transform.localScale = new Vector3 (1f, 1f, 1f);
		}
	}
	#endregion //highlighting

	public void Select(Vector2 location) {

		int sectorNum = sector(location);

		if (sectorNum >= 0 && sectorNum < cwActions.Count) {
			cwActions [sectorNum].Action ();
		}
	}
			

	private int sector(Vector2 point)
	{
		if (cwActions.Count <= 0)
		{
			return -1;
		}

		//only one sector is an edge case, since the only point will fail our clockwise / not clockwise test
		if (cwActions.Count == 1)
		{
			return 0;
		}


		for (int i = 0; i < cwActions.Count; i++)
		{
			Vector2 startVec = dividingVectors[i];
			Vector2 endVec = dividingVectors[(i+1)%dividingVectors.Count];

			if ( areClockwise (startVec, point) == false && areClockwise(endVec, point) == true)
			{
				return (int)i;
			}
		}

		return -1;
	}

	private bool areClockwise(Vector2 v1, Vector2 v2)
	{
		return -v1.x * v2.y + v1.y * v2.x >= 0;
	}
}

public class ControlWheelSegment {

	Action action;
	public Action Action {
		get {
			return action;
		}
	}
	
	Sprite icon;
	public Sprite Icon {
		get {
			return icon;
		}
	}
						
	public ControlWheelSegment(Action action, Sprite icon){
		this.action = action;
		this.icon = icon;
	}
}