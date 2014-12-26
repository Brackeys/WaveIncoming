using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public static bool PauseMenuEnabled = false;

	public GameObject pauseMenu;
	public GameObject warningDialogue;

	private bool leaveGame = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (PauseMenuEnabled == true) {
				PauseMenuEnabled = false;
				pauseMenu.SetActive (false);
				Time.timeScale = 1f;
			} else {
				PauseMenuEnabled = true;
				pauseMenu.SetActive (true);
				Time.timeScale = 0f;
			}
		}
	}
	
	public void Continue () {
		PauseMenuEnabled = false;
		transform.FindChild ("PauseMenu").gameObject.SetActive (false);
		Time.timeScale = 1f;
	}

	public void ToggleWarningDialogue (bool isOn) {
		warningDialogue.SetActive (isOn);
	}

	public void SetLeaveGame (bool isTrue) {
		leaveGame = isTrue;
	}

	public void LeaveGame () {
		if (leaveGame)
			_Quit();
		else
			StartCoroutine ( _OpenMainMenu() );
	}
	
	private void _Quit () {
		Time.timeScale = 1f;
		Application.Quit();
	}

	private IEnumerator _OpenMainMenu () {
		Time.timeScale = 1f;

		float waitTime = Fade.FadeInstance.BeginFade(1);
		yield return new WaitForSeconds (waitTime);
		
		Application.LoadLevel ("MainMenu");
	}
	
	public void AdjustVolume (float vol) {
		Debug.Log ("TODO: CHANGE GLOBAL VOLUME");
	}
}
