using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class WaveCountdown : MonoBehaviour {

	private Text countdown;
	private Color startColor;

	void Start () {
		countdown = GetComponent<Text>();
		startColor = countdown.color;
	}

	void Update () {
		if (WaveController.waveCountdown == 0) {
			Color c = countdown.color;
			c.a -= 1f * Time.deltaTime;
			countdown.color = c;
		} else {
			countdown.color = startColor;
		}

		countdown.text = ( Mathf.Round (WaveController.waveCountdown) ).ToString();
	}
	
}
