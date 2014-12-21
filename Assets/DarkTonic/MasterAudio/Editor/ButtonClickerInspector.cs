using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ButtonClicker))]
[CanEditMultipleObjects]
public class ButtonClickerInspector : Editor
{
	private List<string> groupNames = null;
	private bool maInScene;
	
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 0;
		
		var ma = MasterAudio.Instance;
		if (ma != null) {
			DTGUIHelper.ShowHeaderTexture(ma.logoTexture);
		}
		
		ButtonClicker sounds = (ButtonClicker)target;
		
		maInScene = ma != null;		
		if (maInScene) {
			groupNames = ma.GroupNames;
		}
		
		var resizeOnClick = EditorGUILayout.Toggle("Resize On Click", sounds.resizeOnClick);
		 
		if (resizeOnClick != sounds.resizeOnClick) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "change Resize On Click");
			sounds.resizeOnClick = resizeOnClick;
		}

		var resizeOnHover = EditorGUILayout.Toggle("Resize On Hover", sounds.resizeOnHover);

		if (resizeOnHover != sounds.resizeOnHover) {
			UndoHelper.RecordObjectPropertyForUndo(sounds, "change Resize On Hover");
			sounds.resizeOnHover = resizeOnHover;
		}
		
		EditSoundGroup(sounds, ref sounds.mouseDownSound, "Mouse Down Sound");
		EditSoundGroup(sounds, ref sounds.mouseUpSound, "Mouse Up Sound");
		EditSoundGroup(sounds, ref sounds.mouseClickSound, "Mouse Click Sound");
		EditSoundGroup(sounds, ref sounds.mouseOverSound, "Mouse Over Sound");
		EditSoundGroup(sounds, ref sounds.mouseOutSound, "Mouse Out Sound");
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}

		this.Repaint();

		//DrawDefaultInspector();
	}

	void EditSoundGroup(ButtonClicker sounds, ref string soundGroup, string label)
	{
		if (maInScene) {
			var existingIndex = groupNames.IndexOf(soundGroup);

			int? groupIndex = null;

			var noMatch = false;

			if (existingIndex >= 1) {
				groupIndex = EditorGUILayout.Popup(label, existingIndex, groupNames.ToArray());
			} else if (existingIndex == -1 && soundGroup == MasterAudio.NO_GROUP_NAME) {
				groupIndex = EditorGUILayout.Popup(label, existingIndex, groupNames.ToArray());
			} else { // non-match
				noMatch = true;

				var newGroup = EditorGUILayout.TextField(label, soundGroup);
				if (newGroup != soundGroup) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
					soundGroup = newGroup;
				}
				var newIndex = EditorGUILayout.Popup("All Sound Types", -1, groupNames.ToArray());
				if (newIndex >= 0) {
					groupIndex = newIndex;
				}
			}

			if (noMatch) {
				DTGUIHelper.ShowRedError("Sound Type found no match. Choose one from 'All Sound Types'.");
			}

			if (groupIndex.HasValue) {
				if (existingIndex != groupIndex.Value) {
					UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
				}
				if (groupIndex.Value == -1) {
					soundGroup = MasterAudio.NO_GROUP_NAME;
				} else {
					soundGroup = groupNames[groupIndex.Value];
				}
			}
		} else {
			var newGroup = EditorGUILayout.TextField(label, soundGroup);
			if (newGroup != soundGroup) {
				soundGroup = newGroup;
				UndoHelper.RecordObjectPropertyForUndo(sounds, "change Sound Group");
			}
		}
	}
}
