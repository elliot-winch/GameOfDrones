using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class AStarPath {

	Stack<GameCube> validPath;

	public AStarPath(Grid grid, GameCube startHex, GameCube endHex, bool hexValuesAffectPath){

		List<GameCube> closedSet = new List<GameCube> ();

		PriorityQueue<float, GameCube> openSet = new PriorityQueue<float, GameCube> ();
		openSet.Enqueue (0, startHex);

		Dictionary<GameCube, GameCube> path = new Dictionary<GameCube, GameCube> ();

		Dictionary<GameCube, float> g_score = new Dictionary<GameCube, float> ();

		foreach (GameCube h in grid.Hexes.Values) {
			g_score [h] = Mathf.Infinity;
		}

		g_score [startHex] = 0;

		Dictionary<GameCube, float> f_score = new Dictionary<GameCube, float> ();

		foreach (GameCube h in grid.Hexes.Values) {
			f_score [h] = Mathf.Infinity;
		}

		f_score [startHex] = heuristicCostEstimate (startHex, endHex);

		while (!openSet.IsEmpty) {
			GameCube current = openSet.Dequeue ().Value;

			if (current == endHex) {
				RecontructPath (path, current);
				return;
			}

			closedSet.Add (current);

			List<GameCube> neighbours = grid.HexNeighbours (current.GetComponent<Tile>());

			foreach (GameCube neighbour in neighbours) {

				if (neighbour.MoveCost == Mathf.Infinity || neighbour.OccupantBlocksMovement) {
					continue;
				}

				if(closedSet.Contains(neighbour)){
					continue;
				}
					

				float tentative_g_score = g_score [current] + (hexValuesAffectPath ? current.MoveCost : 1f);

				if (openSet.Contains (neighbour) && tentative_g_score >= g_score [neighbour]) {
					continue;
				}

				path [neighbour] = current;
				g_score [neighbour] = tentative_g_score;
				f_score[neighbour] = g_score [neighbour] + heuristicCostEstimate (neighbour, endHex);

				if (openSet.Contains (neighbour) == false) {
					openSet.Enqueue (f_score [neighbour], neighbour);
				}
			}
		}
	}

	float heuristicCostEstimate(GameCube a, GameCube b){
		return Grid.Distance (a.GetComponent<Tile>(), b.GetComponent<Tile>());
	}

	void RecontructPath(Dictionary<GameCube, GameCube> path, GameCube current){

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

	public GameCube GetNextHex(){
		if (validPath.Count > 0) {
			return validPath.Pop ();
		} else {
			return null;
		}
	}

	public bool IsNextHex(){
		return validPath != null && validPath.Count > 0;
	}

	public int Length(){
		return validPath.Count;
	}
}
*/