using UnityEngine;
using UnityEditor;
using System.Collections;

public static class DTGUIHelper {
	private const string ALERT_TITLE = "Master Audio Alert";
	private const string ALERT_OK_TEXT = "Ok";
	private const string FOLD_OUT_TOOLTIP = "Click to expand or collapse";
    private const float LED_FRAME_TIME = .07f;
	
	public enum JukeboxButtons {
		None,
		NextSong,
		Pause,
		Play,
		RandomSong,
		Stop
	}
	
	public enum DTFunctionButtons { 
		None, 
		Add, 
		Remove, 
		Mute,
		Solo,
		Go,
		ShiftUp,
		ShiftDown,
		Play,
		Stop,
		Rename
	}

	public static void ShowHeaderTexture(Texture tex) {
		Rect rect = GUILayoutUtility.GetRect(0f, 0f);
		rect.width = tex.width;
		rect.height = tex.height;
		GUILayout.Space(rect.height);
		GUI.DrawTexture(rect, tex);
		
		var e = Event.current;
		if (e.type == EventType.MouseUp) {
			if (rect.Contains(e.mousePosition)) {
				var ma = MasterAudio.Instance;
				if (ma != null) {
					Selection.activeObject = ma.gameObject;
				}
			}
		}
	}

	public static DTFunctionButtons AddCustomEventDeleteIcon(bool showRenameButton) {
		GUI.contentColor = Color.green;
		
		var shouldRename = false;
		if (showRenameButton) {
			shouldRename = GUILayout.Button(new GUIContent("Rename", "Click to rename Custom Event"), EditorStyles.toolbarButton, GUILayout.MaxWidth(50));
		}
		
		var shouldDelete = GUILayout.Button(new GUIContent("Delete", "Click to delete Custom Event"), EditorStyles.toolbarButton, GUILayout.MaxWidth(50));
		GUI.contentColor = Color.white;
		
		if (shouldDelete) {
			return DTFunctionButtons.Remove;
		}
		if (shouldRename) {
			return DTFunctionButtons.Rename;
		}
		
		return DTFunctionButtons.None;
	}
	
	public static bool AddDeleteIcon(MasterAudio sounds, string itemName) {
		var deleteIcon = sounds.deleteTexture;
		return GUILayout.Button(new GUIContent(deleteIcon, "Click to delete " + itemName), EditorStyles.toolbarButton, GUILayout.MaxWidth(30));
	}

	public static bool AddDynamicDeleteIcon(DynamicSoundGroupCreator creator, string itemName) {
		var deleteIcon = creator.deleteTexture;
		return GUILayout.Button(new GUIContent(deleteIcon, "Click to delete " + itemName), EditorStyles.toolbarButton, GUILayout.MaxWidth(30));
	}
	
	public static JukeboxButtons AddJukeboxIcons(MasterAudio sounds) {
		JukeboxButtons buttonPressed = JukeboxButtons.None;
		
		var stopIcon = sounds.stopTrackTexture;
		var stopContent = stopIcon == null ? new GUIContent("Stop", "Stop Playlist") : new GUIContent(stopIcon, "Stop Playlist");
		var buttonWidth = stopIcon == null ? 50 : 30;
		if (GUILayout.Button(stopContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(buttonWidth))) {
			buttonPressed = JukeboxButtons.Stop;
		}
		
		var pauseIcon = sounds.pauseTrackTexure;
		var pauseContent = pauseIcon == null ? new GUIContent("Pause", "Pause Playlist") : new GUIContent(pauseIcon, "Pause Playlist");
		buttonWidth = pauseIcon == null ? 50 : 30;
		if (GUILayout.Button(pauseContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(buttonWidth))) {
			buttonPressed = JukeboxButtons.Pause;
		}

		var playIcon = sounds.playTrackTexture;
		var playContent = playIcon == null ? new GUIContent("Play", "Play Playlist") : new GUIContent(playIcon, "Play Playlist");
		buttonWidth = playIcon == null ? 50 : 30;
		if (GUILayout.Button(playContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(buttonWidth))) {
			buttonPressed = JukeboxButtons.Play;
		}
		
		var nextTrackIcon = sounds.nextTrackTexture;
		var nextContent = nextTrackIcon == null ? new GUIContent("Next", "Next Track in Playlist") : new GUIContent(nextTrackIcon, "Next Track in Playlist");
		buttonWidth = nextTrackIcon == null ? 50 : 30;
		if (GUILayout.Button(nextContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(buttonWidth))) {
			buttonPressed = JukeboxButtons.NextSong;
		}

		GUILayout.Space(10);
		
		var randomIcon = sounds.randomTrackTexure;
		var randomContent = randomIcon == null ? new GUIContent("Random", "Random Track in Playlist") : new GUIContent(randomIcon, "Random Track in Playlist");
		buttonWidth = randomIcon == null ? 50 : 30;
		if (GUILayout.Button(randomContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(buttonWidth))) {
			buttonPressed = JukeboxButtons.RandomSong;
		}
		
		if (!Application.isPlaying) {
			buttonPressed = JukeboxButtons.None;
		}
		
		return buttonPressed;
	}
	
	public static DTFunctionButtons AddDynamicGroupButtons(DynamicSoundGroupCreator creator) {
		GUIContent deleteIcon;
		GUIContent settingsIcon;
		GUIContent previewIcon;
		GUIContent stopPreviewIcon;
		
		if (creator.deleteTexture != null) {
			deleteIcon = new GUIContent(creator.deleteTexture, "Click to delete Group");
		} else {
			deleteIcon = new GUIContent("Delete", "Click to delete Group");
		}
		
		if (creator.settingsTexture != null) {
			settingsIcon = new GUIContent(creator.settingsTexture, "Click to edit Group");
		} else {
			settingsIcon = new GUIContent("Edit", "Click to edit Group");
		}
		
		if (creator.playTexture != null) {
			previewIcon = new GUIContent(creator.playTexture, "Click to preview Group");
		} else {
			previewIcon = new GUIContent("Preview", "Click to preview Group");
		}

		if (creator.stopTrackTexture != null) {
			stopPreviewIcon = new GUIContent(creator.stopTrackTexture, "Click to stop previewing Group");
		} else {
			stopPreviewIcon = new GUIContent("End Preview", "Click to stop previewing Group");
		}
		
        if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Go;
		}
	
		if (GUILayout.Button(previewIcon, EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Play;
		}

		if (GUILayout.Button(stopPreviewIcon, EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Stop;			
		}

		if (!Application.isPlaying) {
			if (GUILayout.Button(deleteIcon, EditorStyles.toolbarButton)) {				
				return DTFunctionButtons.Remove;
			}
		}
		
		return DTFunctionButtons.None;
	}
	
	public static DTFunctionButtons AddMixerBusButtons(GroupBus gb, MasterAudio sounds) {
		var deleteIcon = sounds.deleteTexture;
		
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute bus");
		
		if (gb.isMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		var soloContent = new GUIContent(sounds.soloOffTexture, "Click to solo bus");

		if (gb.isSoloed) {
			soloContent.image = sounds.soloOnTexture;
		}
		
		bool soloPressed = GUILayout.Button(soloContent, EditorStyles.toolbarButton);
		bool mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
        bool removePressed = false; 

		if (!Application.isPlaying) {
			removePressed = GUILayout.Button(new GUIContent(deleteIcon, "Click to delete bus"), EditorStyles.toolbarButton);
		} else {
			GUILayout.Space(26);
		}
		
        // Return the pressed button if any
        if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}          
		if (soloPressed == true) {
			return DTFunctionButtons.Solo;
		}
		if (mutePressed == true) {
			return DTFunctionButtons.Mute;
		}

		return DTFunctionButtons.None;
	}

	public static DTFunctionButtons AddDynamicVariationButtons(DynamicGroupVariation _variation) {
		if (GUILayout.Button(new GUIContent(_variation.playTexture, "Click to preview Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Play;
		}

		if (GUILayout.Button(new GUIContent(_variation.stopTrackTexture, "Click to stop audio preview"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Stop;
		}
		
		return DTFunctionButtons.None;
	}
	
	public static DTFunctionButtons AddDynamicGroupButtons(DynamicSoundGroup _group) {
		if (GUILayout.Button(new GUIContent(_group.playTexture, "Click to preview Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Play;
		}

		if (GUILayout.Button(new GUIContent(_group.stopTrackTexture, "Click to stop audio preview"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Stop;
		}
		
		return DTFunctionButtons.None;
	}
	
	public static DTFunctionButtons AddVariationButtons(MasterAudio sounds) {
		if (GUILayout.Button(new GUIContent(sounds.playTexture, "Click to preview Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Play;
		}

		if (GUILayout.Button(new GUIContent(sounds.stopTrackTexture, "Click to stop audio preview"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
			return DTFunctionButtons.Stop;
		}
		
		return DTFunctionButtons.None;
	}

    public static DTFunctionButtons AddMixerMuteButton(string itemName, MasterAudio sounds) {
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute " + itemName);
		
		if (sounds.mixerMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		bool mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
		if (mutePressed == true) {
			return DTFunctionButtons.Mute;
		}
		
        return DTFunctionButtons.None;
	}	

    public static DTFunctionButtons AddPlaylistMuteButton(string itemName, MasterAudio sounds) {
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute " + itemName);
		
		if (sounds.playlistsMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		bool mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
		if (mutePressed == true) {
			return DTFunctionButtons.Mute;
		}
		
        return DTFunctionButtons.None;
	}	
	
	public static void AddLedSignalLight(MasterAudio sounds, string groupName) {
		if (sounds.ledTextures.Length == 0) {
			return;
		}
		
		GUIContent content = new GUIContent(sounds.ledTextures[sounds.ledTextures.Length - 1]); 
		
		if (Application.isPlaying) {
			var groupInfo = MasterAudio.GetGroupInfo(groupName);
			if (groupInfo != null && groupInfo._lastTimePlayed > 0f && groupInfo._lastTimePlayed <= Time.time) {
				var timeDiff = Time.time - groupInfo._lastTimePlayed;
				
				var timeSlot = (int)(timeDiff / LED_FRAME_TIME);
				
				if (timeSlot >= 4 && timeSlot < 5) {
					content = new GUIContent(sounds.ledTextures[4]); 
				} else if (timeSlot >= 3 && timeSlot < 4) {
					content = new GUIContent(sounds.ledTextures[3]); 
				} else if (timeSlot >= 2 && timeSlot < 3) {
					content = new GUIContent(sounds.ledTextures[2]); 
				} else if (timeSlot >= 1 && timeSlot < 2) {
					content = new GUIContent(sounds.ledTextures[1]); 
				} else if (timeSlot >= 0 && timeSlot < 1f) {
					content = new GUIContent(sounds.ledTextures[0]); 
				}
			}
		} 
		
		GUILayout.Label(content, EditorStyles.toolbarButton, GUILayout.Width(26));
	}
	
    public static DTFunctionButtons AddMixerButtons(MasterAudioGroup aGroup, string itemName, MasterAudio sounds)
    {
		var deleteIcon = sounds.deleteTexture;
		var settingsIcon = sounds.settingsTexture;
		
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute " + itemName);
		
		if (aGroup.isMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		var soloContent = new GUIContent(sounds.soloOffTexture, "Click to solo " + itemName);

		if (aGroup.isSoloed) {
			soloContent.image = sounds.soloOnTexture;
		}
		
		if (GUILayout.Button(soloContent, EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Solo;
		}
		if (GUILayout.Button(muteContent, EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Mute;
		}
		
        if (GUILayout.Button(new GUIContent(settingsIcon, "Click to edit " + itemName), EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Go;
		}
		
		if (GUILayout.Button(new GUIContent(sounds.playTexture, "Click to preview " + itemName), EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Play;
		}
		if (GUILayout.Button(new GUIContent(sounds.stopTrackTexture, "Click to stop all of Sound"), EditorStyles.toolbarButton)) {
			return DTFunctionButtons.Stop;			
		}

		if (!Application.isPlaying) {
			if (GUILayout.Button(new GUIContent(deleteIcon, "Click to delete " + itemName), EditorStyles.toolbarButton)) {				
				return DTFunctionButtons.Remove;
			}
		}
		
        return DTFunctionButtons.None;
    }

    public static DTFunctionButtons AddPlaylistControllerSetupButtons(PlaylistController controller, string itemName, MasterAudio sounds, bool jukeboxMode)
    {
		var deleteIcon = sounds.deleteTexture;
		var settingsIcon = sounds.settingsTexture;
		
		var muteContent = new GUIContent(sounds.muteOffTexture, "Click to mute " + itemName);
		if (controller.isMuted) {
			muteContent.image = sounds.muteOnTexture;
		}
		
		var mutePressed = GUILayout.Button(muteContent, EditorStyles.toolbarButton);
		
		if (!jukeboxMode) {
			// Remove Button - Process presses later
	        bool goPressed = GUILayout.Button(new GUIContent(settingsIcon, "Click to edit " + itemName), EditorStyles.toolbarButton);
	        bool removePressed = false; 
			
			if (Application.isPlaying) {
				//GUILayout.Space(26);
			} else {
				removePressed = GUILayout.Button(new GUIContent(deleteIcon, "Click to delete " + itemName), EditorStyles.toolbarButton);
			}
			
			if (removePressed) {
				return DTFunctionButtons.Remove;
			}         
			if (goPressed) {
				return DTFunctionButtons.Go;
			}
		}

        // Return the pressed button if any
		if (mutePressed) {		
			return DTFunctionButtons.Mute;
		}
		
        return DTFunctionButtons.None;
    }
	
    public static DTFunctionButtons AddFoldOutListItemButtons(int position, int totalPositions, string itemName, bool showAfterText, bool showMoveButtons = false, bool showAudioPreview = false)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

        // A little space between button groups
        GUILayout.Space(24);
		
		bool upPressed = false;
		bool downPressed = false;
		
		if (showAudioPreview) {
	   		var ma = MasterAudio.Instance;
			if (ma != null) {
				if (GUILayout.Button(new GUIContent(ma.playTexture, "Click to preview clip"), 
						EditorStyles.toolbarButton)) {
					return DTFunctionButtons.Play;
				}
				if (GUILayout.Button(new GUIContent(ma.stopTrackTexture, "Click to stop previewing clip"), 
						EditorStyles.toolbarButton)) {
					return DTFunctionButtons.Stop;
				}
			}
		}
		
		if (showMoveButtons) {
	        if (position > 0) {
				// the up arrow.
				var upArrow = '\u25B2'.ToString();
        		upPressed = GUILayout.Button(new GUIContent(upArrow, "Click to shift " + itemName + " up"),
                                          EditorStyles.toolbarButton);
			}

			if (position < totalPositions - 1) {
	        	// The down arrow will move things towards the end of the List
				var dnArrow = '\u25BC'.ToString();
	        	downPressed = GUILayout.Button(new GUIContent(dnArrow, "Click to shift " + itemName + " down"), 
					EditorStyles.toolbarButton);
			}
		}

        var buttonText = "Click to add new " + itemName;
		if (showAfterText) {
			buttonText += " after this one";
		}
		bool addPressed = false;	
		
        // Add button - Process presses later
		addPressed = GUILayout.Button(new GUIContent("+", buttonText),
                                           EditorStyles.toolbarButton);

		// Remove Button - Process presses later
        bool removePressed = GUILayout.Button(new GUIContent("-", "Click to remove " + itemName),
	                                              EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();

        // Return the pressed button if any
        if (removePressed == true) {
			return DTFunctionButtons.Remove;
		}         
		if (addPressed == true) {
			return DTFunctionButtons.Add;
		}
		if (upPressed) {
			return DTFunctionButtons.ShiftUp;
		}
		if (downPressed) {
			return DTFunctionButtons.ShiftDown;
		}

        return DTFunctionButtons.None;
    }
	
	public static bool Foldout(bool expanded, string label)
    {
        var content = new GUIContent(label, FOLD_OUT_TOOLTIP);
        expanded = EditorGUILayout.Foldout(expanded, content);

        return expanded;
    }
	
	public static void ShowColorWarning(string warningText) {
		GUI.color = Color.green;
		EditorGUILayout.LabelField(warningText, EditorStyles.miniLabel);
		GUI.color = Color.white;
	}

	public static void ShowRedError(string errorText) {
		GUI.color = Color.red;
		EditorGUILayout.LabelField(errorText, EditorStyles.toolbarButton);
		GUI.color = Color.white;
	}

	public static void ShowLargeBarAlert(string errorText) {
		GUI.color = Color.yellow;
		EditorGUILayout.LabelField(errorText, EditorStyles.toolbarButton);
		GUI.color = Color.white;
	}
	
	public static void ShowAlert(string text) {
		if (Application.isPlaying) {
			Debug.LogWarning(text);
		} else {
			EditorUtility.DisplayDialog(DTGUIHelper.ALERT_TITLE, text,
					DTGUIHelper.ALERT_OK_TEXT);
		}
	}

	public static string GetResourcePath(AudioClip audioClip) {
		var fullPath = AssetDatabase.GetAssetPath(audioClip);
		var index = fullPath.ToLower().IndexOf("/resources/");
		if (index <= -1) {
			ShowAlert("You have dragged an Audio Clip that is not in a Resource folder while in Resource file mode. Creation succeeded, but this Group / Variation will probably not function.");
			return null;
		}
		
		var shortPath = fullPath.Substring(index + 11);
		
		var dotIndex = shortPath.LastIndexOf(".");
		if (dotIndex >= 0) {
			shortPath = shortPath.Substring(0, dotIndex);
		}
		return shortPath;
	}
	
	private static PrefabType GetPrefabType(Object gObject) {
		return PrefabUtility.GetPrefabType(gObject);
	}
	
	public static bool IsPrefabInProjectView(Object gObject) {
		return GetPrefabType(gObject) == PrefabType.Prefab;
	}
}
