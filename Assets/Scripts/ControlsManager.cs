using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class ControlsManager : MonoBehaviour {

	static ControlsManager instance;

	public KeyCode[] keyboardStandins;

	public static ControlsManager Instance {
		get {
			return instance;
		}
	}

	void Start () {
		if (instance != null) {
			Debug.LogError ("Error: Cannot have two ControlsManager");
		}

		instance = this;
	}


	public HandControls GetControlsFromHand(Hand hand){

		if (hand == null || hand.controller == null) {

			return new  HandControls (
				ControlStateForKey (keyboardStandins [0]),
				ControlStateForKey (keyboardStandins [1]),
				ControlStateForKey (keyboardStandins [2]),
				new Vector2 ((Input.mousePosition.x - Screen.width / 2) / Screen.width, (Input.mousePosition.y - Screen.height / 2) / Screen.height)
			);
		} else {

			ControlState triggerPulled = new ControlState (
				hand.controller.GetHairTriggerDown,
				hand.controller.GetHairTriggerUp,
				hand.controller.GetHairTrigger
			);

			ControlState touchPadTouched = new ControlState (
				() => { return hand.controller.GetTouchDown (SteamVR_Controller.ButtonMask.Touchpad); },
				() => { return hand.controller.GetTouchUp (SteamVR_Controller.ButtonMask.Touchpad); },
				() => { return hand.controller.GetTouch (SteamVR_Controller.ButtonMask.Touchpad); }
			);
				
			ControlState touchPadPressed = new ControlState (
				() => { return hand.controller.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad); },
				() => { return hand.controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad); },
				() => { return hand.controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad); }
			);
				
			//touch pad will return value between -1 and 1, so we know anything above this is bogus
			//wish i could just null it but cant so rip
			Vector2 touchPadLocation = new Vector2 (200f, 200f);

			if(touchPadTouched.Any){
				touchPadLocation = hand.controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
			}
				
			return new HandControls (
				triggerPulled,
				touchPadTouched,
				touchPadPressed,
				touchPadLocation
			);
		}
	}

	//Helper for Hand
	ControlState ControlStateForKey(KeyCode key){

		return new ControlState (
			() => { return Input.GetKeyDown(key); },
			() => { return Input.GetKeyUp(key); },
			() => { return Input.GetKey(key); }
		);
	}
}

/*
 * This struct is to store every control input from a hand
 * 
 * A control state is defined by the below enum
 * 
 */ 

public class HandControls {

	//hand movement axis?
	private ControlState triggerPulled;
	private ControlState touchPadPressed;
	private ControlState touchPadTouched;
	private Vector2 touchPadLocation;

	public ControlState TriggerPulled { get { return this.triggerPulled; } }
	public ControlState TouchPadPressed { get { return this.touchPadPressed; } }
	public ControlState TouchPadTouched { get { return this.touchPadTouched; } }
	public Vector2 TouchPadLocation { get { return this.touchPadLocation; } }

	public HandControls(ControlState triggerPulled, ControlState touchPadTouched, ControlState touchPadPressed, Vector2 touchPadLocation){

		this.triggerPulled = triggerPulled;
		this.touchPadPressed = touchPadPressed;
		this.touchPadTouched = touchPadTouched;
		this.touchPadLocation = touchPadLocation;
	}
}

public class ControlState {

	bool down;
	bool up;
	bool held;

	public bool Down { get { return down; } } 
	public bool Up { get { return up; } } 
	public bool Held { get { return held; } } 

	public bool Any { get { return down || up || held; } }

	public ControlState(Func<bool> determineDown, Func<bool> determineUp, Func<bool> determineHeld){

		down = determineDown ();
		up = determineUp ();
		held = determineHeld ();
	}


}