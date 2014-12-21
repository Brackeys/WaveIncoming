using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {

	[System.Serializable]
	public class Wave {
		public string name;
		public Transform enemy;
		public int count;
		public float spawnRate;
	}

	public float timeBetweenWaves = 20f;
	
	public Animator textAnim;
	
	private GameObject[] SpawnPoints;
	
	public Wave[] Waves;
	
	public static float timeUntilWave = 5f;
	private float lastSecond = 0f;
	
	public bool waveAlive = false;
	public int nextWave = 0;
	
	private int multiplier = 0;
	
	private bool doOnce = true;
	
	void Start () {
		SpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawnPoint");
		if (SpawnPoints.Length == 0)
			Debug.LogError ("No enemy spawn points?!");
	}
	
	void Update () {
		if (GameObject.FindGameObjectWithTag ("Enemy") == null) {
			if (doOnce) {
				timeUntilWave = timeBetweenWaves;
				doOnce = false;
			}
			waveAlive = false;
		}
	
		if (!waveAlive) {
			if (Time.time > lastSecond + 1f) {
				timeUntilWave -= 1;
				
				lastSecond = Time.time;
			}
			
			if (timeUntilWave <= 0 && waveAlive != true) {
				if (nextWave == 0)
				{
					multiplier += 1;
				}
			
				StartCoroutine ( SpawnWave (Waves[nextWave]) );
				
				if (nextWave < Waves.Length - 1)
					nextWave++;
				else {
					nextWave = 0;
				}
			}
		}
	}
	
	public IEnumerator SpawnWave (Wave wave) {
		waveAlive = true;
		doOnce = true;
	
		textAnim.SetTrigger ("Go");
	
		int spawnIndex = Random.Range (0, SpawnPoints.Length);
		int j = 0;
		for (int i = 0; i < wave.count; i++) {
			Transform enemy = Instantiate (wave.enemy, SpawnPoints[spawnIndex].transform.position, SpawnPoints[spawnIndex].transform.rotation) as Transform;
			MultiplyStats (enemy);
			yield return new WaitForSeconds (1f/wave.spawnRate);
			
			if (j == 1) {
				j = 0;
				spawnIndex = Random.Range (0, SpawnPoints.Length);
			}
			else {
				j++;
			}
		}
	}
	
	public void MultiplyStats (Transform enemy) {
		enemy.GetComponent<EnemyAI>().speed *= multiplier/3f;
		EnemyMaster em = enemy.GetComponent<EnemyMaster>();
		em.health *= multiplier;
		em.moneyReward *= multiplier;
		EnemyMelee melee = enemy.GetComponent<EnemyMelee>();
		if (melee != null) {
			melee.damage *= (int)(multiplier * 2f);
			melee.hitRate *= (int)(multiplier * 2f);
		}
	}
}
