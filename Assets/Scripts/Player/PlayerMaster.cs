using UnityEngine;
using System.Collections;

public class PlayerMaster : MonoBehaviour {

	public Transform AssaultPlayer;
	public Transform TankPlayer;
	public Transform SniperPlayer;
	
	public Transform spawnPoint;

	public float healthRegenRate = 1f;
	private float lastRegen = 0 ;
	
	public Transform deathParticle;
	
	public static PlayerMaster playerMaster;
	
	void Awake () {
		if (PlayerStats.SelectedClass == "Assault")
			Instantiate (AssaultPlayer, spawnPoint.position, spawnPoint.rotation);
		else if (PlayerStats.SelectedClass == "Tank")
			Instantiate (TankPlayer, spawnPoint.position, spawnPoint.rotation);
		else
			Instantiate (SniperPlayer, spawnPoint.position, spawnPoint.rotation);
	}
	
	void Start () {
		playerMaster = this;
	}
	
	void Update () {
		if (Time.time > lastRegen + 1f/healthRegenRate && PlayerStats.playerHealth < PlayerStats.playerVitality) {
			PlayerStats.playerHealth += 5;
			CombatText.SpawnCombatText (PlayerStats.PlayerPosition, 5, CombatText.CombatTextType.heal);
			lastRegen = Time.time;
		}
	}
	
	public void AdjustPlayerHealth (int adj) {
		PlayerStats.playerHealth += adj;
		
		CombatText.SpawnCombatText (PlayerStats.PlayerPosition, adj, CombatText.CombatTextType.damage);
		
		if (PlayerStats.playerHealth <= 0) {
			StartCoroutine ("KillPlayer");
		}
	}
	
	public IEnumerator KillPlayer () {
		Destroy (PlayerStats.PlayerInstance);
		Instantiate (deathParticle, PlayerStats.PlayerPosition, Quaternion.identity);
		
		yield return new WaitForSeconds (1f);
		
		float waitTime = Fade.FadeInstance.BeginFade(1);
		yield return new WaitForSeconds (waitTime);
		
		Application.LoadLevel ("MainMenu");
	}
}
