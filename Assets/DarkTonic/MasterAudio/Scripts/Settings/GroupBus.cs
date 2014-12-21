using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GroupBus {
	public string busName;
	public float volume = 1.0f;
	public bool isSoloed = false;
	public bool isMuted = false;
	public int voiceLimit = -1;
	public bool isExisting = false; // for Dynamic Sound Group - referenced Buses
	
	private List<int> activeAudioSourcesIds = new List<int>();
	
	public void AddActiveAudioSourceId(int id) {
		if (activeAudioSourcesIds.Contains(id)) {
			return;
		}
		
		activeAudioSourcesIds.Add(id);
	}
	
	public void RemoveActiveAudioSourceId(int id) {
		activeAudioSourcesIds.Remove(id);
	}
	
	public int ActiveVoices {
		get {
			return activeAudioSourcesIds.Count;
		}
	}
	
	public bool BusVoiceLimitReached {
		get {
			if (voiceLimit <= 0) { 
				return false; // no limit set
			}
			
			return activeAudioSourcesIds.Count >= voiceLimit;
		}
	}
}
