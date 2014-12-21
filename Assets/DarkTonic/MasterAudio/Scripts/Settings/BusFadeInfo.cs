using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BusFadeInfo  {
	public string NameOfBus;
	public GroupBus ActingBus;
	public float TargetVolume;
	public float VolumeStep;
	public bool IsActive = true;
	public System.Action completionAction;
}
