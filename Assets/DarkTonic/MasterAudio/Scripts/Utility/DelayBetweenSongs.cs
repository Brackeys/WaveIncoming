using UnityEngine;
using System.Collections;

public class DelayBetweenSongs : MonoBehaviour {
	public float minTimeToWait = 1f;
	public float maxTimeToWait = 2f;
	public string playlistControllerName = "PlaylistControllerBass";

	private PlaylistController controller;
	
	void Start() {
		controller = PlaylistController.InstanceByName(playlistControllerName);
		controller.SongEnded += SongEnded;
	}
	
	private void SongEnded(string songName) {
		StopAllCoroutines(); // just in case we are still waiting from calling this before. Don't want multiple coroutines running!
		StartCoroutine(PlaySongWithDelay());	
	}
	
	private IEnumerator PlaySongWithDelay() {
		var randomTime = Random.Range(minTimeToWait, maxTimeToWait);
		yield return new WaitForSeconds(randomTime);
		
		controller.PlayNextSong();
	}
}
