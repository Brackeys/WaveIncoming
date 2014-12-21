using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class LightFlicker : MonoBehaviour {

	float random;
	
	public float min = 1.2f;
	public float max = 2f;
	
	void Start()
	{
		random = Random.Range(0.0f, 65535.0f);
	}
	
	void Update()
	{
		float noise = Mathf.PerlinNoise(random, Time.time * 5f);
		light.intensity = Mathf.Lerp(min, max, noise);
	}
}
