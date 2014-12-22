using UnityEngine;
using System.Collections;

public class QualitySettingsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Resolution[] resolutions = Screen.resolutions;
		foreach (Resolution res in resolutions) {
			print(res.width + "x" + res.height);
		}
		Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
