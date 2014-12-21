using UnityEngine;
using System.Collections;

public class EnemyMaster : MonoBehaviour {

	public int health = 30;
	public Transform deathParticles;
	public int moneyReward = 30;
	
	public void AdjustHealth (int adj) {
		health += adj;
		
		CombatText.SpawnCombatText (transform.position, adj, CombatText.CombatTextType.damage);
		
		if (health <= 0) {
			Die ();
		}
	}
	
	public void Die () {
		Transform clone = Instantiate (deathParticles, transform.position, Quaternion.identity) as Transform;
		
		PlayerStats.AdjustMoney (moneyReward);
		
		Destroy (clone.gameObject, 20f);
		Destroy (gameObject);
	}
}
