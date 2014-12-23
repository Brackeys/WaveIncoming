using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

	public float speed = 1;
	public ForceMode2D fMode;
	
	Vector2 moveDirection = new Vector2();
	
	public static Animator PlayerAnim;
	
	void Start () {
		speed = PlayerStats.playerSpeed;
		
		PlayerStats.PlayerInstance = this.gameObject;
	
		PlayerAnim = GetComponentInChildren<Animator>();
		if (PlayerAnim == null)
			Debug.LogError ("No player animator?!");
	}

	void Update () {
		
		if (Customization.CustomizationMenuEnabled) {
			speed = PlayerStats.playerSpeed;
			return;
		}
	
		moveDirection = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		moveDirection *= speed;
		//Debug.Log (moveDirection);
		
		PlayerAnim.SetFloat ("speed", Mathf.Abs (moveDirection.x + moveDirection.y));
		
		PlayerStats.PlayerPosition = transform.position;
	}
	
	void FixedUpdate () {
		rigidbody2D.AddForce (moveDirection, fMode);
	}
}
