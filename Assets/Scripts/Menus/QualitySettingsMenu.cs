using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsMenu : MonoBehaviour {

	public Transform resElement;
	public Transform resList;

	private Resolution[] resolutions;
	private int qualityLevel = 1;
	private bool fullscreen = true;

	// Use this for initialization
	void Start () {
		resolutions = Screen.resolutions;

		bool firstIteration = true;
		foreach (Resolution res in resolutions) {
			if (res.width > 1000 && res.height > 600) {
				Transform re = (Transform)Instantiate (resElement);
				re.SetParent (resList);
				re.GetComponentInChildren <Text>().text = res.width.ToString() + " x " + res.height.ToString();
				re.GetComponentInChildren<Toggle>().group = resList.GetComponent<ToggleGroup>();
			
				if (firstIteration) {
					re.GetComponentInChildren<Toggle>().isOn = true;
					firstIteration = false;
				} else {
					re.GetComponentInChildren<Toggle>().isOn = false;
				}
			}
		}

	}

	public void SetQualityLevel (int level) {
		qualityLevel = level;
	}

	public void SetFullScreen (bool isOn) {
		fullscreen = isOn;
	}
	
	public void ApplySettings () {
	
		Screen.SetResolution ( resolutions[0].width, resolutions[0].height, fullscreen );

		QualitySettings.SetQualityLevel (qualityLevel);

		Application.LoadLevel (Application.loadedLevel + 1);
	}
}