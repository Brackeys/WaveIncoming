using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AudioResourceOptimizer {
	private static Dictionary<string, List<AudioSource>> audioResourceTargetsByName = new Dictionary<string, List<AudioSource>>();
	private static Dictionary<string, AudioClip> audioClipsByName = new Dictionary<string, AudioClip>();

    public static void ClearAudioClips()
    {
        audioClipsByName.Clear();
        audioResourceTargetsByName.Clear();
    }

	public static void AddTargetForClip(string clipName, AudioSource source) {
		if (!audioResourceTargetsByName.ContainsKey(clipName)) {
			audioResourceTargetsByName.Add(clipName, new List<AudioSource>() {
				source
			});
		} else {
			var sources = audioResourceTargetsByName[clipName];
			sources.Add(source);
		}
	}
	
	
	public static AudioClip PopulateResourceSongToPlaylistController(string songResourceName, string playlistName) {
		var	resAudioClip = Resources.Load(songResourceName) as AudioClip;

		if (resAudioClip == null) {
			MasterAudio.LogWarning("Resource file '" + songResourceName + "' could not be located from Playlist '" + playlistName + "'.");
			return null;
		}
		
		return resAudioClip;
	}
	
	public static void UnloadPlaylistSong(AudioClip clipToRemove) {
		if (clipToRemove == null) {
			return; // no need
		}
		Resources.UnloadAsset(clipToRemove);
	}
	
	public static void PopulateSourcesWithResourceClip(string clipName) {
		if (audioClipsByName.ContainsKey(clipName)) {
			//Debug.Log("clip already exists: " + clipName);
			return; // work is done already!
		}
		
		var	resAudioClip = Resources.Load(clipName) as AudioClip;

		if (resAudioClip == null) {
			MasterAudio.LogWarning("Resource file '" + clipName + "' could not be located.");
			return;
		}
		
		if (!audioResourceTargetsByName.ContainsKey(clipName)) {
			Debug.LogError("No Audio Sources found to add Resource file '" + clipName + "'.");
			return;
		}  else { 
			var sources = audioResourceTargetsByName[clipName];

			for (var i = 0; i < sources.Count; i++) {
				sources[i].clip = resAudioClip;
			}
		}
		
		audioClipsByName.Add(clipName, resAudioClip);
	}
	
	public static void DeleteAudioSourceFromList(string clipName, AudioSource source) {
		if (!audioResourceTargetsByName.ContainsKey(clipName)) {
			Debug.Log("No Audio Sources found for Resource file '" + clipName + "'.");
			return;
		}		
		
		var sources = audioResourceTargetsByName[clipName];
		sources.Remove(source);
		
		if (sources.Count == 0) {
			audioResourceTargetsByName.Remove(clipName);
		}
	}
	 
	public static void UnloadClipIfUnused(string clipName) {
		if (!audioClipsByName.ContainsKey(clipName)) {
			// already removed.
			return;
		}
		
		var sources = new List<AudioSource>();
		
		if (audioResourceTargetsByName.ContainsKey(clipName)) {
			sources = audioResourceTargetsByName[clipName];
			
			AudioSource aSource = null;
			
			for (var i = 0; i < sources.Count; i++) {
				aSource = sources[i]; 
				if (aSource.time > 0) {
					return; // still something playing
				}
			}			
		}
		
		var clipToRemove = audioClipsByName[clipName];

		for (var i = 0; i < sources.Count; i++) {
			sources[i].clip = null;
		}			

		audioClipsByName.Remove(clipName);
		Resources.UnloadAsset(clipToRemove);
	}
}
