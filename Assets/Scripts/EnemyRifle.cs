using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyAI))]
public class EnemyRifle : MonoBehaviour {
	
	public int damage = 4;
	public float fireRate = 5;
	
	public Transform bulletTrailPrefab;
	public Transform hitParticlesPrefab;
	public Transform muzzleFlashPrefab;
	
	private float lastHit = 0f;
	
	private Transform target;
	private EnemyAI ai;
	private Transform firePoint;
	
	public LayerMask mask;
	
	void Start () {
		ai = GetComponent<EnemyAI>();
		firePoint = transform.FindChild ("FirePoint");
		if (firePoint == null)
			Debug.LogError ("No firePoint under enemy?!");
	}
	
	void Update () {
		if (target == null) {
			target = ai.target;
			return;
		}
		
		if (ai.inSight) {
			if (Time.time > lastHit + 1f/fireRate) {
				Hit ();
				lastHit = Time.time;
			}
		}
		
	}
	
	void Hit () {
		PlayerMaster.playerMaster.AdjustPlayerHealth (-damage);
		//Debug.Log (PlayerStats.playerHealth);
		
		//EFFECT
		
		//Direction to target
		Vector3 targetDir = (target.position-transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir, 100f, mask);
		Debug.DrawLine (transform.position, hit.point, Color.red,0.5f);
		
		Effect (hit);
	}
	
	void Effect (RaycastHit2D hit) {
		Transform trail = Instantiate (bulletTrailPrefab, firePoint.position, Quaternion.identity) as Transform;
		LineRenderer ln = trail.GetComponent<LineRenderer>();
		Vector3 endPoint = new Vector3 (hit.point.x, hit.point.y, firePoint.position.z);
		ln.useWorldSpace = true;
		ln.SetPosition (0, firePoint.position);
		ln.SetPosition (1, endPoint);
		Destroy (trail.gameObject, 0.02f);
		
		Transform sparks = Instantiate (hitParticlesPrefab, hit.point, Quaternion.LookRotation (hit.normal)) as Transform;
		Destroy (sparks.gameObject, 0.2f);
		
		MuzzleFlash ();
	}
	
	void MuzzleFlash () {
		Transform clone = Instantiate (muzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
		clone.parent = firePoint;
		float size = Random.Range (0.6f, 0.9f);
		clone.localScale = new Vector3 (size, size, size);
		Destroy (clone.gameObject, 0.02f);
	}
	
}
