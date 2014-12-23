using UnityEngine;
using System.Collections;

public class PostProcessingSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int qualityLevel = QualitySettings.GetQualityLevel();

		switch (qualityLevel) {
		case 0:
			// Do nothing
			break;
		case 1:
			// Turn on Contrast stretch and vignetting
			MonoBehaviour vig = (MonoBehaviour)GetComponent("Vignetting");
			if (vig != null)
				vig.enabled = true;

			MonoBehaviour cs = (MonoBehaviour)GetComponent("ContrastStretchEffect");
			if (cs != null)
				cs.enabled = true;

			break;
		case 2:
			// Turn on everything
			MonoBehaviour vignette = (MonoBehaviour)GetComponent("Vignetting");
			if (vignette != null)
				vignette.enabled = true;
			
			MonoBehaviour contrast = (MonoBehaviour)GetComponent("ContrastStretchEffect");
			if (contrast != null)
				contrast.enabled = true;

			MonoBehaviour noise = (MonoBehaviour)GetComponent("NoiseAndGrain");
			if (noise != null)
				noise.enabled = true;

			MonoBehaviour bloom = (MonoBehaviour)GetComponent("Bloom");
			if (bloom != null)
				bloom.enabled = true;

			break;
		}
	}

}
