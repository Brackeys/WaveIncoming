using UnityEngine;
using System.Collections;

public class MA_Laser : MonoBehaviour {
	private Transform trans;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.StartsWith("Enemy(")) {
			Destroy (collision.gameObject);
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		var moveAmt = 10f * Time.deltaTime;
		
		var pos = this.trans.position;
		pos.y += moveAmt;
		this.trans.position = pos;
		
		if (this.trans.position.y > 7) {
			Destroy (this.gameObject);
		}
	}
}
