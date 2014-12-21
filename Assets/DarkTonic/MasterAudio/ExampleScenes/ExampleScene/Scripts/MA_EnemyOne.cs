using UnityEngine;
using System.Collections;

public class MA_EnemyOne : MonoBehaviour {
	public GameObject ExplosionParticlePrefab;
	private Transform trans;
	private float speed;
	private float horizSpeed;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
		this.speed = Random.Range(-3, -8) * Time.deltaTime;
		this.horizSpeed = Random.Range(-3, 3) * Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision collision) {
		Instantiate(ExplosionParticlePrefab, this.trans.position, Quaternion.identity);
	}
	
	
	// Update is called once per frame
	void Update () {
		var pos = this.trans.position;
		pos.x += this.horizSpeed;
		pos.y += speed;
		this.trans.position = pos;
		
		this.trans.Rotate(Vector3.down * 300 * Time.deltaTime);
		
		if (this.trans.position.y < -5) {
			//this.gameObject.SetActiveRecursively(false);
			Destroy (this.gameObject);
		}
	}
}
