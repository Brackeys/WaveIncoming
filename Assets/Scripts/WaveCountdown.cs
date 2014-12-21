using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class WaveCountdown : MonoBehaviour {

	private Text countdown;

	void Start () {
		countdown = GetComponent<Text>();
	}

	void Update () {
		countdown.text = ( Mathf.Round (WaveController.timeUntilWave) ).ToString();
	}
	
}
