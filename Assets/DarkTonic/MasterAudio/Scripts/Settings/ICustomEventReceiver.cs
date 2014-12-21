using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICustomEventReceiver { // this interface is used to "listen" to custom events that MasterAudio transmits.
	/// <summary>
	/// This checks for events that are not found in MasterAudio. It's a good idea to call this in Start (Awake is too early), and save yourself some troubleshooting time! Optional
	/// </summary>
	void CheckForIllegalCustomEvents();
	
	/// <summary>
	/// This receives the event when it's fired.
	/// </summary>
	void ReceiveEvent(string customEventName);
	
	/// <summary>
	/// This returns a bool of whether the specified custom event is subscribed to in this class
	/// </summary>
	bool SubscribesToEvent(string customEventName);
	
	/// <summary>
	/// Registers the receiver with MasterAudio. Call this in OnEnable
	/// </summary>
	void RegisterReceiver();
	
	/// <summary>
	/// Unregisters the receiver with MasterAudio. Call this in OnDisable
	/// </summary>
	void UnregisterReceiver();
}
