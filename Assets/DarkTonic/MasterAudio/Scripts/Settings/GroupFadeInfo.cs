using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GroupFadeInfo  {
	public MasterAudioGroup ActingGroup;
	public string NameOfGroup;
	public float TargetVolume;
	public float VolumeStep;
	public bool IsActive = true;
	public System.Action completionAction;
}
