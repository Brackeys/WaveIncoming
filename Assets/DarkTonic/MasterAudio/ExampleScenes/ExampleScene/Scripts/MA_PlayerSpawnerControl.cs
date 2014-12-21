using UnityEngine;
using System.Collections;

public class MA_PlayerSpawnerControl : MonoBehaviour {
	public GameObject Player;
	
	private float nextSpawnTime;
	
	void Awake() {
		this.useGUILayout = false;
		this.nextSpawnTime = -1f;
	}
	
	private bool PlayerActive {
		get {
			#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
				return Player.active;
			#else
				return Player.activeInHierarchy;
			#endif
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!PlayerActive) {
			if (nextSpawnTime < 0) {
				nextSpawnTime = Time.time + 1;
			}
			
			if (Time.time >= this.nextSpawnTime) {
				#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					Player.SetActiveRecursively(true);
				#else
					Player.SetActive(true);
				#endif
				
				nextSpawnTime = -1;
			}
		}
	}
}
