//-----------------------------------------------------------------
// This script is not at all optimized and is very dependent on all components
// being on the right objects... USE WITH CAUTION!
//-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QualitySettingsMenu : MonoBehaviour {

	public Transform resElement;
	public Transform resList;

	private Resolution[] resolutions;
	public List<Toggle> resToggles;
	private int qualityLevel = 1;
	private bool fullscreen = true;

	// Use this for initialization
	void Start () {
		Screen.SetResolution ( 600, 400, false );

		resolutions = Screen.resolutions;
		resToggles = new List<Toggle>();

		bool firstIteration = true;
		foreach (Resolution res in resolutions) {
			if (res.width > 1000 && res.height > 600) {
				Transform re = (Transform)Instantiate (resElement);
				re.SetParent (resList);
				re.GetComponentInChildren <Text>().text = res.width.ToString() + " x " + res.height.ToString();
				Toggle toggle = re.GetComponentInChildren<Toggle>();
				toggle.group = resList.GetComponent<ToggleGroup>();
			
				if (firstIteration) {
					toggle.isOn = true;
					firstIteration = false;
				} else {
					toggle.isOn = false;
				}

				re.GetComponent<ResolutionElement>().res = res;
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

		foreach (Transform child in resList) {
			if (child.GetComponentInChildren<Toggle>().isOn) {
				Resolution res = child.GetComponent<ResolutionElement>().res;
				Screen.SetResolution ( res.width, res.height, fullscreen );
			}
		}

		QualitySettings.SetQualityLevel (qualityLevel);

		Application.LoadLevel (Application.loadedLevel + 1);
	}
}