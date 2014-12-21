using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicSoundGroup : MonoBehaviour {
	public Texture logoTexture;
	public Texture settingsTexture;
	public Texture deleteTexture;
	public Texture playTexture;
	public Texture stopTrackTexture;
	public GameObject variationTemplate;
	
	public float groupMasterVolume = 1f;
	public int retriggerPercentage = 50;
	public MasterAudioGroup.VariationSequence curVariationSequence = MasterAudioGroup.VariationSequence.Randomized;
	public bool useInactivePeriodPoolRefill = false;
	public float inactivePeriodSeconds = 5f;
	public MasterAudioGroup.VariationMode curVariationMode = MasterAudioGroup.VariationMode.Normal;
	public MasterAudio.AudioLocation bulkVariationMode = MasterAudio.AudioLocation.Clip;
	
	public float chainLoopDelayMin;
	public float chainLoopDelayMax;
	public MasterAudioGroup.ChainedLoopLoopMode chainLoopMode = MasterAudioGroup.ChainedLoopLoopMode.Endless;
	public int chainLoopNumLoops = 0;
	
	public bool logSound = false;
	public int busIndex = -1;
	public string busName = string.Empty; // only used to remember the bus name during group creation.
	
	public MasterAudioGroup.LimitMode limitMode = MasterAudioGroup.LimitMode.None;
	public int limitPerXFrames = 1;
	public float minimumTimeBetween = 0.1f;
	public bool limitPolyphony = false;
	public int voiceLimitCount = 1;
	
	public List<DynamicGroupVariation> groupVariations = new List<DynamicGroupVariation>(); // filled and used by Inspector only
}
