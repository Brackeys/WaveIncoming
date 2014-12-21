using System;
using System.Collections;

[Serializable]
public class CustomEvent{
	public string EventName;
	public string ProspectiveName;
	
	public CustomEvent(string eventName) {
		EventName = eventName;
		ProspectiveName = eventName;
	}
}
