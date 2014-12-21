using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MasterAudioManager : EditorWindow {
    public const string MasterAudioFolderPath = "Assets/DarkTonic/MasterAudio";

    Vector2 scrollPos = Vector2.zero;
	
	[MenuItem("Window/Master Audio Manager")]
	static void Init()
    {
        EditorWindow.GetWindow(typeof(MasterAudioManager));
    }
	
	void OnGUI() {
		scrollPos = GUI.BeginScrollView (
				new Rect (0, 0, position.width, position.height), 
				scrollPos, 
				new Rect (0, 0, 520, 220)
		);	
		
		PlaylistController.Instances = null;
		var pcs = PlaylistController.Instances;
		var plControllerInScene = pcs.Count > 0;

		Texture header = (Texture) Resources.LoadAssetAtPath(MasterAudioFolderPath + "/Sources/Textures/inspector_header_master_audio.png", typeof(Texture));
		if (header != null) {
			DTGUIHelper.ShowHeaderTexture(header);
		}
        Texture settings = (Texture)Resources.LoadAssetAtPath(MasterAudioFolderPath + "/Sources/Textures/gearIcon.png", typeof(Texture));
		
		MasterAudio.Instance = null;
		var ma = MasterAudio.Instance;
		
		DTGUIHelper.ShowColorWarning("The Master Audio prefab holds sound FX group and mixer controls. Add this first (only one per scene).");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		
		EditorGUILayout.LabelField("Master Audio prefab", GUILayout.Width(300));
		if (ma == null) {
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Create", "Create Master Audio prefab"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
				CreateMasterAudio();
			}
			GUI.contentColor = Color.white;
		} else { 
			if (settings != null) { 
				if (GUILayout.Button(new GUIContent(settings, "Master Audio Settings"), EditorStyles.toolbarButton)) {
					Selection.activeObject = ma.transform;
				}
			}
			EditorGUILayout.LabelField("Exists in scene", EditorStyles.boldLabel);
		}
		
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		// Playlist Controller
		DTGUIHelper.ShowColorWarning("The Playlist Controller prefab controls sets of songs (or other audio) and ducking. No limit per scene.");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		EditorGUILayout.LabelField("Playlist Controller prefab", GUILayout.Width(300));

		GUI.contentColor = Color.green;
		if (GUILayout.Button(new GUIContent("Create", "Place a Playlist Controller prefab in the current scene."), EditorStyles.toolbarButton, GUILayout.Width(100))) { 
			CreatePlaylistController();
		}
		GUI.contentColor = Color.white;

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		if (!plControllerInScene) {
			DTGUIHelper.ShowRedError("*There is no Playlist Controller in the scene. Music will not play.");
		}
		
		EditorGUILayout.Separator();
		// Dynamic Sound Group Creators
		DTGUIHelper.ShowColorWarning("The Dynamic Sound Group Creator prefab can create Sound Groups on the fly. No limit per scene.");
		EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
		EditorGUILayout.LabelField("Dynamic Sound Group Creator prefab", GUILayout.Width(300));

		GUI.contentColor = Color.green;
		if (GUILayout.Button(new GUIContent("Create", "Place a Dynamic Sound Group prefab in the current scene."), EditorStyles.toolbarButton, GUILayout.Width(100))) { 
			CreateDynamicSoundGroupCreator();
		}
		GUI.contentColor = Color.white;

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		
		if (!Application.isPlaying) {
			EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
			GUILayout.Label("Utility Functions");
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUI.contentColor = Color.green;
			if (GUILayout.Button(new GUIContent("Delete all unused Filter FX", "This will delete all unused Unity Audio Filter FX components in the entire MasterAudio prefab and all Sound Groups within."), EditorStyles.toolbarButton, GUILayout.Width(150))) {
				DeleteAllUnusedFilterFX();
			}
			GUI.contentColor = Color.white;
			EditorGUILayout.EndHorizontal();
		}
		
		GUI.EndScrollView();
		
		this.Repaint();
	}
	
	private void DeleteAllUnusedFilterFX() {
		var ma = MasterAudio.Instance;
		
		if (ma == null) {
			DTGUIHelper.ShowAlert("There is no MasterAudio prefab in this scene. Try pressing this button on a different Scene.");
			return;
		}
		
		var affectedVariations = new List<SoundGroupVariation>();
		var filtersToDelete = new List<Object>();
		
		for (var g = 0; g < ma.transform.childCount; g++) {
			var sGroup = ma.transform.GetChild(g);
			for (var v = 0; v < sGroup.childCount; v++) {
				var variation = sGroup.GetChild(v);
				var grpVar = variation.GetComponent<SoundGroupVariation>();
				if (grpVar == null) {
					continue;
				}
				
				if (grpVar.LowPassFilter != null && !grpVar.LowPassFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.LowPassFilter)) {
						filtersToDelete.Add(grpVar.LowPassFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}

				if (grpVar.HighPassFilter != null && !grpVar.HighPassFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.HighPassFilter)) {
						filtersToDelete.Add(grpVar.HighPassFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}

				if (grpVar.ChorusFilter != null && !grpVar.ChorusFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.ChorusFilter)) {
						filtersToDelete.Add(grpVar.ChorusFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}

				if (grpVar.DistortionFilter != null && !grpVar.DistortionFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.DistortionFilter)) {
						filtersToDelete.Add(grpVar.DistortionFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}

				if (grpVar.EchoFilter != null && !grpVar.EchoFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.EchoFilter)) {
						filtersToDelete.Add(grpVar.EchoFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}

				if (grpVar.ReverbFilter != null && !grpVar.ReverbFilter.enabled) {
					if (!filtersToDelete.Contains(grpVar.ReverbFilter)) {
						filtersToDelete.Add(grpVar.ReverbFilter);
					}

					if (!affectedVariations.Contains(grpVar)) {
						affectedVariations.Add(grpVar);
					}
				}
			}
		}
		
		UndoHelper.RecordObjectsForUndo(affectedVariations.ToArray(), "delete all unused Filter FX Components");
		
		for (var i = 0; i < filtersToDelete.Count; i++) {
			DestroyImmediate(filtersToDelete[i]);
		}
		
		DTGUIHelper.ShowAlert(string.Format("{0} Filter FX Components deleted.", filtersToDelete.Count));
	}
	
	private void CreateMasterAudio() {
        var ma = Resources.LoadAssetAtPath(MasterAudioFolderPath + "/Prefabs/MasterAudio.prefab", typeof(GameObject));
		if (ma == null) {
			Debug.LogError("Could not find MasterAudio prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}
		
		
        var go = PrefabUtility.InstantiatePrefab(ma) as GameObject;

        UndoHelper.CreateObjectForUndo(go, "Create Master Audio prefab");

		go.name = "MasterAudio";
	}
	
	private void CreatePlaylistController() {
        var pc = Resources.LoadAssetAtPath(MasterAudioFolderPath + "/Prefabs/PlaylistController.prefab", typeof(GameObject));
		if (pc == null) {
			Debug.LogError("Could not find PlaylistController prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}

        var go = PrefabUtility.InstantiatePrefab(pc) as GameObject;
		go.name = "PlaylistController";

        UndoHelper.CreateObjectForUndo(go, "Create Playlist Controller prefab");
	}
	
	private void CreateDynamicSoundGroupCreator() {
        var pc = Resources.LoadAssetAtPath(MasterAudioFolderPath + "/Prefabs/DynamicSoundGroupCreator.prefab", typeof(GameObject));
		if (pc == null) {
			Debug.LogError("Could not find DynamicSoundGroupCreator prefab. Please drag it into the scene yourself. It is located under MasterAudio/Prefabs.");
			return;
		}
		var go = PrefabUtility.InstantiatePrefab(pc) as GameObject;
		go.name = "DynamicSoundGroupCreator";

        UndoHelper.CreateObjectForUndo(go, "Create Dynamic Sound Group Creator prefab");
	}
}
