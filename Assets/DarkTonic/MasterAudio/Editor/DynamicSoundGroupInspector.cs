using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DynamicSoundGroup))]
public class DynamicSoundGroupInspector : Editor {
	private DynamicSoundGroup _group;
	private bool isValid = true;
	
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		
		EditorGUI.indentLevel = 0;
		var isDirty = false;
		
		_group = (DynamicSoundGroup)target;
		
		_group = RescanChildren(_group);
		
		var dsgc = _group.transform.parent;
		DynamicSoundGroupCreator creator = null;
		if (dsgc != null) {
			creator = dsgc.GetComponent<DynamicSoundGroupCreator>();
		}
		
		if (creator == null) {
			isValid = false;
		}
		
		if (!isValid) {
			return;
		}

		var isInProjectView = DTGUIHelper.IsPrefabInProjectView(_group);
		
		if (_group.logoTexture != null) {
			DTGUIHelper.ShowHeaderTexture(_group.logoTexture);
		}
		
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
		GUI.contentColor = Color.green;
		if (GUILayout.Button(new GUIContent("Back to Dynamic Sound Group Creator", "Select Group in Hierarchy"), EditorStyles.toolbarButton, GUILayout.Width(220))) {
			Selection.activeObject = _group.transform.parent.gameObject;
		}	
		GUI.contentColor = Color.white;
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		
		var newVol = EditorGUILayout.Slider("Group Master Volume", _group.groupMasterVolume, 0f, 1f);
		if (newVol != _group.groupMasterVolume) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "change Group Master Volume");
			_group.groupMasterVolume = newVol;
		}
		
		var newRetrigger = EditorGUILayout.IntSlider("Retrigger Percentage", _group.retriggerPercentage, 0, 100);
		if (newRetrigger != _group.retriggerPercentage) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "change Retrigger Percentage");
			_group.retriggerPercentage = newRetrigger;
		}
		
		var newVarSequence = (MasterAudioGroup.VariationSequence) EditorGUILayout.EnumPopup("Variation Sequence", _group.curVariationSequence);
		if (newVarSequence != _group.curVariationSequence) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "change Variation Sequence");
			_group.curVariationSequence = newVarSequence;
		}

		if (_group.curVariationSequence == MasterAudioGroup.VariationSequence.TopToBottom) {
			var newUseInactive = EditorGUILayout.BeginToggleGroup("Refill Variation Pool After Inactive Time", _group.useInactivePeriodPoolRefill);
			if (newUseInactive != _group.useInactivePeriodPoolRefill) {
				UndoHelper.RecordObjectPropertyForUndo(_group, "toggle Inactive Refill");
				_group.useInactivePeriodPoolRefill = newUseInactive;
			}

			EditorGUI.indentLevel = 1;
			var newInactivePeriod = EditorGUILayout.Slider("Inactive Time (sec)", _group.inactivePeriodSeconds, .2f, 30f);
			if (newInactivePeriod != _group.inactivePeriodSeconds) {
				UndoHelper.RecordObjectPropertyForUndo(_group, "change Inactive Time");
				_group.inactivePeriodSeconds = newInactivePeriod;
			}

			EditorGUILayout.EndToggleGroup();
		}

		EditorGUI.indentLevel = 0;
		var newVarMode = (MasterAudioGroup.VariationMode) EditorGUILayout.EnumPopup("Variation Mode", _group.curVariationMode);
		if (newVarMode != _group.curVariationMode) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "change Variation Mode");
			_group.curVariationMode = newVarMode;
		}
		
		if (_group.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain) {
			DTGUIHelper.ShowColorWarning("*In this mode, only one Variation can be played at a time.");
			EditorGUI.indentLevel = 1;
			
			var newLoopMode = (MasterAudioGroup.ChainedLoopLoopMode) EditorGUILayout.EnumPopup("Loop Mode", _group.chainLoopMode);
			if (newLoopMode != _group.chainLoopMode) {
				UndoHelper.RecordObjectPropertyForUndo(_group, "change Loop Mode");
				_group.chainLoopMode = newLoopMode;
			}
			
			if (_group.chainLoopMode == MasterAudioGroup.ChainedLoopLoopMode.NumberOfLoops) {
				var newLoopCount = EditorGUILayout.IntSlider("Number of Loops", _group.chainLoopNumLoops, 1, 500);
				if (newLoopCount != _group.chainLoopNumLoops) {
					UndoHelper.RecordObjectPropertyForUndo(_group, "change Number of Loops");
					_group.chainLoopNumLoops = newLoopCount;
				}
			}
			
			var newDelayMin = EditorGUILayout.Slider("Clip Change Delay Min", _group.chainLoopDelayMin, 0f, 20f);
			if (newDelayMin != _group.chainLoopDelayMin) {
				if (_group.chainLoopDelayMax < newDelayMin) {
					_group.chainLoopDelayMax = newDelayMin;
				}
				UndoHelper.RecordObjectPropertyForUndo(_group, "change Chained Clip Delay Min");
				_group.chainLoopDelayMin = newDelayMin;
			}
			
			var newDelayMax = EditorGUILayout.Slider("Clip Change Delay Max", _group.chainLoopDelayMax, 0f, 20f);
			if (newDelayMax != _group.chainLoopDelayMax) {
				if (newDelayMax < _group.chainLoopDelayMin) {
					newDelayMax = _group.chainLoopDelayMin;
				}
				UndoHelper.RecordObjectPropertyForUndo(_group, "change Chained Clip Delay Max");
				_group.chainLoopDelayMax = newDelayMax;
			}
		}
		
		if (_group.curVariationMode == MasterAudioGroup.VariationMode.Normal) {
			var newLimitPoly = EditorGUILayout.Toggle("Limit Polyphony", _group.limitPolyphony);
			if (newLimitPoly != _group.limitPolyphony) {
				UndoHelper.RecordObjectPropertyForUndo(_group, "toggle Limit Polyphony");
				_group.limitPolyphony = newLimitPoly;
			}
			if (_group.limitPolyphony) {
				int maxVoices = 0;
				for (var i = 0; i < _group.groupVariations.Count; i++) {
					var variation = _group.groupVariations[i];
					maxVoices += variation.weight;
				}
				
				var newVoiceLimit = EditorGUILayout.IntSlider("Polyphony Voice Limit", _group.voiceLimitCount, 1, maxVoices);
				if (newVoiceLimit != _group.voiceLimitCount) {
					UndoHelper.RecordObjectPropertyForUndo(_group, "change Polyphony Voice Limit");
					_group.voiceLimitCount = newVoiceLimit;
				}
			}
			
			var newLimitMode = (MasterAudioGroup.LimitMode) EditorGUILayout.EnumPopup("Replay Limit Mode", _group.limitMode);
			if (newLimitMode != _group.limitMode) {
				UndoHelper.RecordObjectPropertyForUndo(_group, "change Replay Limit Mode");
				_group.limitMode = newLimitMode;
			}
			
			switch (_group.limitMode) {
			case MasterAudioGroup.LimitMode.FrameBased:
				var newFrameLimit = EditorGUILayout.IntSlider("Min Frames Between", _group.limitPerXFrames, 1, 120);
				if (newFrameLimit != _group.limitPerXFrames) {
					UndoHelper.RecordObjectPropertyForUndo(_group, "change Min Frames Between");
					_group.limitPerXFrames = newFrameLimit;
				}
				break;
			case MasterAudioGroup.LimitMode.TimeBased:
				var newMinTime = EditorGUILayout.Slider("Min Seconds Between", _group.minimumTimeBetween, 0.1f, 10f);
				if (newMinTime != _group.minimumTimeBetween) {
					UndoHelper.RecordObjectPropertyForUndo(_group, "change Min Seconds Between");
					_group.minimumTimeBetween = newMinTime;
				}
				break;
			}
		}
		
		var newBulkMode = (MasterAudio.AudioLocation) EditorGUILayout.EnumPopup("Variation Create Mode", _group.bulkVariationMode);
		if (newBulkMode != _group.bulkVariationMode) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "change Bulk Variation Mode");
			_group.bulkVariationMode = newBulkMode;
		}
		if (_group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
			DTGUIHelper.ShowColorWarning("*Resource mode: make sure to drag from Resource folders only.");
		}
		
		var newLog = EditorGUILayout.Toggle("Log Sounds", _group.logSound);
		if (newLog != _group.logSound) {
			UndoHelper.RecordObjectPropertyForUndo(_group, "toggle Log Sounds");
			_group.logSound = newLog;
		}
		
		int? deadChildIndex = null;
		
		if (!Application.isPlaying) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(4);
			GUILayout.Label("Actions", EditorStyles.wordWrappedLabel, GUILayout.Width(50f));
			GUILayout.Space(96);
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Equalize Weights", "Reset Weights to one"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
				isDirty = true;
				EqualizeWeights(_group);
			}	
			
			GUILayout.Space(10);
			if (GUILayout.Button(new GUIContent("Equalize Variation Volumes"), EditorStyles.toolbarButton, GUILayout.Width(150))) {
				EqualizeVariationVolumes(_group.groupVariations);
			}
			
			GUI.contentColor = Color.white;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		
		if (!Application.isPlaying) {
			// new variation settings
			EditorGUILayout.BeginVertical();
			var anEvent = Event.current;
			
			if (isInProjectView) {
				DTGUIHelper.ShowLargeBarAlert("*You are in Project View and cannot create Variations.");
				DTGUIHelper.ShowLargeBarAlert("*Pull this prefab into the Scene to create Variations.");
			} else {
				GUI.color = Color.yellow;
				
				var dragArea = GUILayoutUtility.GetRect(0f,35f,GUILayout.ExpandWidth(true));
				GUI.Box (dragArea, "Drag Audio clips here to create Variations!");
				
				GUI.color = Color.white;
				
				switch (anEvent.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if(!dragArea.Contains(anEvent.mousePosition)) {
						break;
					}
					
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if(anEvent.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag();
						
						foreach (var dragged in DragAndDrop.objectReferences) {
							var aClip = dragged as AudioClip;
							if(aClip == null) {
								continue;
							}
							
							CreateVariation(_group, aClip);
						}
					}
					Event.current.Use();
					break;
				}
			}
			EditorGUILayout.EndVertical();
			// end new variation settings
		}
		
		if (_group.groupVariations.Count == 0) {
			DTGUIHelper.ShowRedError("You currently have no Variations.");
		} else {
			_group.groupVariations.Sort(delegate(DynamicGroupVariation x, DynamicGroupVariation y) {
				return x.name.CompareTo(y.name);	
			});
			
			for (var i = 0; i < _group.groupVariations.Count; i++) {
				var variation = _group.groupVariations[i];
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				EditorGUILayout.LabelField(variation.name, EditorStyles.boldLabel);
				
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(new GUIContent(_group.settingsTexture, "Click to goto Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
					Selection.activeObject = variation; 
				}
				
				if (!Application.isPlaying) {
					if (GUILayout.Button(new GUIContent(_group.deleteTexture, "Click to delete this Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
						deadChildIndex = i;
						isDirty = true;
					}
				}
				
				var buttonPressed = DTGUIHelper.AddDynamicGroupButtons(_group);
				switch (buttonPressed) {
					case DTGUIHelper.DTFunctionButtons.Play:
						if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
							creator.PreviewerInstance.Stop();
							creator.PreviewerInstance.PlayOneShot(Resources.Load(variation.resourceFileName) as AudioClip);
						} else {
							variation.audio.Stop();
							variation.audio.Play();
						}
						isDirty = true;
						break;
					case DTGUIHelper.DTFunctionButtons.Stop:
						if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
							creator.PreviewerInstance.Stop();
						} else {
							variation.audio.Stop();
						}
						isDirty = true;
						break;
				}
				
				EditorGUILayout.EndHorizontal();
				
				if (!Application.isPlaying) {
					DTGUIHelper.ShowColorWarning("*Fading & random settings are ignored by preview in edit mode.");
				}
				if (variation.audio == null) {
					DTGUIHelper.ShowRedError(string.Format("The Variation: '{0}' has no Audio Source.", variation.name));
					break;
				}
				
				var oldLocation = variation.audLocation;
				var newLocation = (MasterAudio.AudioLocation) EditorGUILayout.EnumPopup("Audio Origin", variation.audLocation);
				if (newLocation != variation.audLocation) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "change Audio Origin");
					variation.audLocation = newLocation;
				}
				 
				switch (variation.audLocation) {
					case MasterAudio.AudioLocation.Clip:
						var newClip = (AudioClip) EditorGUILayout.ObjectField("Audio Clip", variation.audio.clip, typeof(AudioClip), false);
						if (newClip != variation.audio.clip) {
							UndoHelper.RecordObjectPropertyForUndo(variation.audio, "change Audio Clip");
							variation.audio.clip = newClip;
						}
						break;
					case MasterAudio.AudioLocation.ResourceFile:
						if (oldLocation != variation.audLocation) {
							if (variation.audio.clip != null) {
								Debug.Log("Audio clip removed to prevent unnecessary memory usage on Resource file Variation.");
							}
							variation.audio.clip = null;
						}
						
						EditorGUILayout.BeginVertical();
						var anEvent = Event.current;
						
						GUI.color = Color.yellow;
						var dragArea = GUILayoutUtility.GetRect(0f, 20f,GUILayout.ExpandWidth(true));
						GUI.Box (dragArea, "Drag Resource Audio clip here to use its name!");
						GUI.color = Color.white;
						
						switch (anEvent.type) {
						case EventType.DragUpdated:
						case EventType.DragPerform:
							if(!dragArea.Contains(anEvent.mousePosition)) {
								break;
							}
							
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							
							if(anEvent.type == EventType.DragPerform) {
								DragAndDrop.AcceptDrag();
								
								foreach (var dragged in DragAndDrop.objectReferences) {
									var aClip = dragged as AudioClip;
									if(aClip == null) {
										continue;
									}
									
									UndoHelper.RecordObjectPropertyForUndo(variation, "change Resource Filename");
									variation.resourceFileName = aClip.name;
								}
							}
							Event.current.Use();
							break;
						}
						EditorGUILayout.EndVertical();
						
						var newFilename = EditorGUILayout.TextField("Resource Filename", variation.resourceFileName);		
						if (newFilename != variation.resourceFileName) {
							UndoHelper.RecordObjectPropertyForUndo(variation, "change Resource Filename");
							variation.resourceFileName = newFilename;
						}
						break;
				}
				
				var newVolume = EditorGUILayout.Slider("Volume", variation.audio.volume, 0f, 1f);
				if (newVolume != variation.audio.volume) {
					UndoHelper.RecordObjectPropertyForUndo(variation.audio, "change Volume");
					variation.audio.volume = newVolume;
				}
				
				var newPitch = EditorGUILayout.Slider("Pitch", variation.audio.pitch, -3f, 3f);
				if (newPitch != variation.audio.pitch) {
					UndoHelper.RecordObjectPropertyForUndo(variation.audio, "change Pitch");
					variation.audio.pitch = newPitch;
				}
				
				var newLoop = EditorGUILayout.Toggle("Loop Clip", variation.audio.loop);
				if (newLoop != variation.audio.loop) {
					UndoHelper.RecordObjectPropertyForUndo(variation.audio, "toggle Loop Clip");
					variation.audio.loop = newLoop;
				}
				
				EditorUtility.SetDirty(variation.audio);
				
				var newRandomPitch = EditorGUILayout.Slider("Random Pitch", variation.randomPitch, 0f, 3f);
				if (newRandomPitch != variation.randomPitch) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "change Random Pitch");
					variation.randomPitch = newRandomPitch;
				}
				
				var newRandomVol = EditorGUILayout.Slider("Random Volume", variation.randomVolume, 0f, 1f);
				if (newRandomVol != variation.randomVolume) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "change Random Volume");
					variation.randomVolume = newRandomVol;
				}
				
				var newWeight = EditorGUILayout.IntSlider("Weight (Instances)", variation.weight, 0, 100);
				if (newWeight != variation.weight) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "change Weight");
					variation.weight = newWeight;
				}
				
				if (variation.HasActiveFXFilter) {
					var newFxTailTime = EditorGUILayout.Slider("FX Tail Time", variation.fxTailTime, 0f, 10f);
					if (newFxTailTime != variation.fxTailTime) {
						UndoHelper.RecordObjectPropertyForUndo(variation, "change FX Tail Time");
						variation.fxTailTime = newFxTailTime;
					}
				}
				
				var newSilence = EditorGUILayout.BeginToggleGroup("Use Random Delay", variation.useIntroSilence);
				if (newSilence != variation.useIntroSilence) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "toggle Use Random Delay");
					variation.useIntroSilence = newSilence;
				}
				
				if (variation.useIntroSilence) {
					var newSilenceMin = EditorGUILayout.Slider("Delay Min (sec)", variation.introSilenceMin, 0f, 100f);
					if (newSilenceMin != variation.introSilenceMin) {
						UndoHelper.RecordObjectPropertyForUndo(variation, "change Delay Min (sec)");
						variation.introSilenceMin = newSilenceMin;
						if (variation.introSilenceMin > variation.introSilenceMax) {
							variation.introSilenceMax = newSilenceMin;
						}
					}
					
					var newSilenceMax = EditorGUILayout.Slider("Delay Max (sec)", variation.introSilenceMax, 0f, 100f);
					if (newSilenceMax != variation.introSilenceMax) {
						UndoHelper.RecordObjectPropertyForUndo(variation, "change Delay Max (sec)");
						variation.introSilenceMax = newSilenceMax;
						if (variation.introSilenceMax < variation.introSilenceMin) {
							variation.introSilenceMin = newSilenceMax;
						}
					}
				}
				
				EditorGUILayout.EndToggleGroup();
				
				var newFades = EditorGUILayout.BeginToggleGroup("Use Custom Fading", variation.useFades);
				if (newFades != variation.useFades) {
					UndoHelper.RecordObjectPropertyForUndo(variation, "toggle Use Custom Fading");
					variation.useFades = newFades;
				}
				
				if (variation.useFades) {
					var newFadeIn = EditorGUILayout.Slider("Fade In Time (sec)", variation.fadeInTime, 0f, 10f);
					if (newFadeIn != variation.fadeInTime) {
						UndoHelper.RecordObjectPropertyForUndo(variation, "change Fade In Time");
						variation.fadeInTime = newFadeIn;
					}
					
					var newFadeOut = EditorGUILayout.Slider("Fade Out time (sec)", variation.fadeOutTime, 0f, 10f);
					if (newFadeOut != variation.fadeOutTime) {
						UndoHelper.RecordObjectPropertyForUndo(variation, "change Fade Out Time");
						variation.fadeOutTime = newFadeOut;
					}
				}
				EditorGUILayout.EndToggleGroup();
				
				EditorGUILayout.Separator();
			}
		}
		
		if (deadChildIndex.HasValue) {
			var deadVar = _group.groupVariations[deadChildIndex.Value];
			
			if (deadVar != null) {
				// delete variation from Hierarchy
				UndoHelper.DestroyForUndo(deadVar.gameObject);
			}
			
			// delete group.
			_group.groupVariations.RemoveAt(deadChildIndex.Value);
		}
		
		
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}	
		
		this.Repaint();

		//DrawDefaultInspector();
    }
	
	private DynamicSoundGroup RescanChildren(DynamicSoundGroup group) {
		var newChildren = new List<DynamicGroupVariation>();
		
		var childNames = new List<string>();
		
		DynamicGroupVariation variation = null;
		
		for (var i = 0; i < group.transform.childCount; i++) {
			var child = group.transform.GetChild(i);
			
			if (!Application.isPlaying) {
				if (childNames.Contains(child.name)) {
					DTGUIHelper.ShowRedError("You have more than one Variation named: " + child.name + ".");
					DTGUIHelper.ShowRedError("Please ensure each Variation of this Group has a unique name.");
					isValid = false;
					return null;
				}
			}
			
			childNames.Add(child.name);
			
			variation = child.GetComponent<DynamicGroupVariation>();
			
			newChildren.Add(variation);
		}
		
		group.groupVariations = newChildren;
		return group;
	}

	public void EqualizeWeights(DynamicSoundGroup _group) {
		var variations = new DynamicGroupVariation[_group.groupVariations.Count];
		
		DynamicGroupVariation variation = null;
		for (var i = 0; i < _group.groupVariations.Count; i++) {
			variation = _group.groupVariations[i];
			variations[i] = variation;
		}
		
		UndoHelper.RecordObjectsForUndo(variations, "Equalize Weights");
		
		foreach (var vari in variations) {
			vari.weight = 1;
		}
	}
	
	private void EqualizeVariationVolumes(List<DynamicGroupVariation> variations) {
		var clips = new Dictionary<DynamicGroupVariation, float>();
		
		if (variations.Count < 2) {
			DTGUIHelper.ShowAlert("You must have at least 2 Variations to use this function.");
			return;
		}
		
		float lowestVolume = 1f;
		
		for (var i = 0; i < variations.Count; i++) {
			var setting = variations[i];
			
			AudioClip ac = null;
			
			switch (setting.audLocation) {
				case MasterAudio.AudioLocation.Clip:
					if (setting.audio.clip == null) {
						continue;
					}
					ac = setting.audio.clip;
					break;
				case MasterAudio.AudioLocation.ResourceFile:
					if (string.IsNullOrEmpty(setting.resourceFileName)) {
						continue;
					}
					
					ac = Resources.Load(setting.resourceFileName) as AudioClip;	
					
					if (ac == null) {
						continue; // bad resource path
					}
					break;
			}

			if (!ac.isReadyToPlay) {
				Debug.Log("Clip is not ready to play (streaming?). Skipping '" + setting.name + "'.");
				continue;
			}
			
			float average = 0f;
			var buffer = new float[ac.samples];
			
			Debug.Log("Measuring amplitude of '" + ac.name + "'.");
			
			ac.GetData(buffer, 0);
			
			for(int c = 0; c < ac.samples; c++){
				average += Mathf.Pow(buffer[c], 2);
			}
			
			average = Mathf.Sqrt(1f/(float)ac.samples*average);
			
			if (average < lowestVolume) {
				lowestVolume = average;
			}
		
			if (average == 0f) {
				// don't factor in.
				continue;
			}
			clips.Add (setting, average);
		}
		
		if (clips.Count < 2) {
			DTGUIHelper.ShowAlert("You must have at least 2 Variations with non-compressed, non-streaming clips to use this function.");
			return;
		}
		
		foreach (var kv in clips) { 
			if (kv.Value == 0) {
				// skip
				continue;
			}
			float adjustedVol = lowestVolume / kv.Value;
			//set your volume for each Variation in your Sound Group.
			kv.Key.audio.volume = adjustedVol;
		}
	}

	public void CreateVariation(DynamicSoundGroup group, AudioClip clip) {
		var resourceFileName = string.Empty;
		if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
			resourceFileName = DTGUIHelper.GetResourcePath(clip);
			if (string.IsNullOrEmpty(resourceFileName)) {
				resourceFileName = clip.name;
			}
		}
		
		var clipName = clip.name;
		
		if (group.transform.FindChild(clipName) != null) {
			DTGUIHelper.ShowAlert("You already have a Variation for this Group named '" + clipName + "'. \n\nPlease rename these Variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
		}
		
		var newVar = (GameObject) GameObject.Instantiate(_group.variationTemplate, _group.transform.position, Quaternion.identity);
		UndoHelper.CreateObjectForUndo(newVar, "create Variation");
		
		newVar.transform.name = clipName;
		newVar.transform.parent = group.transform;
		var variation = newVar.GetComponent<DynamicGroupVariation>();
		
		if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
			variation.audLocation = MasterAudio.AudioLocation.ResourceFile;
			variation.resourceFileName = resourceFileName;
		} else {
			newVar.audio.clip = clip;
		}
	}
}
