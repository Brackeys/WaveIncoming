using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public static bool PauseMenuEnabled = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (PauseMenuEnabled == true) {
				PauseMenuEnabled = false;
				transform.FindChild ("PauseMenu").gameObject.SetActive (false);
				Time.timeScale = 1f;
			} else {
				PauseMenuEnabled = true;
				transform.FindChild ("PauseMenu").gameObject.SetActive (true);
				Time.timeScale = 0f;
			}
		}
	}
	
	public void Continue () {
		PauseMenuEnabled = false;
		transform.FindChild ("PauseMenu").gameObject.SetActive (false);
		Time.timeScale = 1f;
	}
	
	public void Quit () {
		Application.Quit();
	}
	
	public void AdjustVolume (float vol) {
		MasterAudio.MasterVolumeLevel = vol;
	}
}
