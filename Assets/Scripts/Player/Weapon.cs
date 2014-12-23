using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	public int fireRate = 0;
	public int damage = 10;
	public int critChance = 1;
	public int clipSize = 1;
	public float reloadTime = 1f;
	
	private bool reloading = false;
	
	public static int shotsFired = 0;
	
	public float knockBack = 200f;
	public float enemyKnockBack = 800f;
	public ForceMode2D fMode;
	
	public LayerMask whatToHit;
	
	public Transform BulletTrailPrefab;
	public Transform MuzzleFlashPrefab;
	public Transform HitParticlesPrefab;
	float timeToSpawnEffect = 0;
	public float effectSpawnRate = 10;
	
	float timeToFire = 0;
	Transform firePoint;
	Rigidbody2D rb;
	ParticleSystem ps;
	
	// Use this for initialization
	void Awake () {
		firePoint = transform.FindChild ("FirePoint");
		if (firePoint == null) {
			Debug.LogError ("No firePoint? WHAT?!");
		}
		
		rb = transform.parent.GetComponent<Rigidbody2D>();
		if (rb == null)
			Debug.LogError ("No rigidbody in parent?!");
			
		ps = GetComponentInChildren<ParticleSystem>();
		if (ps == null)
			Debug.LogError ("No ParticleSystem in children?!");
	}
	
	void Start () {
		damage = PlayerStats.playerDamage;
		fireRate = PlayerStats.playerFireRate;
		critChance = PlayerStats.playerCritChance;
		clipSize = PlayerStats.playerClipSize;
		reloadTime = PlayerStats.playerReloadTime;
		
		shotsFired = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Customization.CustomizationMenuEnabled) {
			damage = PlayerStats.playerDamage;
			fireRate = PlayerStats.playerFireRate;
			critChance = PlayerStats.playerCritChance;
			clipSize = PlayerStats.playerClipSize;
			reloadTime = PlayerStats.playerReloadTime;
			
			return;
		}
		
		if (PauseMenu.PauseMenuEnabled)
			return;
	
		if (Input.GetKeyDown (KeyCode.R)) {
			StartCoroutine ("Reload");
			reloading = true;
		}
		
		if (reloading)
			return;
	
		if (shotsFired >= clipSize) {
			StartCoroutine ("Reload");
			ps.emissionRate = 0;
			reloading = true;
			return;
		}
	
		if (fireRate == 0) {
			PlayerController.PlayerAnim.SetBool ("isShooting", false);
			ps.emissionRate = 0;
			if (Input.GetButtonDown ("Fire1")) {
				Shoot();
				shotsFired++;
				ps.emissionRate = 1;
				PlayerController.PlayerAnim.SetBool ("isShooting", true);
			}
		}
		else {
			ps.emissionRate = 0;
			PlayerController.PlayerAnim.SetBool ("isShooting", false);
			if (Input.GetButton ("Fire1")) {
				if (Time.time > timeToFire) {
					timeToFire = Time.time + 1/(float)fireRate;
					Shoot();
					shotsFired++;
				}
				
				ps.emissionRate = fireRate;
				PlayerController.PlayerAnim.SetBool ("isShooting", true);
			}
		}
	}
	
	void Shoot () {
		Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
		
		Vector2 shotDir = new Vector2 ();
		
		if (Vector2.Distance (mousePosition, firePointPosition) > 0.5f)
			shotDir = mousePosition - firePointPosition;
		else
			shotDir = firePoint.parent.up;
		
		shotDir.Normalize();
		
		shotDir.x += Random.Range (-PlayerStats.playerAimOffset, PlayerStats.playerAimOffset);
		shotDir.y += Random.Range (-PlayerStats.playerAimOffset, PlayerStats.playerAimOffset);
		
		shotDir.Normalize();
		
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, shotDir, 100, whatToHit);
		if (Time.time >= timeToSpawnEffect) {
			if (hit.collider != null)
				Effect (hit);
			else
				Effect (new Vector3 (shotDir.x, shotDir.y, 0));
			timeToSpawnEffect = Time.time + 1/effectSpawnRate;
		}
		Debug.DrawLine (firePointPosition, shotDir*100, Color.cyan);
		if (hit.collider != null) {
			Debug.DrawLine (firePointPosition, hit.point, Color.red);
			//Debug.Log ("We hit " + hit.collider.name + " and did " + damage + " damage.");
			
			if (hit.collider.tag == "Enemy") {
				EnemyMaster em = hit.collider.GetComponent<EnemyMaster>();
				em.AdjustHealth (DoDamageCalculation());
				hit.collider.GetComponent<Rigidbody2D>().AddForce (transform.up * enemyKnockBack, ForceMode2D.Impulse);
			}
		}
		
		rb.AddRelativeForce (-transform.up * knockBack, fMode);
	}
	
	int DoDamageCalculation () {
	
		float dmg = PlayerStats.playerDamage;
	
		float chance = Random.Range (0.1f, -0.1f) * (float)PlayerStats.playerDamage;
		dmg += chance;
		
		int crit = Random.Range (0, 101);
		if (crit <= PlayerStats.playerCritChance)
			dmg *= Random.Range (2f, 3f);
	
		return -(int)dmg;
	}
	
	IEnumerator Reload () {
		Debug.Log ("TODO: Insert reload sound.");
		yield return new WaitForSeconds (reloadTime);
		shotsFired = 0;
		reloading = false;
	}
	
	void Effect (RaycastHit2D hit) {
		Transform trail = Instantiate (BulletTrailPrefab, firePoint.position, Quaternion.identity) as Transform;
		LineRenderer ln = trail.GetComponent<LineRenderer>();
		Vector3 endPoint = new Vector3 (hit.point.x, hit.point.y, firePoint.position.z);
		ln.useWorldSpace = true;
		ln.SetPosition (0, firePoint.position);
		ln.SetPosition (1, endPoint);
		Destroy (trail.gameObject, 0.02f);
		
		Transform sparks = Instantiate (HitParticlesPrefab, hit.point, Quaternion.LookRotation (hit.normal)) as Transform;
		Destroy (sparks.gameObject, 0.2f);
		
		MuzzleFlash ();
	}
	
	void Effect (Vector3 shotDir) {
		float trailAngle = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg;
		Quaternion trailRot = Quaternion.AngleAxis(trailAngle, Vector3.forward);
	
		Transform trail = Instantiate (BulletTrailPrefab, firePoint.position, trailRot) as Transform;
		
		Destroy (trail.gameObject, 0.02f);
		
		MuzzleFlash();
	}
	
	void MuzzleFlash () {
		Transform clone = Instantiate (MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
		clone.parent = firePoint;
		float size = Random.Range (0.6f, 0.9f);
		clone.localScale = new Vector3 (size, size, size);
		Destroy (clone.gameObject, 0.02f);
	}
}
