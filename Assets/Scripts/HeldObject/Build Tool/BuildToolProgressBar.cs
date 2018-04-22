using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildToolProgressBar : MonoBehaviour {

	public float deadHoldToActTime = 0.05f;
	public float holdToActTime = 1f;
	private float actOnCubeTimer = 0f;

	public Canvas progressBarCanvas;

	private Slider actTimeSlider; 

	public float ActOnCubeTimer {
		get {
			return actOnCubeTimer;
		}
		set {
			actOnCubeTimer = value;

			if (actOnCubeTimer > deadHoldToActTime) {

				if (actTimeSlider.gameObject.activeSelf == false) {

					actTimeSlider.gameObject.SetActive(true);
						
					actTimeSlider.value = actOnCubeTimer;
				}

				actTimeSlider.value = actOnCubeTimer;
			} else {
				actTimeSlider.gameObject.SetActive(false);
			}
		}
	}

	void Start(){
		actTimeSlider = progressBarCanvas.GetComponentInChildren<Slider>(true);
		actTimeSlider.maxValue = holdToActTime;
		actTimeSlider.minValue = deadHoldToActTime;

		actTimeSlider.gameObject.SetActive(false);
	}
}
