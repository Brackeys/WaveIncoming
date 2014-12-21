using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MA_Bootstrapper : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(20,40, 640, 190), "This is the Bootstrapper Scene. Set it up in BuildSettings as the first Scene. Then add 'Game Scene' as the second Scene. Hit play. Master Audio is configured in 'persist between Scenes' mode. "
			+ "Finally, click 'Load Game Scene' button and notice how the music doesn't get interruped even though we're changing Scenes. Normally a Bootstrapper Scene would not be seen. We are illustrating how to set up though. Notice that no Sound Groups are set up in Master Audio.");
		
		if (GUI.Button(new Rect(100, 150, 150, 100), "Load Game Scene")) {
			Application.LoadLevel(1);
		}
	}
}
		
