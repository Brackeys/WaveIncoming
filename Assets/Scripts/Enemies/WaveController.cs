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

	public float timeBetweenWaves = 5f;
	
	private GameObject[] SpawnPoints;
	public Wave[] Waves;
	
	public Animator textAnim;
	public string animTriggerName = "Go";
	
	public bool enemiesAlive = false;
	public bool spawning = false;
	private static float _waveCountdown = 0f;
	public static float waveCountdown {
		get { return _waveCountdown; }
		set { _waveCountdown = Mathf.Clamp (value, 0, Mathf.Infinity); }
	}
	
	public static int waveNumber = 1;
	
	int waveIndex = 0;
	float multiplier = 1f;
	
	void Start () {
		waveNumber = 1;
		waveCountdown = 5;

		SpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawnPoint");
		if (SpawnPoints.Length == 0)
			Debug.LogError ("No enemy spawn points?!");
		
		InvokeRepeating ("WaveTracker", 0f, 1f);
	}
	
	void WaveTracker () {
		if (waveCountdown == 0 && !spawning) {
			if (GameObject.FindGameObjectsWithTag ("Enemy").Length == 0) {
				enemiesAlive = false;
				waveCountdown = timeBetweenWaves;
			}	
		}
	}
	
	void Update () {
		waveCountdown -= Time.deltaTime*2f;
		
		if (waveCountdown == 0 && enemiesAlive == false) {
			StartCoroutine ( SpawnWave ( Waves[waveIndex] ) );
			return;
		}
	
//		if (GameObject.FindGameObjectWithTag ("Enemy") == null) {
//			if (doOnce) {
//				timeUntilWave = timeBetweenWaves;
//				doOnce = false;
//			}
//			waveAlive = false;
//		}
//	
//		if (!waveAlive) {
//			if (Time.time > lastSecond + 1f) {
//				timeUntilWave -= 1;
//				
//				lastSecond = Time.time;
//			}
//			
//			if (timeUntilWave <= 0 && waveAlive != true) {
//				if (nextWave == 0)
//				{
//					multiplier += 1;
//				}
//			
//				StartCoroutine ( SpawnWave (Waves[nextWave]) );
//				
//				if (nextWave < Waves.Length - 1)
//					nextWave++;
//				else {
//					nextWave = 0;
//				}
//			}
//		}
	}
	
	public IEnumerator SpawnWave (Wave wave) {
	
		enemiesAlive = true;
		spawning = true;
		
		if (textAnim != null)
			textAnim.SetTrigger (animTriggerName);
		
		yield return new WaitForSeconds (2f);
	
		for (int i = 0; i < wave.count; i++) {
			int spawnIndex = Random.Range (0, SpawnPoints.Length);
			Transform enemy = Instantiate (wave.enemy, SpawnPoints[spawnIndex].transform.position, SpawnPoints[spawnIndex].transform.rotation) as Transform;
			MultiplyStats (enemy);
			yield return new WaitForSeconds (1f/wave.spawnRate);
		}
		
		waveNumber += 1;
		
		if (waveIndex < Waves.Length - 1)
			waveIndex++;
		else {
			waveIndex = 0;
			multiplier *= 2f;
		}

		spawning = false;
	}
	
	// Delete this method if you are implementing this script into your own game
	// It's way too specific
	public void MultiplyStats (Transform enemy) {
		enemy.GetComponent<EnemyAI>().speed += enemy.GetComponent<EnemyAI>().speed * multiplier/10f;
		EnemyMaster em = enemy.GetComponent<EnemyMaster>();
		em.health = (int)(em.health * multiplier);
		em.moneyReward = (int) (em.moneyReward * multiplier);
		EnemyMelee melee = enemy.GetComponent<EnemyMelee>();
		if (melee != null) {
			melee.damage = (int)(melee.damage * multiplier);
			melee.hitRate = (multiplier);
		}
	}
}
