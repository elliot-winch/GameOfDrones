using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath {

	Stack<GameCube> validPath;

	public bool IsComplete {
		get {
			return validPath != null;
		}
	}
		
	public AStarPath(GameCube start, GameCube end){

		ConceptualGrid grid = GameCubeManager.Instance.Grid;
		List<GameCube> allCubes = grid.AllCubes;

		List<GameCube> closedSet = new List<GameCube> ();

		PriorityQueue<float, GameCube> openSet = new PriorityQueue<float, GameCube> ();
		openSet.Enqueue (0, start);

		//Essentially a linked list
		Dictionary<GameCube, GameCube> path = new Dictionary<GameCube, GameCube> ();

		Dictionary<GameCube, float> g_score = new Dictionary<GameCube, float> ();

		foreach (GameCube h in allCubes) {
			g_score [h] = Mathf.Infinity;
		}

		g_score [start] = 0;

		Dictionary<GameCube, float> f_score = new Dictionary<GameCube, float> ();

		foreach (GameCube h in allCubes) {
			f_score [h] = Mathf.Infinity;
		}

		f_score [start] = heuristicCostEstimate (start, end);

		while (!openSet.IsEmpty) {
			GameCube current = openSet.Dequeue ().Value;

			if (current == end) {
				RecontructPath (path, current);
				return;
			}

			closedSet.Add (current);

			List<GameCube> neighbours = grid.GetNeighbours (current);

			foreach (GameCube neighbour in neighbours) {

				if (neighbour.MoveCost == Mathf.Infinity) {
					Debug.Log ("Encounted unpassable");
					continue;
				}

				if(closedSet.Contains(neighbour)){
					continue;
				}
					

				float tentative_g_score = g_score [current] + current.MoveCost;

				if (openSet.Contains (neighbour) && tentative_g_score >= g_score [neighbour]) {
					continue;
				}

				path [neighbour] = current;
				g_score [neighbour] = tentative_g_score;
				f_score[neighbour] = g_score [neighbour] + heuristicCostEstimate (neighbour, end);

				if (openSet.Contains (neighbour) == false) {
					openSet.Enqueue (f_score [neighbour], neighbour);
				}
			}
		}

		//if we reach this case, it means all nodes have been closed by final destination wasn't found
	}

	float heuristicCostEstimate(GameCube a, GameCube b){
		return Vector3.Distance (a.Position, b.Position);
	}

	void RecontructPath(Dictionary<GameCube, GameCube> path, GameCube current){

		Debug.Log ("Found a valid path");
		validPath = new Stack<GameCube> ();
		Queue<GameCube> pathQueue = new Queue<GameCube> ();

		pathQueue.Enqueue (current);

		while(path.ContainsKey(current)) {
			current = path [current];
			pathQueue.Enqueue (current);
		}

		while (pathQueue.Count > 1) {
			validPath.Push (pathQueue.Dequeue ());
		}
	}

	public GameCube GetNext(){
		if (validPath.Count > 0) {
			return validPath.Pop ();
		} else {
			return null;
		}
	}

	public bool IsNext(){
		return validPath != null && validPath.Count > 0;
	}

	public int Length(){
		return validPath.Count;
	}
}
