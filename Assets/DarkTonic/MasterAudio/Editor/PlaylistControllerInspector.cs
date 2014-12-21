using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlaylistController))]
public class PlaylistControllerInspector : Editor {
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 0;
		
		PlaylistController controller = (PlaylistController)target;
		
		MasterAudio.Instance = null;
		
		var ma = MasterAudio.Instance;
		if (ma != null) {
			DTGUIHelper.ShowHeaderTexture(ma.logoTexture);
		}
		
		var newVol = EditorGUILayout.Slider("Playlist Volume", controller.playlistVolume, 0f, 1f);
		if (newVol != controller.playlistVolume) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "change Playlist Volume");
			controller.playlistVolume = newVol;
			controller.UpdateMasterVolume();
		}
		
		ma = MasterAudio.Instance;
		if (ma != null) {
			var plNames = MasterAudio.Instance.PlaylistNames;
			
			var existingIndex = plNames.IndexOf(controller.startPlaylistName);
			
			int? groupIndex = null;
			
			var noPl = false;
			var noMatch = false;
			
			if (existingIndex >= 1) {
				groupIndex = EditorGUILayout.Popup("Initial Playlist", existingIndex, plNames.ToArray());
				if (existingIndex == 1) {
					noPl = true;
				}
			} else if (existingIndex == -1 && controller.startPlaylistName == MasterAudio.NO_GROUP_NAME) {
				groupIndex = EditorGUILayout.Popup("Initial Playlist", existingIndex, plNames.ToArray());
			} else { // non-match
				noMatch = true;
				var newPlaylist = EditorGUILayout.TextField("Initial Playlist", controller.startPlaylistName);
				if (newPlaylist != controller.startPlaylistName) {
					UndoHelper.RecordObjectPropertyForUndo(controller, "change Initial Playlist");
					controller.startPlaylistName = newPlaylist;
				}
				
				var newIndex = EditorGUILayout.Popup("All Playlists", -1, plNames.ToArray());
				if (newIndex >= 0) {
					groupIndex = newIndex;
				}
			}
			
			if (noPl) {
				DTGUIHelper.ShowRedError("Initial Playlist not specified. No music will play.");
			} else if (noMatch) {
				DTGUIHelper.ShowRedError("Initial Playlist found no match. Type in or choose one from 'All Playlists'.");
			}
			
			if (groupIndex.HasValue) {
				if (existingIndex != groupIndex.Value) {
					UndoHelper.RecordObjectPropertyForUndo(controller, "change Initial Playlist");
				}
				if (groupIndex.Value == -1) {
					controller.startPlaylistName = MasterAudio.NO_GROUP_NAME;
				} else {
					controller.startPlaylistName = plNames[groupIndex.Value];
				}
			}
		}
		
		
		var syncGroupList = new List<string>();
		for (var i = 0; i < 4; i++) {
			syncGroupList.Add((i + 1).ToString());
		}
		syncGroupList.Insert(0, MasterAudio.NO_GROUP_NAME);

		var syncIndex = syncGroupList.IndexOf(controller.syncGroupNum.ToString());
		if (syncIndex == -1) {
			syncIndex = 0;
		}
		var newSync = EditorGUILayout.Popup("Controller Sync Group", syncIndex, syncGroupList.ToArray());
		if (newSync != syncIndex) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "change Controller Sync Group");
			controller.syncGroupNum = newSync;
		}
		
		EditorGUI.indentLevel = 0;
		var newAwake = EditorGUILayout.Toggle("Start Playlist on Awake?", controller.startPlaylistOnAwake);
		if (newAwake != controller.startPlaylistOnAwake) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "toggle Start Playlist on Awake");
			controller.startPlaylistOnAwake = newAwake;
		}
		
		var newShuffle = EditorGUILayout.Toggle("Shuffle Mode", controller.isShuffle);
		if (newShuffle != controller.isShuffle) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "toggle Shuffle Mode");
			controller.isShuffle = newShuffle;
		}
		
		var newLoop = EditorGUILayout.Toggle("Loop Playlists", controller.loopPlaylist);
		if (newLoop != controller.loopPlaylist) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "toggle Loop Playlists");
			controller.loopPlaylist = newLoop;
		}
		
		var newAuto = EditorGUILayout.Toggle("Auto advance clips", controller.isAutoAdvance);
		if (newAuto != controller.isAutoAdvance) {
			UndoHelper.RecordObjectPropertyForUndo(controller, "toggle Auto advance clips");
			controller.isAutoAdvance = newAuto;
		}
		
		DTGUIHelper.ShowColorWarning("*Note: auto advance will not advance past a looped track.");

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
		
		this.Repaint();
		
		//DrawDefaultInspector();
	}
}