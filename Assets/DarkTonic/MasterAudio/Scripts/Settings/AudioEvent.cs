using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AudioEvent {
	public string soundType = string.Empty;
	public bool allPlaylistControllersForGroupCmd = false;
	public bool allSoundTypesForGroupCmd = false;
	public bool allSoundTypesForBusCmd = false;
	public float volume = 1.0f;
	public bool useFixedPitch = false;
	public float pitch = 1f;
	public bool emitParticles = false;
	public int particleCountToEmit = 1;
	public bool useLayerFilter = false;
	public bool useTagFilter = false;
	public float delaySound = 0f;
	public List<int> matchingLayers = new List<int>() { 0 };
	public List<string> matchingTags = new List<string>() { "Default" };
	public MasterAudio.EventSoundFunctionType currentSoundFunctionType = MasterAudio.EventSoundFunctionType.PlaySound;
	public MasterAudio.PlaylistCommand currentPlaylistCommand = MasterAudio.PlaylistCommand.None;
	public MasterAudio.SoundGroupCommand currentSoundGroupCommand = MasterAudio.SoundGroupCommand.None;
	public MasterAudio.BusCommand currentBusCommand = MasterAudio.BusCommand.None;
	public MasterAudio.CustomEventCommand currentCustomEventCommand = MasterAudio.CustomEventCommand.None;
	public string busName = string.Empty;
	public string playlistName = string.Empty;
	public string playlistControllerName = string.Empty;
	public bool startPlaylist = true;
	public float fadeVolume = 0f;
	public float fadeTime = 1f;
	public string clipName = "[None]";
	public EventSounds.VariationType variationType = EventSounds.VariationType.PlayRandom;
	public string variationName = string.Empty;
	
	public bool customSoundActive = false; // for custom events only
	public bool isCustomEvent = false;
	public string customEventName = string.Empty;
}
