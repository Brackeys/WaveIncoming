using UnityEngine;
using System.Collections;

public class MA_PlayerControl : MonoBehaviour {
	public GameObject ProjectilePrefab;
	public bool canShoot = true;
	
	private const float MOVE_SPEED = 10f;
	private Transform trans;
	private float lastMoveAmt = 0f;
	
	// Use this for initialization
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.StartsWith("Enemy(")) {
			#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
				this.gameObject.SetActiveRecursively(false); 
			#else
				this.gameObject.SetActive(false);
			#endif
		}
	}
	
	void OnBecameInvisible() {
	}
	
	// Update is called once per frame 
	void Update () {
		var moveAmt = Input.GetAxis("Horizontal") * MOVE_SPEED * Time.deltaTime;
		
		if (moveAmt != 0f) {
			if (lastMoveAmt == 0f) {
				MasterAudio.FireCustomEvent("PlayerMoved");
			}
		} else {
			if (lastMoveAmt != 0f) {
				MasterAudio.FireCustomEvent("PlayerStoppedMoving");
			}
		}
		
		lastMoveAmt = moveAmt;
		
		var pos = this.trans.position;
		pos.x += moveAmt;
		this.trans.position = pos;
		
		if (canShoot && Input.GetMouseButtonDown(0)) {
			var spawnPos = this.trans.position;
			spawnPos.y += 1;
			
			Instantiate(ProjectilePrefab, spawnPos, ProjectilePrefab.transform.rotation);
		}
	}
}
