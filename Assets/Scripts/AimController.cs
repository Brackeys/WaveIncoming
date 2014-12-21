using UnityEngine;
using System.Collections;

public class AimController : MonoBehaviour {

	public float speed = 1f;

	Camera cam;

	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		if (cam == null)
			Debug.LogError ("No camera found.");
	}
	
	// Update is called once per frame
	void Update () {
		if (PauseMenu.PauseMenuEnabled)
			return;
	
		Vector3 pos = cam.WorldToScreenPoint(transform.position);
		Vector3 dir = Input.mousePosition - pos;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
	}
}
