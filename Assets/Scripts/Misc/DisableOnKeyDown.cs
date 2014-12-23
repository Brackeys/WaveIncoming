using UnityEngine;
using System.Collections;

public class DisableOnKeyDown : MonoBehaviour {

	public KeyCode key;

	void Update () {
		if (Input.GetKeyDown (key)) {
			gameObject.SetActive (false);
		}
	}
}
