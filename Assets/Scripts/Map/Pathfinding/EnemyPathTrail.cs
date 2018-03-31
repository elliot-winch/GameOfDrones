using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathTrail : MonoBehaviour {

	public float moveSpeed = 2f;

	List<GameObject> allTrails;

	public void StartPath(List<GameCube> path, int index, List<GameObject> allTrails){

		if (path == null || path.Count <= 0) {
			Debug.LogWarning ("Attempting to create PathUI for null or zero length path");
		}

		this.allTrails = allTrails;
				
		StartCoroutine(CycleThroughPath(path, index));

	}

	IEnumerator CycleThroughPath(List<GameCube> path, int currentIndex){

		if (currentIndex + 1 >= path.Count) {
			allTrails.Add(EnemyPathManager.Instance.SpawnEnemyPathUI (path, 0, allTrails));

			allTrails.Remove (gameObject);

			Destroy (gameObject);
			yield break;
		}

		currentIndex++;
			
		Vector3 destinationPosition = path[currentIndex].transform.position;
		Vector3 startPos = transform.position;

		float dist = Vector3.Distance (startPos, destinationPosition);
		float movePercentage = 0f;

		while (movePercentage < 1f) {
			movePercentage += (moveSpeed * Time.deltaTime) / dist;

			transform.position = Vector3.Lerp (startPos, destinationPosition, movePercentage);

			yield return null;
		}

		StartCoroutine (CycleThroughPath (path, currentIndex));
	}	
}
