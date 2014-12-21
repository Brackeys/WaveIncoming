using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour {
	//The point to move to
	Vector3 targetPosition;

	public Transform target;

	public float updateDelay = 1f;

	private Seeker seeker;
	private Rigidbody2D rb;

	//The calculated path
	public Path path;

	//The AI's speed per second
	public float speed = 100;
	public ForceMode2D fMode;
	public bool useVelocity = false;

	private bool pathIsEnded = false;

	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;

	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	public float stopDist = 2f;
	public LayerMask mask;
	
	public bool inSight = false;
	
	private Animator anim;

	public void Start () {
		anim = GetComponent<Animator>();
	
		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();

		if (target == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			if (go != null)
				target = go.transform;
		}

		if (target == null)
			return;
		targetPosition = target.position;

		//Start a new path to the targetPosition, return the result to the OnPathComplete function
		seeker.StartPath (transform.position,targetPosition, OnPathComplete);

		StartCoroutine ("UpdatePath");
	}

	IEnumerator UpdatePath () {
		if (target == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			if (go != null)
				target = go.transform;
			return false;
		}
		targetPosition = target.position;

		//Start a new path to the targetPosition, return the result to the OnPathComplete function
		seeker.StartPath (transform.position,targetPosition, OnPathComplete);

		yield return new WaitForSeconds (updateDelay);
		StartCoroutine ("UpdatePath");
	}

	public void OnPathComplete (Path p) {
		//Debug.Log ("Yay, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}

	public void FixedUpdate () {
		if (target == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Player");
			if (go != null)
				target = go.transform;
			return;
		}
		targetPosition = target.position;
		
		if (anim != null)
			anim.SetBool ("Walk", false);
	
		//Always look at the player
		Vector3 pos = target.position;
		Vector3 lookDir = pos - transform.position;
		float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

		if (path == null) {
			//We have no path to move after yet
			return;
		}

		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded)
				return;

			Debug.Log ("End Of Path Reached");
			if (useVelocity)
				rb.velocity = Vector2.zero;

			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		inSight = false;

		if (stopDist > Vector3.Distance (transform.position, target.position))
		{
			//Direction to target
			Vector3 targetDir = (target.position-transform.position).normalized;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir, stopDist + 2f, mask);
			
			Debug.DrawLine (transform.position, hit.point);
			
			if (hit.collider.tag == "Player") {
				inSight = true;
				return;
			}
		}

		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		if (useVelocity)
			rb.velocity = dir;
		else {
			rb.AddForce (dir, fMode);
		}
		
		if (anim != null)
			anim.SetBool ("Walk", true);

		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, stopDist);
	}
}