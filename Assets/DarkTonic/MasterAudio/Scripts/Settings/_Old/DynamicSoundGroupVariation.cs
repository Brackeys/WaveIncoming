using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class DynamicSoundGroupVariation {
	public bool isExpanded = true;
	public int weight = 1;
	public float volume = 1f;
	public float pitch = 1f;
	public bool loopClip = false;
	public float randomPitch = 0f;
	public float randomVolume = 0f;
	public MasterAudio.AudioLocation audLocation = MasterAudio.AudioLocation.Clip;
	public AudioClip clip;
	public string resourceFileName = string.Empty;
	public string clipName = "NEW CLIP";
	public bool useFades = false;
	public float fadeInTime = 0f;
	public float fadeOutTime = 0f;
	
	public bool showAudio3DSettings = false;
	
	//properties of Audio Source
	public AudioRolloffMode audRollOffMode = AudioRolloffMode.Logarithmic;
	public float audMinDistance = 10f;
	public float audMaxDistance = 500f;
	public float audDopplerLevel = 1f;
	public float audSpread = 0f;
	public float audPanLevel = 1f;
}
