using UnityEngine;
using System.Collections;

public class MoveTrail : MonoBehaviour {
	
	public int moveSpeed = 230;
	
	[HideInInspector]
	public Vector3 targetPos;	
	private float lastDist = 0;
	

	// Update is called once per frame
	void Update () {
		if (targetPos == Vector3.zero) {
			return;
		}
		else if (lastDist == 0) {
			lastDist = Vector3.Distance (transform.position, targetPos);
		}

		Vector3 newPos = Vector3.up * Time.deltaTime * moveSpeed - transform.position;

		if (Vector3.Distance (newPos, targetPos) > lastDist) {
			Destroy (gameObject);
		} else
			lastDist = Vector3.Distance (transform.position, targetPos);

		transform.Translate (Vector3.up * Time.deltaTime * moveSpeed);
		Destroy (gameObject, 1);
	}
	
	void OnDrawGizmos () {
		if (targetPos == Vector3.zero)
			return;
			
		Gizmos.DrawSphere (targetPos, 0.1f);
	}
}
