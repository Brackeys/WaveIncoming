using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EventSounds))]
[CanEditMultipleObjects]
public class AudioEventInspector : Editor
{
	private List<string> groupNames = null;
	private List<string> busNames = null;
	private List<string> playlistNames = null;
	private List<string> playlistControllerNames = null;
	private List<string> customEventNames = null;
	private bool maInScene;
	private MasterAudio ma;
	private EventSounds sounds;

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls(); 
		
		MasterAudio.Instance = null;
		
		ma = MasterAudio.Instance;
		if (ma != null) {
			DTGUIHelper.ShowHeaderTexture(ma.logoTexture);
		}
		
		sounds = (EventSounds)target;
		
		maInScene = ma != null;		
		if (maInScene) {
			groupNames = ma.GroupNames;
			busNames = ma.BusNames;
			playlistNames = ma.PlaylistNames;
			customEventNames = ma.CustomEventNames;
		}
		
		playlistControllerNames = new List<string>();
		playlistControllerNames.Add(MasterAudio.DYNAMIC_GROUP_NAME);
		playlistControllerNames.Add(MasterAudio.NO_GROUP_NAME);
		
		var pcs = GameObject.FindObjectsOfType(typeof(PlaylistController));
		for (var i = 0; i < pcs.Length; i++) {
			playlistControllerNames.Add(pcs[i].name);
		}

		// populate unused Events for dropdown
		var unusedEventTypes = new List<string>();
		if (!sounds.useStartSound) {
			unusedEventTypes.Add("Start");
		}
		if (!sounds.useEnableSound) {
			unusedEventTypes.Add("Enable");
		}
		if (!sounds.useDisableSound) {
			unusedEventTypes.Add("Disable");
		}
		if (!sounds.useVisibleSound) {
			unusedEventTypes.Add("Visible");
		}
		if (!sounds.useInvisibleSound) {
			unusedEventTypes.Add("Invisible");
		}
		
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// these events don't exist
		#else 		
			if (!sounds.useCollision2dSound) {
				unusedEventTypes.Add("2D Collision");
			}
			if (!sounds.useTriggerEnter2dSound) {
				unusedEventTypes.Add("2D Trigger Enter");
			}
			if (!sounds.useTriggerExit2dSound) {
				unusedEventTypes.Add("2D Trigger Exit");
			}
		#endif		
		
		if (!sounds.useCollisionSound) {
			unusedEventTypes.Add("Collision");
		}
		if (!sounds.useTriggerEnterSound) {
			unusedEventTypes.Add("Trigger Enter");
		}
		if (!sounds.useTriggerExitSound) {
			unusedEventTypes.Add("Trigger Exit");
		}
		if (!sounds.useParticleCollisionSound) {
			unusedEventTypes.Add("Particle Collision");
		}
		if (!sounds.useMouseEnterSound) {
			unusedEventTypes.Add("Mouse Enter");
		}
		if (!sounds.useMouseClickSound) {
			unusedEventTypes.Add("Mouse Click");
		}
		if (!sounds.useSpawnedSound && sounds.showPoolManager) {
			unusedEventTypes.Add("Spawned");
		}
		if (!sounds.useDespawnedSound && sounds.showPoolManager) {
			unusedEventTypes.Add("Despawned");
		}
		
		unusedEventTypes.Add("Custom Event");
		
		var newDisable = EditorGUILayout.Toggle("Disable Sounds", sounds.disableSounds);
		if (newDisable != sounds.disableSounds) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Disable Sounds");
			sounds.disableSounds = newDisable;
		}

		if (!sounds.disableSounds) {
			var newSpawnMode = (MasterAudio.SoundSpawnLocationMode) EditorGUILayout.EnumPopup("Sound Spawn Mode", sounds.soundSpawnMode);
			if (newSpawnMode != sounds.soundSpawnMode) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Spawn Mode");
				sounds.soundSpawnMode = newSpawnMode;
			}
			
			var newGiz = EditorGUILayout.Toggle("Show 3D Gizmo", sounds.showGizmo);
			if (newGiz != sounds.showGizmo) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Show 3D Gizmo");
				sounds.showGizmo = newGiz;
			}

			var newPM = EditorGUILayout.Toggle("Pooling Events", sounds.showPoolManager);
			if (newPM != sounds.showPoolManager) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Pooling Events");
				sounds.showPoolManager = newPM;
			}

			var newUnused = EditorGUILayout.Toggle("Minimal Mode", sounds.hideUnused);
			if (newUnused != sounds.hideUnused) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Hide Unused Events");
				sounds.hideUnused = newUnused;
			}
			
   			var newLogMissing = EditorGUILayout.Toggle("Log Missing Events", sounds.logMissingEvents);
	        if (newLogMissing != sounds.logMissingEvents) {
 			    UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Log Missing Events");
		        sounds.logMissingEvents = newLogMissing;
			}
			
			if (sounds.hideUnused) {
				var newEventIndex = EditorGUILayout.Popup("Event To Activate", -1, unusedEventTypes.ToArray());
				if (newEventIndex > -1) {
					var selectedEvent = unusedEventTypes[newEventIndex];
					UndoHelper.RecordObjectPropertyForUndo(sounds, "Active Event");
					
					switch (selectedEvent) {
						case "Start":
							sounds.useStartSound = true;
							break;
						case "Enable":
							sounds.useEnableSound = true;
							break;
						case "Disable":
							sounds.useDisableSound = true;
							break;
						case "Visible":
							sounds.useVisibleSound = true;
							break;
						case "Invisible":
							sounds.useInvisibleSound = true;
							break;
						case "2D Collision":
							sounds.useCollision2dSound = true;
							break;
						case "2D Trigger Enter":
							sounds.useTriggerEnter2dSound = true;
							break;
						case "2D Trigger Exit":
							sounds.useTriggerExit2dSound = true;
							break;
						case "Collision":
							sounds.useCollisionSound = true;
							break;
						case "Trigger Enter":
							sounds.useTriggerEnterSound = true;
							break;
						case "Trigger Exit":
							sounds.useTriggerExitSound = true;
							break;
						case "Particle Collision":
							sounds.useParticleCollisionSound = true;
							break;
						case "Mouse Enter":
							sounds.useMouseEnterSound = true;
							break;
						case "Mouse Click":
							sounds.useMouseClickSound = true;
							break;
						case "Spawned":
							sounds.useSpawnedSound = true;
							break;
						case "Despawned":
							sounds.useDespawnedSound = true;
							break;
						case "Custom Event":
							CreateCustomEvent(false);
							break;
						default:
							Debug.LogError("Add code for event type: " + selectedEvent);
							break;
					} 
				}
			} else {
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(154);
				GUI.contentColor = Color.green;
				if (GUILayout.Button("Add Custom Event", EditorStyles.toolbarButton, GUILayout.Width(110))) {
					CreateCustomEvent(true);
				}
				GUI.contentColor = Color.white;
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Separator();
		var suffix = string.Empty;
		if (sounds.disableSounds) {
			suffix = " (DISABLED)";
		} else if (unusedEventTypes.Count > 0 && sounds.hideUnused) {
			suffix = " (" + unusedEventTypes.Count + " hidden)";
		}
		GUILayout.Label("Sound Triggers" + suffix, EditorStyles.boldLabel);

		var disabledText = "";
		if (sounds.disableSounds) {
			disabledText = " (DISABLED) ";
		}

		List<bool> changedList = new List<bool>();

		// trigger sounds
		if (!sounds.hideUnused || sounds.useStartSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useStartSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

			var newUseStart = EditorGUILayout.Toggle("Start" + disabledText, sounds.useStartSound);
			if (newUseStart != sounds.useStartSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Start Sound");
				sounds.useStartSound = newUseStart;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useStartSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.startSound, EventSounds.EventType.OnStart));
			}
		}
		
		if (!sounds.hideUnused || sounds.useEnableSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useEnableSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newUseEnable = EditorGUILayout.Toggle("Enable" + disabledText, sounds.useEnableSound);
			if (newUseEnable != sounds.useEnableSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Enable Sound");
				sounds.useEnableSound = newUseEnable;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useEnableSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.enableSound, EventSounds.EventType.OnEnable));
			}
		}

		if (!sounds.hideUnused || sounds.useDisableSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useDisableSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newDisableSound = EditorGUILayout.Toggle("Disable" + disabledText, sounds.useDisableSound);
			if (newDisableSound != sounds.useDisableSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Disable Sound");
				sounds.useDisableSound = newDisableSound;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useDisableSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.disableSound, EventSounds.EventType.OnDisable));
			}
		}

		if (!sounds.hideUnused || sounds.useVisibleSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useVisibleSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newVisible = EditorGUILayout.Toggle("Visible" + disabledText, sounds.useVisibleSound);
			if (newVisible != sounds.useVisibleSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Visible Sound");
				sounds.useVisibleSound = newVisible;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useVisibleSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.visibleSound, EventSounds.EventType.OnVisible));
			}
		}

		if (!sounds.hideUnused || sounds.useInvisibleSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useInvisibleSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newInvis = EditorGUILayout.Toggle("Invisible" + disabledText, sounds.useInvisibleSound);
			if (newInvis != sounds.useInvisibleSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Invisible Sound");
				sounds.useInvisibleSound = newInvis;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useInvisibleSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.invisibleSound, EventSounds.EventType.OnInvisible));
			}
		}

		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// these events don't exist
		#else 
			if (!sounds.hideUnused || sounds.useCollision2dSound) {
				EditorGUI.indentLevel = 0;
				GUI.color = sounds.useCollision2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newCollision2d = EditorGUILayout.Toggle("2D Collision" + disabledText, sounds.useCollision2dSound);
				if (newCollision2d != sounds.useCollision2dSound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle 2D Collision Sound");
					sounds.useCollision2dSound = newCollision2d;
				}
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				if (sounds.useCollision2dSound && !sounds.disableSounds) {
					changedList.Add(RenderAudioEvent(sounds.collision2dSound, EventSounds.EventType.OnCollision2D));
				}
			}

			if (!sounds.hideUnused || sounds.useTriggerEnter2dSound) {
				EditorGUI.indentLevel = 0;
				GUI.color = sounds.useTriggerEnter2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newTrigger2d = EditorGUILayout.Toggle("2D Trigger Enter" + disabledText, sounds.useTriggerEnter2dSound);
				if (newTrigger2d != sounds.useTriggerEnter2dSound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle 2D Trigger Enter Sound");
					sounds.useTriggerEnter2dSound = newTrigger2d;
				}
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				if (sounds.useTriggerEnter2dSound && !sounds.disableSounds) {
					changedList.Add(RenderAudioEvent(sounds.triggerEnter2dSound, EventSounds.EventType.OnTriggerEnter2D));
				}
			}

			if (!sounds.hideUnused || sounds.useTriggerExit2dSound) {
				EditorGUI.indentLevel = 0;
				GUI.color = sounds.useTriggerExit2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newTriggerExit2d = EditorGUILayout.Toggle("2D Trigger Exit" + disabledText, sounds.useTriggerExit2dSound);
				if (newTriggerExit2d != sounds.useTriggerExit2dSound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle 2D Trigger Exit Sound");
					sounds.useTriggerExit2dSound = newTriggerExit2d;
				}
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				if (sounds.useTriggerExit2dSound && !sounds.disableSounds) {
					changedList.Add(RenderAudioEvent(sounds.triggerExit2dSound, EventSounds.EventType.OnTriggerExit2D));
				}
			}
		#endif

		if (!sounds.hideUnused || sounds.useCollisionSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useCollisionSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newCollision = EditorGUILayout.Toggle("Collision" + disabledText, sounds.useCollisionSound);
			if (newCollision != sounds.useCollisionSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Collision Sound");
				sounds.useCollisionSound = newCollision;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useCollisionSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.collisionSound, EventSounds.EventType.OnCollision));
			}
		}

		if (!sounds.hideUnused || sounds.useTriggerEnterSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useTriggerEnterSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newTrigger = EditorGUILayout.Toggle("Trigger Enter" + disabledText, sounds.useTriggerEnterSound);
			if (newTrigger != sounds.useTriggerEnterSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Trigger Enter Sound");
				sounds.useTriggerEnterSound = newTrigger;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useTriggerEnterSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.triggerSound, EventSounds.EventType.OnTriggerEnter));
			}
		}

		if (!sounds.hideUnused || sounds.useTriggerExitSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useTriggerExitSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newTriggerExit = EditorGUILayout.Toggle("Trigger Exit" + disabledText, sounds.useTriggerExitSound);
			if (newTriggerExit != sounds.useTriggerExitSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Trigger Exit Sound");
				sounds.useTriggerExitSound = newTriggerExit;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useTriggerExitSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.triggerExitSound, EventSounds.EventType.OnTriggerExit));
			}
		}
		
		if (!sounds.hideUnused || sounds.useParticleCollisionSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useParticleCollisionSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newCollision = EditorGUILayout.Toggle("Particle Collision" + disabledText, sounds.useParticleCollisionSound);
			if (newCollision != sounds.useParticleCollisionSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Particle Collision Sound");
				sounds.useParticleCollisionSound = newCollision;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useParticleCollisionSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.particleCollisionSound, EventSounds.EventType.OnParticleCollision));
			}
		}
		
		if (!sounds.hideUnused || sounds.useMouseEnterSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useMouseEnterSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newMouseEnter = EditorGUILayout.Toggle("Mouse Enter" + disabledText, sounds.useMouseEnterSound);
			if (newMouseEnter != sounds.useMouseEnterSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Mouse Enter Sound");
				sounds.useMouseEnterSound = newMouseEnter;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useMouseEnterSound && !sounds.disableSounds) {
				changedList.Add(RenderAudioEvent(sounds.mouseEnterSound, EventSounds.EventType.OnMouseEnter));
			}
		}

		if (!sounds.hideUnused || sounds.useMouseClickSound) {
			EditorGUI.indentLevel = 0;
			GUI.color = sounds.useMouseClickSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			var newMouseClick = EditorGUILayout.Toggle("Mouse Click" + disabledText, sounds.useMouseClickSound);
			if (newMouseClick != sounds.useMouseClickSound) {
				sounds.useMouseClickSound = newMouseClick;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (sounds.useMouseClickSound && !sounds.disableSounds) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Mouse Click Sound");
				changedList.Add(RenderAudioEvent(sounds.mouseClickSound, EventSounds.EventType.OnMouseClick));
			}
		}

		if (sounds.showPoolManager) {
			if (!sounds.hideUnused || sounds.useSpawnedSound) {
				EditorGUI.indentLevel = 0;
				GUI.color = sounds.useSpawnedSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newSpawned = EditorGUILayout.Toggle("Spawned (Pooling)" + disabledText, sounds.useSpawnedSound);
				if (newSpawned != sounds.useSpawnedSound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Spawned Sound");
					sounds.useSpawnedSound = newSpawned;
				}
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				if (sounds.useSpawnedSound && !sounds.disableSounds) {
					changedList.Add(RenderAudioEvent(sounds.spawnedSound, EventSounds.EventType.OnSpawned));
				}
			}

			if (!sounds.hideUnused || sounds.useDespawnedSound) {
				EditorGUI.indentLevel = 0;
				GUI.color = sounds.useDespawnedSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newDespawned = EditorGUILayout.Toggle("Despawned (Pooling)" + disabledText, sounds.useDespawnedSound);
				if (newDespawned != sounds.useDespawnedSound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Despawned Sound");
					sounds.useDespawnedSound = newDespawned;
				}
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				if (sounds.useDespawnedSound && !sounds.disableSounds) {
					changedList.Add(RenderAudioEvent(sounds.despawnedSound, EventSounds.EventType.OnDespawned));
				}
			}
		}
		
		if (sounds.userDefinedSounds.Count > 0) {
			EditorGUI.indentLevel = 0;
			
			int? eventToDelete = null;
			
			for (var i = 0; i < sounds.userDefinedSounds.Count; i++) {
				var customEvent = sounds.userDefinedSounds[i];
				
				GUI.color = customEvent.customSoundActive ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
				EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
				var newUse = EditorGUILayout.Toggle("Custom Event " + disabledText, customEvent.customSoundActive);
				if (newUse != customEvent.customSoundActive) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Custom Event active");
					customEvent.customSoundActive = newUse;
				}
				
				var buttonPressed = DTGUIHelper.AddCustomEventDeleteIcon(false);
				switch (buttonPressed) {
					case DTGUIHelper.DTFunctionButtons.Remove:
						eventToDelete = i;
						break;
				}
					
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
				
				if (customEvent.customSoundActive && !sounds.disableSounds) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Custom Event Sound");
					changedList.Add(RenderAudioEvent(customEvent, EventSounds.EventType.UserDefinedEvent));
				}
			}
			
			if (eventToDelete.HasValue) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "delete Custom Event Sound");
				sounds.userDefinedSounds.RemoveAt(eventToDelete.Value);
			}
		}
		
		if (GUI.changed || changedList.Contains(true)) {
			EditorUtility.SetDirty(target);
		}

		this.Repaint();

		//DrawDefaultInspector();
	}

	private bool RenderAudioEvent(AudioEvent aEvent, EventSounds.EventType eType)
	{
		bool showLayerTagFilter = EventSounds.layerTagFilterEvents.Contains(eType.ToString());
		
		bool isDirty = false;

		EditorGUI.indentLevel = 0;
	
		if (eType == EventSounds.EventType.OnEnable) {
			DTGUIHelper.ShowColorWarning("*If this prefab is in the scene at startup, use Start event instead.");
		}
		
		if (eType == EventSounds.EventType.UserDefinedEvent) {
			if (maInScene) {
				var existingIndex = customEventNames.IndexOf(aEvent.customEventName);
	
				int? customEventIndex = null;

				EditorGUI.indentLevel = 0;
			
				var noEvent = false;
				var noMatch = false;
			
				if (existingIndex >= 1) {
					customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
					if (existingIndex == 1) {
						noEvent = true;
					}
				} else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
					customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
				} else { // non-match
					noMatch = true;
					var newEventName = EditorGUILayout.TextField("Custom Event Name", aEvent.customEventName);
					if (newEventName != aEvent.customEventName) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Custom Event Name");
						aEvent.customEventName = newEventName;
					}

					var newIndex = EditorGUILayout.Popup("All Custom Events", -1, customEventNames.ToArray());
					if (newIndex >= 0) {
						customEventIndex = newIndex;
					}
				}
				
				if (noEvent) {	
					DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");					
				} else if (noMatch) {
					DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
				}
				
				if (customEventIndex.HasValue) {
					if (existingIndex != customEventIndex.Value) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Custom Event");
					}
					if (customEventIndex.Value == -1) {
						aEvent.customEventName = MasterAudio.NO_GROUP_NAME;
					} else {
						aEvent.customEventName = customEventNames[customEventIndex.Value];
					}
				}
			} else {
				var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", aEvent.customEventName);
				if (newCustomEvent != aEvent.customEventName) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "Custom Event Name");
					aEvent.customEventName = newCustomEvent;
				}
			}
			
		}
		
		var newSoundType = (MasterAudio.EventSoundFunctionType) EditorGUILayout.EnumPopup("Action Type", aEvent.currentSoundFunctionType);
		if (newSoundType != aEvent.currentSoundFunctionType) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "change Action Type"); 
			aEvent.currentSoundFunctionType = newSoundType;
		}
		
		switch (aEvent.currentSoundFunctionType) {
			case MasterAudio.EventSoundFunctionType.PlaySound:
				if (maInScene) {
					var existingIndex = groupNames.IndexOf(aEvent.soundType);
		
					int? groupIndex = null;

					EditorGUI.indentLevel = 1;
				
					var noGroup = false;
					var noMatch = false;
				
					if (existingIndex >= 1) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
						if (existingIndex == 1) {
							noGroup = true;
						}
					} else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
					} else { // non-match
						noMatch = true;
						var newSound = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
						if (newSound != aEvent.soundType) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
							aEvent.soundType = newSound;
						}

						var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
						if (newIndex >= 0) {
							groupIndex = newIndex;
						}
					}
					
					if (noGroup) {	
						DTGUIHelper.ShowRedError("No Sound Group specified. Event will do nothing.");					
					} else if (noMatch) {
						DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
					}
					
					if (groupIndex.HasValue) {
						if (existingIndex != groupIndex.Value) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
						}
						if (groupIndex.Value == -1) {
							aEvent.soundType = MasterAudio.NO_GROUP_NAME;
						} else {
							aEvent.soundType = groupNames[groupIndex.Value];
						}
					}
				} else {
					var newSType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
					if (newSType != aEvent.soundType) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
						aEvent.soundType = newSType;
					}
				}
			
				var newVarType = (EventSounds.VariationType) EditorGUILayout.EnumPopup("Variation Mode", aEvent.variationType);
				if (newVarType != aEvent.variationType) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Variation Mode");
					aEvent.variationType = newVarType;
				}
			
				if (aEvent.variationType == EventSounds.VariationType.PlaySpecific) {
					var newVarName = EditorGUILayout.TextField("Variation Name", aEvent.variationName);
					if (newVarName != aEvent.variationName) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Variation Name");
						aEvent.variationName = newVarName;
					}
				
					if (string.IsNullOrEmpty(aEvent.variationName)) {
						DTGUIHelper.ShowRedError("Variation Name is empty. No sound will play.");
					}
				}
			
				var newVol = EditorGUILayout.Slider("Volume", aEvent.volume, 0f, 1f);
				if (newVol != aEvent.volume) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Volume");
					aEvent.volume = newVol;
				}

				var newFixedPitch = EditorGUILayout.Toggle("Override pitch?", aEvent.useFixedPitch);
				if (newFixedPitch != aEvent.useFixedPitch) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Override pitch");
					aEvent.useFixedPitch = newFixedPitch;
				}
				if (aEvent.useFixedPitch) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Pitch");
					aEvent.pitch = EditorGUILayout.Slider("Pitch", aEvent.pitch, -3f, 3f);
				}
		
				var newDelay = EditorGUILayout.Slider("Delay Sound (sec)", aEvent.delaySound, 0f, 10f);
				if (newDelay != aEvent.delaySound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Delay Sound");
					aEvent.delaySound = newDelay;
				}
				break;
			case MasterAudio.EventSoundFunctionType.PlaylistControl:
				var newPlaylistCmd = (MasterAudio.PlaylistCommand) EditorGUILayout.EnumPopup("Playlist Command", aEvent.currentPlaylistCommand);
				if (newPlaylistCmd != aEvent.currentPlaylistCommand) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Command");
					aEvent.currentPlaylistCommand = newPlaylistCmd;
				}
			
				EditorGUI.indentLevel = 1;

				if (aEvent.currentPlaylistCommand != MasterAudio.PlaylistCommand.None) {
					// show Playlist Controller dropdown
					if (EventSounds.playlistCommandsWithAll.Contains(aEvent.currentPlaylistCommand)) {
						var newAllControllers = EditorGUILayout.Toggle("All Playlist Controllers?", aEvent.allPlaylistControllersForGroupCmd);
						if (newAllControllers != aEvent.allPlaylistControllersForGroupCmd) {	
							UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle All Playlist Controllers");
							aEvent.allPlaylistControllersForGroupCmd = newAllControllers;
						}
					}
				
					if (!aEvent.allPlaylistControllersForGroupCmd) {
						if (playlistControllerNames.Count > 0) {
							var existingIndex = playlistControllerNames.IndexOf(aEvent.playlistControllerName);
				
							int? playlistControllerIndex = null;
						
							var noPC = false;
							var noMatch = false;
						
							if (existingIndex >= 1) { 
								playlistControllerIndex = EditorGUILayout.Popup("Playlist Controller", existingIndex, playlistControllerNames.ToArray());
								if (existingIndex == 1) {
									noPC = true;
								}
							}  else if (existingIndex == -1 && aEvent.playlistControllerName == MasterAudio.NO_GROUP_NAME) {
								playlistControllerIndex = EditorGUILayout.Popup("Playlist Controller", existingIndex, playlistControllerNames.ToArray());
							} else { // non-match
								noMatch = true;
		
								var newPlaylistController = EditorGUILayout.TextField("Playlist Controller", aEvent.playlistControllerName);
								if (newPlaylistController != aEvent.playlistControllerName) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Controller");
									aEvent.playlistControllerName = newPlaylistController;
								}
								var newIndex = EditorGUILayout.Popup("All Playlist Controllers", -1, playlistControllerNames.ToArray());
								if (newIndex >= 0) {
									playlistControllerIndex = newIndex;
								}
							}
						
							if (noPC) {
								DTGUIHelper.ShowRedError("No Playlist Controller specified. Event will do nothing.");
							} else if (noMatch) {
								DTGUIHelper.ShowRedError("Playlist Controller found no match. Type in or choose one.");							
							}
						
							if (playlistControllerIndex.HasValue) {
								if (existingIndex != playlistControllerIndex.Value) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Controller");
								}
								if (playlistControllerIndex.Value == -1) {
									aEvent.playlistControllerName = MasterAudio.NO_GROUP_NAME;
								} else {
									aEvent.playlistControllerName = playlistControllerNames[playlistControllerIndex.Value];
								}
							}
						} else {
							var newPlaylistControllerName = EditorGUILayout.TextField("Playlist Controller", aEvent.playlistControllerName);
							if (newPlaylistControllerName != aEvent.playlistControllerName) {
								UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Controller");
								aEvent.playlistControllerName = newPlaylistControllerName;
							}
						}
					}
				}
				
				switch (aEvent.currentPlaylistCommand) {
					case MasterAudio.PlaylistCommand.ChangePlaylist:
						// show playlist name dropdown
						if (maInScene) {
							var existingIndex = playlistNames.IndexOf(aEvent.playlistName);
				
							int? playlistIndex = null;
					
							var noPl = false;
							var noMatch = false;
					
							if (existingIndex >= 1) {
								playlistIndex = EditorGUILayout.Popup("Playlist Name", existingIndex, playlistNames.ToArray());
								if (existingIndex == 1) {
									noPl = true;
								}
							} else if (existingIndex == -1 && aEvent.playlistName == MasterAudio.NO_GROUP_NAME) {
								playlistIndex = EditorGUILayout.Popup("Playlist Name", existingIndex, playlistNames.ToArray());
							} else { // non-match
								noMatch = true;

								var newPlaylist = EditorGUILayout.TextField("Playlist Name", aEvent.playlistName);
								if (newPlaylist != aEvent.playlistName) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Name");
									aEvent.playlistName = newPlaylist;
								}
								var newIndex = EditorGUILayout.Popup("All Playlists", -1, playlistNames.ToArray());
								if (newIndex >= 0) {
									playlistIndex = newIndex;
								}
							}
							
							if (noPl) {
								DTGUIHelper.ShowRedError("No Playlist Name specified. Event will do nothing.");
							} else if (noMatch) {
								DTGUIHelper.ShowRedError("Playlist Name found no match. Type in or choose one.");					
							}
					
							if (playlistIndex.HasValue) {
								if (existingIndex != playlistIndex.Value) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Name");
								}
								if (playlistIndex.Value == -1) {
									aEvent.playlistName = MasterAudio.NO_GROUP_NAME;
								} else {
									aEvent.playlistName = playlistNames[playlistIndex.Value];
								}
							}
						} else {
							var newPlaylistName = EditorGUILayout.TextField("Playlist Name", aEvent.playlistName);
							if (newPlaylistName != aEvent.playlistName) {
								UndoHelper.RecordObjectPropertyForUndo(sounds, "change Playlist Name");
								aEvent.playlistName = newPlaylistName;
							}
						}
						
						var newStartPlaylist = EditorGUILayout.Toggle("Start Playlist?", aEvent.startPlaylist);
						if (newStartPlaylist != aEvent.startPlaylist) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Start Playlist");
							aEvent.startPlaylist = newStartPlaylist;
						}
						break;
					case MasterAudio.PlaylistCommand.FadeToVolume:
						var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
						if (newFadeVol != aEvent.fadeVolume) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Target Volume");
							aEvent.fadeVolume = newFadeVol;
						}

						var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
						if (newFadeTime != aEvent.fadeTime) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Fade Time");
							aEvent.fadeTime = newFadeTime;
						}
						break;
					case MasterAudio.PlaylistCommand.PlayClip:
						var newClip = EditorGUILayout.TextField("Clip Name", aEvent.clipName);
						if (newClip != aEvent.clipName) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Clip Name");
							aEvent.clipName = newClip;
						}
						if (string.IsNullOrEmpty(aEvent.clipName)) {
							DTGUIHelper.ShowRedError("Clip name is empty. Event will do nothing.");
						}
						break;
				}
				break;
			case MasterAudio.EventSoundFunctionType.GroupControl:
				EditorGUI.indentLevel = 1;

				var newGroupCmd = (MasterAudio.SoundGroupCommand) EditorGUILayout.EnumPopup("Group Command", aEvent.currentSoundGroupCommand);
				if (newGroupCmd != aEvent.currentSoundGroupCommand) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Group Command");
					aEvent.currentSoundGroupCommand = newGroupCmd;
				}
			
				if (aEvent.currentSoundGroupCommand != MasterAudio.SoundGroupCommand.None) {
					var newAllTypes = EditorGUILayout.Toggle("Do For Every Group?", aEvent.allSoundTypesForGroupCmd);
					if (newAllTypes != aEvent.allSoundTypesForGroupCmd) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Do For Every Group?");
						aEvent.allSoundTypesForGroupCmd = newAllTypes;
					}
				
					if (!aEvent.allSoundTypesForGroupCmd) {
						if (maInScene) {
							var existingIndex = groupNames.IndexOf(aEvent.soundType);
				
							int? groupIndex = null;
						
							var noGroup = false;	
							var noMatch = false;
						
							if (existingIndex >= 1) {
								groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
								if (existingIndex == 1) {
									noGroup = true;
								}
							
							} else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
								groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
							} else { // non-match
								noMatch = true;
								
								var newSType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
								if (newSType != aEvent.soundType) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
									aEvent.soundType = newSType;
								}
								var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
								if (newIndex >= 0) {
									groupIndex = newIndex;
								}
							}
						
							if (noMatch) {
								DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");					
							} else if (noGroup) {
								DTGUIHelper.ShowRedError("No Sound Group specified. Event will do nothing.");					
							}
						
							if (groupIndex.HasValue) {
								if (existingIndex != groupIndex.Value) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
								}
								if (groupIndex.Value == -1) {
									aEvent.soundType = MasterAudio.NO_GROUP_NAME;
								} else {
									aEvent.soundType = groupNames[groupIndex.Value];
								}
							}
						} else {
							var newSoundT = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
							if (newSoundT != aEvent.soundType) {
								UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
								aEvent.soundType = newSoundT;
							}
						}
					}
				}

				switch (aEvent.currentSoundGroupCommand) {
					case MasterAudio.SoundGroupCommand.None:	
						break;
					case MasterAudio.SoundGroupCommand.FadeToVolume:
						var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
						if (newFadeVol != aEvent.fadeVolume) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Target Volume");
							aEvent.fadeVolume = newFadeVol;
						}

						var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
						if (newFadeTime != aEvent.fadeTime) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Fade Time");
							aEvent.fadeTime = newFadeTime;
						}
						break;
					case MasterAudio.SoundGroupCommand.FadeOutAllOfSound:
						var newFadeT = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
						if (newFadeT != aEvent.fadeTime) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Fade Time");
							aEvent.fadeTime = newFadeT;
						}
						break;
					case MasterAudio.SoundGroupCommand.Mute:	
						break;
					case MasterAudio.SoundGroupCommand.Pause:	
						break;
					case MasterAudio.SoundGroupCommand.Solo:	
						break;
					case MasterAudio.SoundGroupCommand.Unmute:	
						break;
					case MasterAudio.SoundGroupCommand.Unpause:	
						break;
					case MasterAudio.SoundGroupCommand.Unsolo:	
						break;
				}
				
				break;	
			case MasterAudio.EventSoundFunctionType.BusControl:
				var newBusCmd = (MasterAudio.BusCommand) EditorGUILayout.EnumPopup("Bus Command", aEvent.currentBusCommand);
				if (newBusCmd != aEvent.currentBusCommand) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Bus Command");
					aEvent.currentBusCommand = newBusCmd;
				}

				EditorGUI.indentLevel = 1;
			
				if (aEvent.currentBusCommand != MasterAudio.BusCommand.None) {
					var newAllTypes = EditorGUILayout.Toggle("Do For Every Bus?", aEvent.allSoundTypesForBusCmd);
					if (newAllTypes != aEvent.allSoundTypesForBusCmd) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Do For Every Bus?");
						aEvent.allSoundTypesForBusCmd = newAllTypes;
					}
				
					if (!aEvent.allSoundTypesForBusCmd) {
						if (maInScene) {
							var existingIndex = busNames.IndexOf(aEvent.busName);
				
							int? busIndex = null;
						
							var noBus = false;
							var noMatch = false;
						
							if (existingIndex >= 1) {
								busIndex = EditorGUILayout.Popup("Bus Name", existingIndex, busNames.ToArray());
								if (existingIndex == 1) {
									noBus = true;
								}
							} else if (existingIndex == -1 && aEvent.busName == MasterAudio.NO_GROUP_NAME) {
								busIndex = EditorGUILayout.Popup("Bus Name", existingIndex, busNames.ToArray());
							} else { // non-match
								var newBusName = EditorGUILayout.TextField("Bus Name", aEvent.busName);
								if (newBusName != aEvent.busName) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Bus Name");
									aEvent.busName = newBusName;
								}
	
								var newIndex = EditorGUILayout.Popup("All Buses", -1, busNames.ToArray());
								if (newIndex >= 0) {
									busIndex = newIndex;
								}
								noMatch = true;
							}
						
							if (noBus) {
								DTGUIHelper.ShowRedError("No Bus Name specified. Event will do nothing.");	
							} else if (noMatch) {
								DTGUIHelper.ShowRedError("Bus Name found no match. Type in or choose one.");							
							}
						
							if (busIndex.HasValue) {
								if (existingIndex != busIndex.Value) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Bus");
								}
								if (busIndex.Value == -1) {
									aEvent.busName = MasterAudio.NO_GROUP_NAME;
								} else {
									aEvent.busName = busNames[busIndex.Value];
								}
							}
						} else {
							var newBusName = EditorGUILayout.TextField("Bus Name", aEvent.busName);
							if (newBusName != aEvent.busName) {
								UndoHelper.RecordObjectPropertyForUndo(sounds, "change Bus Name");
								aEvent.busName = newBusName;
							}
						}
					}
			
					var newVolume = EditorGUILayout.Slider("Volume", aEvent.volume, 0f, 1f);
					if (newVolume != aEvent.volume) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Volume");
						aEvent.volume = newVolume;
					}

					var newFixPitch = EditorGUILayout.Toggle("Override pitch?", aEvent.useFixedPitch);
					if (newFixPitch != aEvent.useFixedPitch) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Override pitch");
						aEvent.useFixedPitch = newFixPitch;
					}
					if (aEvent.useFixedPitch) {
						DTGUIHelper.ShowColorWarning("*Random pitches for the variation will not be used.");
						var newPitch = EditorGUILayout.Slider("Pitch", aEvent.pitch, -3f, 3f);
						if (newPitch != aEvent.pitch) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Pitch");
							aEvent.pitch = newPitch;
						}
					}
				}	
			
				switch (aEvent.currentBusCommand) {
					case MasterAudio.BusCommand.FadeToVolume:
						var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
						if (newFadeVol != aEvent.fadeVolume) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Target Volume");
							aEvent.fadeVolume = newFadeVol;
						}

						var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
						if (newFadeTime != aEvent.fadeTime) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Fade Time");
							aEvent.fadeTime = newFadeTime;
						}
						break;
					case MasterAudio.BusCommand.Pause:
						break;
					case MasterAudio.BusCommand.Unpause:
						break;
				}
				
				break;
			case MasterAudio.EventSoundFunctionType.CustomEventControl:
				if (eType == EventSounds.EventType.UserDefinedEvent) {
					DTGUIHelper.ShowRedError("Custom Event Receivers cannot fire events. Select another Action Type.");
					break;
				}	
				
				var newEventCmd = (MasterAudio.CustomEventCommand) EditorGUILayout.EnumPopup("Custom Event Command", aEvent.currentCustomEventCommand);
				if (newEventCmd != aEvent.currentCustomEventCommand) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Custom Event Command");
					aEvent.currentCustomEventCommand = newEventCmd;
				}
				EditorGUI.indentLevel = 1;
			
				switch (aEvent.currentCustomEventCommand) {
					case MasterAudio.CustomEventCommand.FireEvent:
						if (maInScene) {
							var existingIndex = customEventNames.IndexOf(aEvent.customEventName);
				
							int? customEventIndex = null;
			
							EditorGUI.indentLevel = 1;
						
							var noEvent = false;
							var noMatch = false;
						
							if (existingIndex >= 1) {
								customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
								if (existingIndex == 1) {
									noEvent = true;
								}
							} else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
								customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
							} else { // non-match
								noMatch = true;
								var newEventName = EditorGUILayout.TextField("Custom Event Name", aEvent.customEventName);
								if (newEventName != aEvent.customEventName) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Custom Event Name");
									aEvent.customEventName = newEventName;
								}
			
								var newIndex = EditorGUILayout.Popup("All Custom Events", -1, customEventNames.ToArray());
								if (newIndex >= 0) {
									customEventIndex = newIndex;
								}
							}
							
							if (noEvent) {	
								DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");					
							} else if (noMatch) {
								DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
							}
							
							if (customEventIndex.HasValue) {
								if (existingIndex != customEventIndex.Value) {
									UndoHelper.RecordObjectPropertyForUndo(sounds, "change Custom Event");
								}
								if (customEventIndex.Value == -1) {
									aEvent.customEventName = MasterAudio.NO_GROUP_NAME;
								} else {
									aEvent.customEventName = customEventNames[customEventIndex.Value];
								}
							}
						} else {
							var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", aEvent.customEventName);
							if (newCustomEvent != aEvent.customEventName) {
								UndoHelper.RecordObjectPropertyForUndo(sounds, "Custom Event Name");
								aEvent.customEventName = newCustomEvent;
							}
						}

						break;
				}
			
				break;			
		}
		
		EditorGUI.indentLevel = 0;

		var newEmit = EditorGUILayout.Toggle("Emit Particle", aEvent.emitParticles);
		if (newEmit != aEvent.emitParticles) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Emit Particle");
			aEvent.emitParticles = newEmit;
		}
		if (aEvent.emitParticles) {
			var newParticleCount = EditorGUILayout.IntSlider("Particle Count", aEvent.particleCountToEmit, 1, 100);
			if (newParticleCount != aEvent.particleCountToEmit) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "change Particle Count");
				aEvent.particleCountToEmit = newParticleCount;
			}
		}

		if (showLayerTagFilter) {
			var newUseLayers = EditorGUILayout.BeginToggleGroup("Layer filters", aEvent.useLayerFilter);
			if (newUseLayers != aEvent.useLayerFilter) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Layer filters");
				aEvent.useLayerFilter = newUseLayers;
			}
			if (aEvent.useLayerFilter) {
				for (var i = 0; i < aEvent.matchingLayers.Count; i++) {
					var newLayer = EditorGUILayout.LayerField("Layer Match " + (i + 1), aEvent.matchingLayers[i]);
					if (newLayer != aEvent.matchingLayers[i]) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Layer filter");
						aEvent.matchingLayers[i] = newLayer;
					}
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(24);

				if (GUILayout.Button(new GUIContent("Add", "Click to add a layer match at the end"), GUILayout.Width(60))) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "add Layer filter");
					aEvent.matchingLayers.Add(0);
					isDirty = true;
				}
				if (aEvent.matchingLayers.Count > 1) {
					if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last layer match"), GUILayout.Width(60))) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "remove Layer filter");
						aEvent.matchingLayers.RemoveAt(aEvent.matchingLayers.Count - 1);
						isDirty = true;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();

			var newTagFilter = EditorGUILayout.BeginToggleGroup("Tag filter", aEvent.useTagFilter);
			if (newTagFilter != aEvent.useTagFilter) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Tag filter");
				aEvent.useTagFilter = newTagFilter;
			}

			if (aEvent.useTagFilter) {
				for (var i = 0; i < aEvent.matchingTags.Count; i++) {
					var newTag = EditorGUILayout.TagField("Tag Match " + (i + 1), aEvent.matchingTags[i]);
					if (newTag != aEvent.matchingTags[i]) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Tag filter");
						aEvent.matchingTags[i] = newTag;
					}
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(24);
				if (GUILayout.Button(new GUIContent("Add", "Click to add a tag match at the end"), GUILayout.Width(60))) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "Add Tag filter");
					aEvent.matchingTags.Add("Untagged");
					isDirty = true;
				}
				if (aEvent.matchingTags.Count > 1) {
					if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last tag match"), GUILayout.Width(60))) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "remove Tag filter");
						aEvent.matchingTags.RemoveAt(aEvent.matchingLayers.Count - 1);
						isDirty = true;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();
		}

		return isDirty;
	}
	
	private void CreateCustomEvent(bool recordUndo) {
		var newEvent = new AudioEvent();
		newEvent.isCustomEvent = true;
		newEvent.customSoundActive = true;
		
		if (recordUndo) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "add Custom Event Sound");
		}
		
		sounds.userDefinedSounds.Add(newEvent);
	}
}
