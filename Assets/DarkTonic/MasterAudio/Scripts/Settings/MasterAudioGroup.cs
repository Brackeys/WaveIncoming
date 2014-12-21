using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterAudioGroup : MonoBehaviour {
	public const string NO_BUS = "[NO BUS]";
	
	public Texture logoTexture;
	public Texture settingsTexture;
	public Texture deleteTexture;
	public int busIndex = -1;
	
	public bool isExpanded = true;
	public float groupMasterVolume = 1f;
	public int retriggerPercentage = 50;
	public VariationMode curVariationMode = VariationMode.Normal;
	
	public float chainLoopDelayMin;
	public float chainLoopDelayMax;
	public ChainedLoopLoopMode chainLoopMode = ChainedLoopLoopMode.Endless;
	public int chainLoopNumLoops = 0;
	
	public VariationSequence curVariationSequence = VariationSequence.Randomized;
	public bool useInactivePeriodPoolRefill = false;
	public float inactivePeriodSeconds = 5f;
	public List<SoundGroupVariation> groupVariations = new List<SoundGroupVariation>();
	public MasterAudio.AudioLocation bulkVariationMode = MasterAudio.AudioLocation.Clip;
	public bool logSound = false;
	
	public LimitMode limitMode = LimitMode.None;
	public int limitPerXFrames = 1;
	public float minimumTimeBetween = 0.1f;
	public bool useClipAgePriority = false;
	
	public bool limitPolyphony = false;
	public int voiceLimitCount = 1;
	
	public bool isSoloed = false;
	public bool isMuted = false;
	
	private List<int> activeAudioSourcesIds = new List<int>();
	private int chainLoopCount = 0;
	
	public enum VariationSequence {
		Randomized,
		TopToBottom
	}

	public enum VariationMode {
		Normal,
		LoopedChain				
	}
	
	public enum ChainedLoopLoopMode {
		Endless,
		NumberOfLoops
	}
	
	public enum LimitMode {
		None,
		FrameBased,
		TimeBased
	}
	
	public int ActiveVoices {
		get {
			return activeAudioSourcesIds.Count;
		}
	}
	
	public void AddActiveAudioSourceId(SoundGroupVariation variation) {
		var id = variation.GetInstanceID();
		
		if (activeAudioSourcesIds.Contains(id)) {
			return;
		}
		
		activeAudioSourcesIds.Add(id);
		
		var bus = BusForGroup;
		if (bus != null) {
			bus.AddActiveAudioSourceId(id);	
		}
	}
	
	public void RemoveActiveAudioSourceId(SoundGroupVariation variation) {
		var id = variation.GetInstanceID();
		activeAudioSourcesIds.Remove(id);
		
		var bus = BusForGroup;
		if (bus != null) {
			bus.RemoveActiveAudioSourceId(id);	
		}
	}
	
	public GroupBus BusForGroup {
		get {
			if (busIndex <= MasterAudio.HARD_CODED_BUS_OPTIONS || !Application.isPlaying) {
				return null; // no bus, so no voice limit
			}
			
			return MasterAudio.GroupBuses[busIndex - MasterAudio.HARD_CODED_BUS_OPTIONS];
		}
	}
	
	public int ChainLoopCount {
		get {
			return chainLoopCount;
		}
		set {
			chainLoopCount = value;
		}
	}
}
