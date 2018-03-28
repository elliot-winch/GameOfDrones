using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BuildTool))]
public class BuiltToolStatsCanvas : MonoBehaviour, IHeldUpdateable {

	public GameObject canvas;

	Text[] statsUI;

	void Start(){
		canvas.SetActive (false);

		statsUI = canvas.GetComponentsInChildren<Text> (true);

		if (statsUI.Length != 5) {
			Debug.LogWarning ("Warning: Not the correct number of text fields to contain stats for turrets on build tool");
		}
	}

	public void FillStats(IPlaceable placing){
		statsUI [0].text = placing.Name;
		statsUI [1].text = "Cost: " + placing.CostStat;
		statsUI [2].text = "Health: " + placing.HealthStat;
		statsUI [3].text = "Damage: " + placing.DamageStat;
		statsUI [4].text = "Rate of Fire: " + placing.RateOfFireStat + "/s";
	}

	#region IHeldUpdateable implementation

	public void HeldStart ()
	{
		canvas.SetActive (true);
	}

	public void HeldUpdate ()
	{
		//none
	}

	public void HeldEnd ()
	{
		canvas.SetActive (false);
	}

	#endregion
}
