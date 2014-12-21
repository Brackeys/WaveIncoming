using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EventCalcSounds))]
[CanEditMultipleObjects]
public class AudioCalcEventInspector : Editor {
	private List<string> groupNames = null;
	private bool maInScene;

	public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 1;

		var ma = MasterAudio.Instance;
		if (ma != null) {
			DTGUIHelper.ShowHeaderTexture(ma.logoTexture);
		}
		
		EventCalcSounds sounds = (EventCalcSounds)target;

		maInScene = ma != null;		
		if (maInScene) {
			groupNames = ma.GroupNames;
		}
		
		GUILayout.Label("Group Controls", EditorStyles.boldLabel);

		var newSpawnMode = (MasterAudio.SoundSpawnLocationMode) EditorGUILayout.EnumPopup("Sound Spawn Mode", sounds.soundSpawnMode);
		if (newSpawnMode != sounds.soundSpawnMode) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "change Spawn Mode");
			sounds.soundSpawnMode = newSpawnMode;
		}

		var newDisable = EditorGUILayout.Toggle("Disable Sounds", sounds.disableSounds);
		if (newDisable != sounds.disableSounds) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Disable Sounds");
			sounds.disableSounds = newDisable;
		}

		EditorGUILayout.Separator();
		GUILayout.Label("Sound Triggers", EditorStyles.boldLabel);
		
		var disabledText = "";
		if (sounds.disableSounds) {
			disabledText = " (DISABLED) ";
		}
		
		var aud = sounds.GetComponent<AudioSource>();
		if (aud == null || aud.clip == null) {
			GUI.color = Color.green;
			EditorGUILayout.LabelField("Audio Source Ended Sound - needs AudioSource component.", EditorStyles.whiteMiniLabel);
			GUI.color = Color.white;
			sounds.useAudioSourceEndedSound = false;
		} else {
			var newAudioEnded = EditorGUILayout.BeginToggleGroup("Audio Source Ended" + disabledText, sounds.useAudioSourceEndedSound);
			if (newAudioEnded != sounds.useAudioSourceEndedSound) {
				UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Audio  Source Ended");
				sounds.useAudioSourceEndedSound = newAudioEnded;
			}
			if (sounds.useAudioSourceEndedSound && !sounds.disableSounds) {
				EditorGUI.indentLevel = 2;

				if (maInScene) {
					var existingIndex = groupNames.IndexOf(sounds.audioSourceEndedSound.soundType);
		
					int? groupIndex = null;
					var noMatch = false;
					
					if (existingIndex >= 1) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
					} else if (existingIndex == -1 && sounds.audioSourceEndedSound.soundType == MasterAudio.NO_GROUP_NAME) {
						groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
					} else { // non-match
						noMatch = true;
						var newGroup = EditorGUILayout.TextField("Sound Group", sounds.audioSourceEndedSound.soundType);
						if (newGroup != sounds.audioSourceEndedSound.soundType) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
							sounds.audioSourceEndedSound.soundType = newGroup;
						}
						var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
						if (newIndex >= 0) {
							groupIndex = newIndex;
						}
					}
					
					if (noMatch) {
						DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
					}
					
					if (groupIndex.HasValue) {
						if (existingIndex != groupIndex.Value) {
							UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
						}

						if (groupIndex.Value == -1) {
							sounds.audioSourceEndedSound.soundType = MasterAudio.NO_GROUP_NAME;
						} else {
							sounds.audioSourceEndedSound.soundType = groupNames[groupIndex.Value];
						}
					}
				} else {
					var newSoundGroup = EditorGUILayout.TextField("Sound Group", sounds.audioSourceEndedSound.soundType);
					if (newSoundGroup != sounds.audioSourceEndedSound.soundType) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
						sounds.audioSourceEndedSound.soundType = newSoundGroup;
					}
				}

				var newVolume = EditorGUILayout.Slider("Volume", sounds.audioSourceEndedSound.volume, 0f, 1f);
				if (newVolume != sounds.audioSourceEndedSound.volume) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Volume");
					sounds.audioSourceEndedSound.volume = newVolume;
				} 

				var newPitch = EditorGUILayout.Toggle("Override pitch?", sounds.audioSourceEndedSound.useFixedPitch);
				if (newPitch != sounds.audioSourceEndedSound.useFixedPitch) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Override pitch");
					sounds.audioSourceEndedSound.useFixedPitch = newPitch;
				}

				if (sounds.audioSourceEndedSound.useFixedPitch) {
					DTGUIHelper.ShowColorWarning("*Random pitches for the variation will not be used.");
					var newFixedPitch = EditorGUILayout.Slider("Pitch", sounds.audioSourceEndedSound.pitch, -3f, 3f);
					if (newFixedPitch != sounds.audioSourceEndedSound.pitch) {
						UndoHelper.RecordObjectPropertyForUndo(sounds, "change Pitch");
						sounds.audioSourceEndedSound.pitch = newFixedPitch;
					}
				}

				var newDelay = EditorGUILayout.Slider("Delay Sound (sec)", sounds.audioSourceEndedSound.delaySound, 0f, 10f);
				if (newDelay != sounds.audioSourceEndedSound.delaySound) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Delay Sound");
					sounds.audioSourceEndedSound.delaySound = newDelay;
				}

				var newEmit = EditorGUILayout.Toggle("Emit Particle", sounds.audioSourceEndedSound.emitParticles);
				if (newEmit != sounds.audioSourceEndedSound.emitParticles) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "toggle Emit Particle");
					sounds.audioSourceEndedSound.emitParticles = newEmit;
				}

				var newParticleCount = EditorGUILayout.IntSlider("Particle Count", sounds.audioSourceEndedSound.particleCountToEmit, 1, 100);
				if (newParticleCount != sounds.audioSourceEndedSound.particleCountToEmit) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Particle Count");
					sounds.audioSourceEndedSound.particleCountToEmit = newParticleCount;
				}
			}
			EditorGUILayout.EndToggleGroup();
		}
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}

		this.Repaint();

		//DrawDefaultInspector();
    }
}
