using UnityEngine;

public interface IPlaceable {

	GameCube Cube { get; set; }
	int Cost { get; }

	string Name { get; }
	string CostStat { get; }
	string HealthStat { get; }
	string DamageStat { get; }
	string RangeStat { get; }
	string RateOfFireStat { get; }
}
