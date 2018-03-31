using UnityEngine;

public interface IPlaceable {

	GameCube Cube { get; set; }
	int BuildCost { get; }
	float MoveCost { get; }
}
