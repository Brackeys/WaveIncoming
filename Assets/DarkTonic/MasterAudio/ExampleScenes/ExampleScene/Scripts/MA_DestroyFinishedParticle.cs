using UnityEngine;
using System.Collections;

public class MA_DestroyFinishedParticle : MonoBehaviour {
	private ParticleSystem particles;
	
	// Update is called once per frame
	void Awake() {
		this.useGUILayout = false;
		this.particles = this.GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if (!this.particles.IsAlive()) {
			Destroy (this.gameObject);
		}
	}
}
