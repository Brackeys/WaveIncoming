using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MA_GameScene : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(20,40, 640, 190), "This is the Game Scene. We have used a Dynamic Sound Group Creator prefab to populate temporary Sound Groups into Master Audio as soon as that prefab becomes enabled (on Scene change for us). If we were to load a different Scene now, those sounds would vanish from the mixer.");
	}
}
		
