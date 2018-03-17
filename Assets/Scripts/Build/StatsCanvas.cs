using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsCanvas : MonoBehaviour {

	Text[] statsUI;

	public void FillStats(IPlaceable placing){

		//lazy instantiation
		if (statsUI == null) {
			statsUI = GetComponentsInChildren<Text> (true);

			if (statsUI.Length != 5) {
				Debug.LogWarning ("Warning: Not the correct number of text fields to contain stats for turrets on build tool");
			}
		}

		statsUI [0].text = placing.Name;
		statsUI [1].text = "Cost: " + placing.CostStat;
		statsUI [2].text = "Health: " + placing.HealthStat;
		statsUI [3].text = "Damage: " + placing.DamageStat;
		statsUI [4].text = "Rate of Fire: " + placing.RateOfFireStat + "/s";
	}

	public void FillStats(int id){

		IPlaceable placing = BuildManager.Instance.buildables [id].GetComponent<IPlaceable>();

		FillStats (placing);
	}
}
