using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DynamicSoundGroupCreator))]
public class DynamicSoundGroupCreatorInspector : Editor {
	private const string EXISTING_BUS = "[EXISTING BUS]";
	private const string EXISTING_NAME_NAME  = "[EXISTING BUS NAME]";
	
	private DynamicSoundGroupCreator _creator;
	private List<DynamicSoundGroup> _groups;
	
	private List<DynamicSoundGroup> ScanForGroups() {
		var groups = new List<DynamicSoundGroup>();
		
		for (var i = 0; i < _creator.transform.childCount; i++) {
			var aChild = _creator.transform.GetChild(i);
			
			var grp = aChild.GetComponent<DynamicSoundGroup>();
			if (grp == null) {
				continue;
			}
			
			grp.groupVariations = VariationsForGroup(aChild.transform);
			
			groups.Add(grp);
		}
		
		return groups;
	}

	private List<DynamicGroupVariation> VariationsForGroup(Transform groupTrans) {
		var variations = new List<DynamicGroupVariation>();
		
		for (var i = 0; i < groupTrans.childCount; i++) {
			var aVar = groupTrans.GetChild(i);
			
			var variation = aVar.GetComponent<DynamicGroupVariation>();
			variations.Add(variation);
		}
		
		return variations;
	}
	
	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		
		EditorGUI.indentLevel = 1;
		var isDirty = false;
		
		_creator = (DynamicSoundGroupCreator)target;
		
		var isInProjectView = DTGUIHelper.IsPrefabInProjectView(_creator);
		
		if (_creator.logoTexture != null) {
			DTGUIHelper.ShowHeaderTexture(_creator.logoTexture);
		}
	
		MasterAudio.Instance = null;
		MasterAudio ma = MasterAudio.Instance;
		
		var busVoiceLimitList = new List<string>();
		busVoiceLimitList.Add(MasterAudio.NO_VOICE_LIMIT_NAME);

		for (var i = 1; i <= 32; i++) {
			busVoiceLimitList.Add(i.ToString());
		}
		
		var busList = new List<string>();
		busList.Add(MasterAudioGroup.NO_BUS);
		busList.Add(MasterAudioInspector.NEW_BUS_NAME);
		busList.Add(EXISTING_BUS);
		
		int maxChars = 12;
		
		GroupBus bus = null;
		for (var i = 0; i < _creator.groupBuses.Count; i++) {
			bus = _creator.groupBuses[i];
			busList.Add(bus.busName);
			
			if (bus.busName.Length > maxChars) {
				maxChars = bus.busName.Length;
			}
		}
		var busListWidth = 9 * maxChars;
		
        EditorGUI.indentLevel = 0;  // Space will handle this for the header

		var newAwake = EditorGUILayout.Toggle("Auto-create Items", _creator.createOnAwake);
		if (newAwake != _creator.createOnAwake) {
			UndoHelper.RecordObjectPropertyForUndo(_creator, "toggle Auto-create Items");
			_creator.createOnAwake = newAwake;
		}
		if (_creator.createOnAwake) {
			DTGUIHelper.ShowColorWarning("*Items will be created as soon as this object is in the Scene.");
		} else {
			DTGUIHelper.ShowColorWarning("*You will need to call this object's CreateItems method.");
		}

		var newRemove = EditorGUILayout.Toggle("Auto-remove Items", _creator.removeGroupsOnSceneChange);
		if (newRemove != _creator.removeGroupsOnSceneChange) {
			UndoHelper.RecordObjectPropertyForUndo(_creator, "toggle Auto-remove Items");
			_creator.removeGroupsOnSceneChange = newRemove;
		}

		if (_creator.removeGroupsOnSceneChange) {
			DTGUIHelper.ShowColorWarning("*Items will be deleted when the Scene changes.");
		} else {
			DTGUIHelper.ShowColorWarning("*Items will persist across Scenes if MasterAudio does.");
		}
		
		EditorGUILayout.Separator();
		
        _groups = ScanForGroups();
		var groupNameList = GroupNameList;
		
		EditorGUI.indentLevel = 0;
		GUI.color = _creator.showMusicDucking ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		
		var newShowDuck = EditorGUILayout.Toggle("Dynamic Music Ducking", _creator.showMusicDucking);		
		if (newShowDuck != _creator.showMusicDucking) {
			UndoHelper.RecordObjectPropertyForUndo(_creator, "toggle Dynamic Music Ducking");
			_creator.showMusicDucking = newShowDuck;
		}
		EditorGUILayout.EndHorizontal();
		GUI.color = Color.white;
		
		if (_creator.showMusicDucking) {
			GUI.contentColor = Color.green;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			
			if (GUILayout.Button(new GUIContent("Add Duck Group"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
				UndoHelper.RecordObjectPropertyForUndo(_creator, "Add Duck Group");
				
				var defaultBeginUnduck = 0.5f;
				if (ma != null) {
					defaultBeginUnduck = ma.defaultRiseVolStart;
				}

				_creator.musicDuckingSounds.Add(new DuckGroupInfo() {
					soundType = MasterAudio.NO_GROUP_NAME,
					riseVolStart = defaultBeginUnduck
				});	
			}

			EditorGUILayout.EndHorizontal();
			GUI.contentColor = Color.white;
			EditorGUILayout.Separator();
			
			if (_creator.musicDuckingSounds.Count == 0) {
				DTGUIHelper.ShowColorWarning("*You currently have no ducking sounds set up.");
			} else {
				int? duckSoundToRemove = null;
				
				for (var i = 0; i < _creator.musicDuckingSounds.Count; i++) {
					var duckSound = _creator.musicDuckingSounds[i];
					var index = groupNameList.IndexOf(duckSound.soundType);
					if (index == -1) {
						index = 0;
					}
					
					EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
					var newIndex = EditorGUILayout.Popup(index, groupNameList.ToArray(), GUILayout.MaxWidth(200));
					if (newIndex >= 0) {
						if (index != newIndex) {
							UndoHelper.RecordObjectPropertyForUndo(_creator, "change Duck Group");							
						}
						duckSound.soundType = groupNameList[newIndex];
					}
					
					GUI.contentColor = Color.green;
					GUILayout.TextField("Begin Unduck " + duckSound.riseVolStart.ToString("N2"), 20, EditorStyles.miniLabel);
					
					var newUnduck = GUILayout.HorizontalSlider(duckSound.riseVolStart, 0f, 1f, GUILayout.Width(60));
					if (newUnduck != duckSound.riseVolStart) {
						UndoHelper.RecordObjectPropertyForUndo(_creator, "change Begin Unduck");
						duckSound.riseVolStart = newUnduck;
					}
					GUI.contentColor = Color.white;
					
					GUILayout.FlexibleSpace();
					GUILayout.Space(10);
					if (DTGUIHelper.AddDynamicDeleteIcon(_creator, "Duck Sound")) {
						duckSoundToRemove = i;	
					}
					
					EditorGUILayout.EndHorizontal();
				}
				
				if (duckSoundToRemove.HasValue) {
					UndoHelper.RecordObjectPropertyForUndo(_creator, "delete Duck Group");
					_creator.musicDuckingSounds.RemoveAt(duckSoundToRemove.Value);
				}	
			}
		}		
		
		EditorGUILayout.Separator();
		
		GUI.color = _creator.soundGroupsAreExpanded ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
		
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		var newGroupEx = EditorGUILayout.Toggle("Dynamic Group Mixer", _creator.soundGroupsAreExpanded);
		if (newGroupEx != _creator.soundGroupsAreExpanded) {
			UndoHelper.RecordObjectPropertyForUndo(_creator, "toggle Dynamic Group Mixer");
			_creator.soundGroupsAreExpanded = newGroupEx;
		}
		
		EditorGUILayout.EndHorizontal();
		GUI.color = Color.white;

        if (_creator.soundGroupsAreExpanded)
        {
            var newDragMode = (MasterAudio.DragGroupMode)EditorGUILayout.EnumPopup("Bulk Creation Mode", _creator.curDragGroupMode);
            if (newDragMode != _creator.curDragGroupMode)
            {
                UndoHelper.RecordObjectPropertyForUndo(_creator, "change Bulk Creation Mode");
                _creator.curDragGroupMode = newDragMode;
            }

            var bulkMode = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Variation Create Mode", _creator.bulkVariationMode);
            if (bulkMode != _creator.bulkVariationMode)
            {
                UndoHelper.RecordObjectPropertyForUndo(_creator, "change Variation Mode");
                _creator.bulkVariationMode = bulkMode;
            }

            // create groups start
            EditorGUILayout.BeginVertical();
            var aEvent = Event.current;
			
			if (isInProjectView) {
				DTGUIHelper.ShowLargeBarAlert("*You are in Project View and cannot create or navigate Groups.");
				DTGUIHelper.ShowLargeBarAlert("*Pull this prefab into the Scene to create Groups.");
			} else {
				GUI.color = Color.yellow;
	
	            var dragAreaGroup = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
	            GUI.Box(dragAreaGroup, "Drag Audio clips here to create groups!");
	
	            GUI.color = Color.white;
	
	            switch (aEvent.type)
	            {
	                case EventType.DragUpdated:
	                case EventType.DragPerform:
	                    if (!dragAreaGroup.Contains(aEvent.mousePosition))
	                    {
	                        break;
	                    }
	
	                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
	
	                    if (aEvent.type == EventType.DragPerform)
	                    {
	                        DragAndDrop.AcceptDrag();
	
	                        Transform groupInfo = null;
	
	                        var clips = new List<AudioClip>();
	
	                        foreach (var dragged in DragAndDrop.objectReferences)
	                        {
	                            var aClip = dragged as AudioClip;
	                            if (aClip == null)
	                            {
	                                continue;
	                            }
	
	                            clips.Add(aClip);
	                        }
	
	                        clips.Sort(delegate(AudioClip x, AudioClip y)
	                        {
	                            return x.name.CompareTo(y.name);
	                        });
	
	                        for (var i = 0; i < clips.Count; i++)
	                        {
	                            var aClip = clips[i];
	                            if (_creator.curDragGroupMode == MasterAudio.DragGroupMode.OneGroupPerClip)
	                            {
	                                CreateGroup(aClip);
	                            }
	                            else
	                            {
	                                if (groupInfo == null)
	                                { // one group with variations
	                                    groupInfo = CreateGroup(aClip);
	                                }
	                                else
	                                {
	                                    CreateVariation(groupInfo, aClip);
	                                }
	                            }
	
	                            isDirty = true;
	                        }
	                    }
	                    Event.current.Use();
	                    break;
	            }
			}
            EditorGUILayout.EndVertical();
            // create groups end

            if (_creator.soundGroupsToCreate.Count > 0 && !Application.isPlaying)
            {
				if (isInProjectView) {
	                DTGUIHelper.ShowLargeBarAlert("You have data in an old format. Pull this prefab into the Scene, then Upgrade Data.");
				} else {
	                DTGUIHelper.ShowRedError("You have data in an old format. It will not be used as is.");
					DTGUIHelper.ShowRedError("Upgrade it to the new format by clicking the Upgrade button below.");
	
	                EditorGUILayout.BeginHorizontal();
	                GUILayout.Space(154);
	                GUI.contentColor = Color.green;
	                if (GUILayout.Button("Upgrade Data", EditorStyles.toolbarButton, GUILayout.Width(150)))
	                {
						UpgradeData();
	                }
	                GUI.contentColor = Color.white;
	                EditorGUILayout.EndHorizontal();
				}
            }

            if (_groups.Count == 0)
            {
                DTGUIHelper.ShowColorWarning("*You currently have no Dynamic Sound Groups created.");
            }

            int? indexToDelete = null;
			
			EditorGUILayout.LabelField("Group Control", EditorStyles.miniBoldLabel);
            GUI.color = Color.white;
			int? busToCreate = null;
			bool isExistingBus = false;
			
			for (var i = 0; i < _groups.Count; i++)
            {
                var aGroup = _groups[i];

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label(aGroup.name, GUILayout.Width(150));

                GUILayout.FlexibleSpace();
				
				// find bus.
				var selectedBusIndex = aGroup.busIndex == -1 ? 0 : aGroup.busIndex;
				
				GUI.contentColor = Color.white;
				GUI.color = Color.cyan;
				
				var busIndex = EditorGUILayout.Popup("", selectedBusIndex, busList.ToArray(), GUILayout.Width(busListWidth));
				if (busIndex == -1) {
					busIndex = 0; 
				}
				
				if (aGroup.busIndex != busIndex && busIndex != 1) {
					UndoHelper.RecordObjectPropertyForUndo(aGroup, "change Group Bus");
				}
				
				if (busIndex != 1) { // don't change the index, so undo will work.
					aGroup.busIndex = busIndex;
				}
				
				GUI.color = Color.white;
				
				if (selectedBusIndex != busIndex) {
					if (busIndex == 1 || busIndex == 2) {
						busToCreate = i;
						
						isExistingBus = busIndex == 2;
					} else if (busIndex >= DynamicSoundGroupCreator.HardCodedBusOptions) {
						//GroupBus newBus = _creator.groupBuses[busIndex - MasterAudio.HARD_CODED_BUS_OPTIONS];
						// do nothing unless we add muting and soloing here.
					}
				}
				
                GUI.contentColor = Color.green;
                GUILayout.TextField("V " + aGroup.groupMasterVolume.ToString("N2"), 6, EditorStyles.miniLabel);

                var newVol = GUILayout.HorizontalSlider(aGroup.groupMasterVolume, 0f, 1f, GUILayout.Width(100));
                if (newVol != aGroup.groupMasterVolume)
                {
                    UndoHelper.RecordObjectPropertyForUndo(aGroup, "change Group Volume");
                    aGroup.groupMasterVolume = newVol;
                }

                GUI.contentColor = Color.white;

                var buttonPressed = DTGUIHelper.AddDynamicGroupButtons(_creator);
                EditorGUILayout.EndHorizontal();

                switch (buttonPressed)
                {
                    case DTGUIHelper.DTFunctionButtons.Go:
                        Selection.activeGameObject = aGroup.gameObject;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Remove:
                        indexToDelete = i;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Play:
                        PreviewGroup(aGroup);
                        break;
                    case DTGUIHelper.DTFunctionButtons.Stop:
                        StopPreviewingGroup();
                        break;
                }
            }
			
			if (busToCreate.HasValue) {
				CreateBus(busToCreate.Value, isExistingBus);
			}
			
            if (indexToDelete.HasValue)
            {
                UndoHelper.DestroyForUndo(_groups[indexToDelete.Value].gameObject);
            }
			
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(6);
				
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Max Group Volumes", "Reset all group volumes to full"), EditorStyles.toolbarButton, GUILayout.Width(120))) { 
				UndoHelper.RecordObjectsForUndo(_groups.ToArray(), "Max Group Volumes");
				
				for (var l = 0; l < _groups.Count; l++) {
					var aGroup = _groups[l];
					aGroup.groupMasterVolume = 1f;
				}
			}			
			GUI.contentColor = Color.white;
			EditorGUILayout.EndHorizontal();
			
			//buses
			if (_creator.groupBuses.Count > 0) {
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Bus Control", EditorStyles.miniBoldLabel);
				
				GroupBus aBus = null;
				int? busToDelete = null;
				
				for (var i = 0; i < _creator.groupBuses.Count; i++) {
					aBus = _creator.groupBuses[i];
					
					EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
					
					var newBusName = EditorGUILayout.TextField("", aBus.busName, GUILayout.MaxWidth(200));
					if (newBusName != aBus.busName) {
						UndoHelper.RecordObjectPropertyForUndo(_creator, "change Bus Name");
						aBus.busName = newBusName;
					}
					
					GUILayout.FlexibleSpace();
					
					if (!aBus.isExisting) {
						GUILayout.Label("Voices");
						GUI.color = Color.cyan;
						
						var oldLimitIndex = busVoiceLimitList.IndexOf(aBus.voiceLimit.ToString());
						if (oldLimitIndex == -1) {
							oldLimitIndex = 0;
						}
						var busVoiceLimitIndex = EditorGUILayout.Popup("", oldLimitIndex, busVoiceLimitList.ToArray(), GUILayout.MaxWidth(70));
						if (busVoiceLimitIndex != oldLimitIndex) {
							UndoHelper.RecordObjectPropertyForUndo(_creator, "change Bus Voice Limit");
							aBus.voiceLimit = busVoiceLimitIndex <= 0 ? -1 : busVoiceLimitIndex;
						}
						
						GUI.color = Color.white;
	
						EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
						GUILayout.TextField("V " + aBus.volume.ToString("N2"), 6, EditorStyles.miniLabel);
						EditorGUILayout.EndHorizontal();
						
						var newBusVol = GUILayout.HorizontalSlider(aBus.volume, 0f, 1f, GUILayout.Width(86));
						if (newBusVol != aBus.volume) {
							UndoHelper.RecordObjectPropertyForUndo(_creator, "change Bus Volume");
							aBus.volume = newBusVol;
						}
						
						GUI.contentColor = Color.white;
					} else {
						DTGUIHelper.ShowColorWarning("Existing bus. No control.");
					}
					
					if (DTGUIHelper.AddDynamicDeleteIcon(_creator, "Bus")) {
						busToDelete = i;
					}
					
					EditorGUILayout.EndHorizontal();
				}
				
				if (busToDelete.HasValue) {
					DeleteBus(busToDelete.Value);
				}
			}			
        }
		
		EditorGUILayout.Separator();
		// Show Custom Events
		GUI.color = _creator.showCustomEvents ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
		
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		var newShowEvents = EditorGUILayout.Toggle("Dynamic Custom Events", _creator.showCustomEvents);
		if (_creator.showCustomEvents != newShowEvents) {
			UndoHelper.RecordObjectPropertyForUndo(_creator, "toggle Dynamic Custom Events");
			_creator.showCustomEvents = newShowEvents;
		}
		
		EditorGUILayout.EndHorizontal();
		GUI.color = Color.white;		
		
		if (_creator.showCustomEvents) {
			var newEvent = EditorGUILayout.TextField("New Event Name", _creator.newEventName);
			if (newEvent != _creator.newEventName) {
				UndoHelper.RecordObjectPropertyForUndo(_creator, "change New Event Name");
				_creator.newEventName = newEvent;
			}
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(154);
			GUI.contentColor = Color.green;
			if (GUILayout.Button("Create New Event", EditorStyles.toolbarButton, GUILayout.Width(100))) {			
				CreateCustomEvent(_creator.newEventName);
			}
			GUI.contentColor = Color.white;
			EditorGUILayout.EndHorizontal();
			
			if (_creator.customEventsToCreate.Count == 0) {
				DTGUIHelper.ShowColorWarning("*You currently have no custom events defined here.");
			}
			
			EditorGUILayout.Separator();
			
			int? indexToDelete = null;
			int? indexToRename = null;
			
			for (var i = 0; i < _creator.customEventsToCreate.Count; i++) {
				var anEvent = _creator.customEventsToCreate[i];
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				GUILayout.Label(anEvent.EventName, GUILayout.Width(170));
				
				GUILayout.FlexibleSpace();
				
				var newName = GUILayout.TextField(anEvent.ProspectiveName, GUILayout.Width(170));
				if (newName != anEvent.ProspectiveName) {
					UndoHelper.RecordObjectPropertyForUndo(_creator, "change Proposed Event Name");
					anEvent.ProspectiveName = newName;
				}
				var buttonPressed = DTGUIHelper.AddCustomEventDeleteIcon(true);
				
				switch (buttonPressed) {
					case DTGUIHelper.DTFunctionButtons.Remove:
						indexToDelete = i;
						break;
					case DTGUIHelper.DTFunctionButtons.Rename:
						indexToRename = i;
						break;
				}
				
				EditorGUILayout.EndHorizontal();
			}
			
			if (indexToDelete.HasValue) {
				_creator.customEventsToCreate.RemoveAt(indexToDelete.Value);
			}
			if (indexToRename.HasValue) {
				RenameEvent(_creator.customEventsToCreate[indexToRename.Value]);
			}
		}
		
		// End Show Custom Events
		
		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}

		this.Repaint();

		//DrawDefaultInspector();
    }
	
	private Transform CreateGroup(AudioClip aClip) {
		if (_creator.groupTemplate == null) {
			DTGUIHelper.ShowAlert("Your 'Group Template' field is empty, please assign it in debug mode. Drag the 'DynamicSoundGroup' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode.");
			return null;
		}

		var groupName = UtilStrings.TrimSpace(aClip.name);
		
		var matchingGroup = _groups.Find(delegate(DynamicSoundGroup obj) {
			return obj.transform.name == groupName;
		});
		
		if (matchingGroup != null) {
			DTGUIHelper.ShowAlert("You already have a Group named '" + groupName + "'. \n\nPlease rename this Group when finished to be unique.");
		}
		
		var spawnedGroup = (GameObject) GameObject.Instantiate(_creator.groupTemplate, _creator.transform.position, Quaternion.identity);
		spawnedGroup.name = groupName;
				
		UndoHelper.CreateObjectForUndo(spawnedGroup, "create Dynamic Group");
		spawnedGroup.transform.parent = _creator.transform;
		
		CreateVariation(spawnedGroup.transform, aClip);
		
		return spawnedGroup.transform;
	}
	
	private void CreateVariation(Transform aGroup, AudioClip aClip) {
		if (_creator.variationTemplate == null) {
			DTGUIHelper.ShowAlert("Your 'Variation Template' field is empty, please assign it in debug mode. Drag the 'DynamicGroupVariation' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode.");
			return;
		}
			
		var resourceFileName = string.Empty;
		if (_creator.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
			resourceFileName = DTGUIHelper.GetResourcePath(aClip);
			if (string.IsNullOrEmpty(resourceFileName)) {
				resourceFileName = aClip.name;
			}
		}

		var clipName = UtilStrings.TrimSpace(aClip.name);
		
		var myGroup = aGroup.GetComponent<DynamicSoundGroup>();
		
		var matches = myGroup.groupVariations.FindAll(delegate(DynamicGroupVariation obj) {
			return obj.name == clipName;	
		});
		
		if (matches.Count > 0) {
			DTGUIHelper.ShowAlert("You already have a variation for this Group named '" + clipName + "'. \n\nPlease rename these variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
		}
		
		var spawnedVar = (GameObject) GameObject.Instantiate(_creator.variationTemplate, _creator.transform.position, Quaternion.identity);
		spawnedVar.name = clipName;
				
		spawnedVar.transform.parent = aGroup;
		
		var dynamicVar = spawnedVar.GetComponent<DynamicGroupVariation>();
		
		if (_creator.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
			dynamicVar.audLocation = MasterAudio.AudioLocation.ResourceFile;
			dynamicVar.resourceFileName = resourceFileName;
		} else {
			dynamicVar.audio.clip = aClip;
		}
	}
	
	private void CreateCustomEvent(string newEventName) {
		var match = _creator.customEventsToCreate.FindAll(delegate(CustomEvent custEvent) {
			return custEvent.EventName == newEventName;
		});
		
		if (match.Count > 0) {
			DTGUIHelper.ShowAlert("You already have a custom event named '" + newEventName + "' configured here. Please choose a different name.");
			return;
		}
		
		var newEvent = new CustomEvent(newEventName);
		
		_creator.customEventsToCreate.Add(newEvent);
	}
	
	private void RenameEvent(CustomEvent cEvent) {
		var match = _creator.customEventsToCreate.FindAll(delegate(CustomEvent obj) {
			return obj.EventName == cEvent.ProspectiveName;	
		});
		
		if (match.Count > 0) {
			DTGUIHelper.ShowAlert("You already have a custom event named '" + cEvent.ProspectiveName + "' configured here. Please choose a different name.");
			return;
		}
		
		cEvent.EventName = cEvent.ProspectiveName;
	}
	
	private void PreviewGroup(DynamicSoundGroup aGroup) {
		var rndIndex = UnityEngine.Random.Range(0, aGroup.groupVariations.Count);
		var rndVar = aGroup.groupVariations[rndIndex];

		if (rndVar.audLocation == MasterAudio.AudioLocation.ResourceFile) {
			_creator.PreviewerInstance.Stop();
			_creator.PreviewerInstance.PlayOneShot(Resources.Load(rndVar.resourceFileName) as AudioClip, rndVar.audio.volume);
		} else {
			_creator.PreviewerInstance.PlayOneShot(rndVar.audio.clip, rndVar.audio.volume);
		}
	}
	
	private void StopPreviewingGroup() {
		_creator.PreviewerInstance.Stop();
	}
	
	private List<string> GroupNameList {
		get {
			var groupNames = new List<string>();
			groupNames.Add(MasterAudio.NO_GROUP_NAME);
			
			for (var i = 0; i < _groups.Count; i++) {
				groupNames.Add(_groups[i].name);
			}
			
			return groupNames;
		}
	}
	
	private void DeleteBus(int busIndex) {
		DynamicSoundGroup aGroup = null;
		
		var groupsWithBus = new List<DynamicSoundGroup>();
		var groupsWithHigherBus = new List<DynamicSoundGroup>();
		
		for (var i = 0; i < this._groups.Count; i++) {
			aGroup = this._groups[i];
			if (aGroup.busIndex == -1) {
				continue;
			}
			if (aGroup.busIndex == busIndex + DynamicSoundGroupCreator.HardCodedBusOptions) {					
				groupsWithBus.Add(aGroup);
			} else if (aGroup.busIndex > busIndex + DynamicSoundGroupCreator.HardCodedBusOptions) {
				groupsWithHigherBus.Add(aGroup);
			}
		}
		
		var allObjects = new List<UnityEngine.Object>();
		allObjects.Add(_creator);
		foreach (var g in groupsWithBus) {
			allObjects.Add(g as UnityEngine.Object);
		}
		
		foreach (var g in groupsWithHigherBus) {
			allObjects.Add(g as UnityEngine.Object);
		}
		
		UndoHelper.RecordObjectsForUndo(allObjects.ToArray(), "delete Bus");
		
		// change all
		_creator.groupBuses.RemoveAt(busIndex);
		
		foreach (var group in groupsWithBus) {
			group.busIndex = -1;
		}
		
		foreach (var group in groupsWithHigherBus) {
			group.busIndex--;
		}
	}
	
	private void CreateBus(int groupIndex, bool isExisting) {
		var sourceGroup = _groups[groupIndex];
		
		var affectedObjects = new UnityEngine.Object[] {
			_creator,
			sourceGroup
		};
		
		UndoHelper.RecordObjectsForUndo(affectedObjects, "create Bus");
		
		var newBusName = isExisting ? EXISTING_NAME_NAME : MasterAudioInspector.RENAME_ME_BUS_NAME;
		
		var newBus = new GroupBus() {
			busName = newBusName
		};
		
		newBus.isExisting = isExisting;
		
		_creator.groupBuses.Add(newBus);
		
		sourceGroup.busIndex = DynamicSoundGroupCreator.HardCodedBusOptions + _creator.groupBuses.Count - 1;
	}
	
	private void UpgradeData() {
		if (_creator.groupTemplate == null) {
			DTGUIHelper.ShowAlert("Your 'Group Template' field is empty, please assign it in debug mode. Drag the 'DynamicSoundGroup' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode. Upgrade data cancelled.");
			return;
		}

		if (_creator.variationTemplate == null) {
			DTGUIHelper.ShowAlert("Your 'Variation Template' field is empty, please assign it in debug mode. Drag the 'DynamicGroupVariation' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode. Upgrade data cancelled");
			return;
		}

		
		for (var g = 0; g < _creator.soundGroupsToCreate.Count; g++) {		
			var grp = _creator.soundGroupsToCreate[g];
			var groupName = grp.groupName;
			
			var match = _groups.FindAll(delegate(DynamicSoundGroup obj) {
				return obj.name == groupName;
			});
			if (match.Count > 0) {
				Debug.LogWarning("Dynamic Sound Group '" + groupName + "' already exists. Please rename one of them.");
			}
			
			var newGrp = (GameObject) GameObject.Instantiate(_creator.groupTemplate, _creator.transform.position, Quaternion.identity);
			newGrp.transform.name = groupName;
			newGrp.transform.parent = _creator.transform;
			
			var groupScript = newGrp.GetComponent<DynamicSoundGroup>();
			
			if (grp.duckSound) {
				var duckMatch = _creator.musicDuckingSounds.FindAll(delegate(DuckGroupInfo obj) {
					return obj.soundType == groupName;	
				});
				
				if (duckMatch.Count > 0) {
					Debug.LogWarning("Dynamic Duck Setting for Sound Group '" + groupName + "' already exists. Skipping adding Duck Group.");
				} else {
					var newDuck = new DuckGroupInfo() {
						soundType = groupName,
						riseVolStart = grp.riseVolStart
					};
					_creator.musicDuckingSounds.Add(newDuck);
				}
			}
			
			switch (grp.busMode) {
				case DynamicSoundGroupInfo.BusMode.NoBus:
					break;
				case DynamicSoundGroupInfo.BusMode.UseExisting:
					var matchBusIndex = _creator.groupBuses.FindIndex(delegate(GroupBus obj) {
						return obj.busName == grp.busName;
					});
					
					if (matchBusIndex < 0) { // bus doesn't exist in Dynamic SGC
						var newBus = new GroupBus() {
							busName = grp.busName,
							isExisting = true
						};
						_creator.groupBuses.Add(newBus);
						
						matchBusIndex = _creator.groupBuses.FindIndex(delegate(GroupBus obj) {
							return obj.busName == grp.busName;
						});	
					} 
				
					groupScript.busIndex = matchBusIndex + DynamicSoundGroupCreator.HardCodedBusOptions;
					groupScript.busName = grp.busName;
					
					break;
				case DynamicSoundGroupInfo.BusMode.CreateNew:
					break;
			}
			
			groupScript.groupMasterVolume = grp.groupMasterVolume;
			groupScript.retriggerPercentage = grp.retriggerPercentage;
			groupScript.curVariationSequence = grp.curVariationSequence;
			groupScript.useInactivePeriodPoolRefill = grp.useInactivePeriodPoolRefill;
			groupScript.inactivePeriodSeconds = grp.inactivePeriodSeconds;
			groupScript.curVariationMode = grp.curVariationMode;
			groupScript.limitMode = grp.limitMode;
			groupScript.limitPerXFrames = grp.limitPerXFrames;
			groupScript.minimumTimeBetween = grp.minimumTimeBetween;
			groupScript.limitPolyphony = grp.limitPolyphony;
			groupScript.voiceLimitCount = grp.voiceLimitCount;
			
			// make variations!
			for (var v = 0; v < grp.variations.Count; v++) {
				var aVar = grp.variations[v];
				
				var newVar = (GameObject) GameObject.Instantiate(_creator.variationTemplate, _creator.transform.position, Quaternion.identity);
				var varName = string.Empty;
				
				switch (aVar.audLocation) {
					case MasterAudio.AudioLocation.Clip:
						varName = string.IsNullOrEmpty(aVar.clipName) ? aVar.clip.name : aVar.clipName;
						break;
					case MasterAudio.AudioLocation.ResourceFile:
						varName = aVar.resourceFileName;
						break;
				}
				
				newVar.transform.name = varName;
				newVar.transform.parent = newGrp.transform;
				
				var varScript = newVar.GetComponent<DynamicGroupVariation>();
				
				varScript.audLocation = aVar.audLocation;
				
				switch (aVar.audLocation) {
					case MasterAudio.AudioLocation.Clip:
						varScript.audio.clip = aVar.clip;
						break;
					case MasterAudio.AudioLocation.ResourceFile:
						varScript.resourceFileName = aVar.resourceFileName;
						break;
				}
				
				varScript.weight = aVar.weight;
				varScript.audio.volume = aVar.volume;
				varScript.audio.pitch = aVar.pitch;
				varScript.audio.loop = aVar.loopClip;
				varScript.randomPitch = aVar.randomPitch;
				varScript.randomVolume = aVar.randomVolume;
				varScript.useFades = aVar.useFades;
				varScript.fadeInTime = aVar.fadeInTime;
				varScript.fadeOutTime = aVar.fadeOutTime;
				
				varScript.audio.rolloffMode = aVar.audRollOffMode;
				varScript.audio.minDistance = aVar.audMinDistance;
				varScript.audio.maxDistance = aVar.audMaxDistance;
				varScript.audio.dopplerLevel = aVar.audDopplerLevel;
				varScript.audio.spread = aVar.audSpread;
				varScript.audio.panLevel = aVar.audPanLevel;
			}
		}

		_creator.soundGroupsToCreate.Clear();
		DTGUIHelper.ShowAlert("Dynamic Sound Group Creator data upgraded successfully!");
	}
}
