using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MA_SampleICustomEventReceiver : MonoBehaviour, ICustomEventReceiver {
	private List<string> eventsSubscribedTo = new List<string>() {
		"PlayerMoved",
		"PlayerStoppedMoving"
	};
	
	private GameObject go;
	
	void Awake() {
		go = this.gameObject;
	}
	
	void Start() {
		CheckForIllegalCustomEvents();
	}
	
	// Use this for initialization
	void OnEnable() {
		RegisterReceiver();
	}
	
	void OnDisable() {
		UnregisterReceiver();
	}
	
	
	#region ICustomEventReceiver methods
	public void CheckForIllegalCustomEvents() {
		// this is totally optional, but implementing this method will save you debugging time because you will know right away if your event(s) don't exist if you call this in Start.
		for (var i = 0; i < eventsSubscribedTo.Count; i++) {
			var eventName = eventsSubscribedTo[i];
			if (!MasterAudio.CustomEventExists(eventName)) {
				Debug.LogError("Custom Event, listened to by '" + this.name + "', could not be found in MasterAudio.");
			}
		}
	}
	
	public void ReceiveEvent(string customEventName) {
		switch (customEventName) {
			case "PlayerMoved":
				Debug.Log("PlayerMoved event recieved by '" + this.name + "'.");
				break;
			case "PlayerStoppedMoving":
				Debug.Log("PlayerStoppedMoving event recieved by '" + this.name + "'.");
				break;
		}
	}
	
	public bool SubscribesToEvent(string customEventName) {
		if (string.IsNullOrEmpty(customEventName)) {
			return false;
		}
		
		return eventsSubscribedTo.Contains(customEventName);
	}
	
	public void RegisterReceiver() {
		MasterAudio.AddCustomEventReceiver(this, go);
	}
	
	public void UnregisterReceiver() {
		MasterAudio.RemoveCustomEventReceiver(this);
	}
	#endregion
}
