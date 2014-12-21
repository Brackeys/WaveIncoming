using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DynamicSoundGroupInfo {
	public const string NEW_GROUP_START_NAME = "NEW GROUP";
	
	public bool isExpanded = true;
	public bool groupSettingsExpanded = true;
	public string groupName = NEW_GROUP_START_NAME;
	public bool duckSound = false;
	public float riseVolStart = .5f;
	public BusMode busMode = BusMode.NoBus;
	public string busName = "EXISTING OR NEW BUS NAME";
	public List<DynamicSoundGroupVariation> variations = new List<DynamicSoundGroupVariation>();

	public float groupMasterVolume = 1f;
	public int retriggerPercentage = 50;
	public MasterAudioGroup.VariationSequence curVariationSequence = MasterAudioGroup.VariationSequence.Randomized;
	public bool useInactivePeriodPoolRefill;
	public float inactivePeriodSeconds = 0f;
	
	public MasterAudioGroup.VariationMode curVariationMode = MasterAudioGroup.VariationMode.Normal;
	public MasterAudioGroup.LimitMode limitMode = MasterAudioGroup.LimitMode.None;
	public int limitPerXFrames = 1;
	public float minimumTimeBetween = 0.1f;
	public bool limitPolyphony = false;
	public int voiceLimitCount = 1;
	
	public enum BusMode {
		NoBus,
		UseExisting,
		CreateNew
	}
}
