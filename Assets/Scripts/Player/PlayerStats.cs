using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public static string SelectedClass = "Assault";

	public static ClassStats PlayerClass = new ClassStats ("Assault");
	public static int PlayerMoney = 0;
	
	public static Vector3 PlayerPosition;
	public static GameObject PlayerInstance;
	
	public static void AdjustMoney (int amount) {
		PlayerMoney += amount;
		CombatText.SpawnCombatText (PlayerPosition, amount, CombatText.CombatTextType.money);
	}
	
	public static void ResetPlayer () {
		//Debug.Log (SelectedClass);
	
		PlayerMoney = 0;
		PlayerClass = new ClassStats (SelectedClass);
		
		playerHealth = PlayerClass.vitality;
		playerAgility = PlayerClass.agility;
		playerVitality = PlayerClass.vitality;
		playerAccuracy = PlayerClass.acuracy;
		
		playerDamage = PlayerClass.weaponStat.damage;
		playerFireRate = PlayerClass.weaponStat.firerate;
		playerCritChance = PlayerClass.weaponStat.critChance;
		playerClipSize = PlayerClass.weaponStat.clipSize;
		playerReloadTime = PlayerClass.weaponStat.reloadTime;
	}
	
	void Awake () {
		ResetPlayer ();
	}

	// CLASS STUFF
	
	private static int _health = playerVitality;
	public static int playerHealth {
		get {
			return ( _health ); 
		}
		set {
			_health = Mathf.Clamp (value, 0, playerVitality);
		}
	}
	
	public static float playerSpeed {
		get {
			return ( 2f + PlayerClass.agility/30 ); 
		}
	}
	
	public static float playerAimOffset {
		get {
			return ( (1f/(float)PlayerClass.acuracy) * 8f ); 
		}
	}
	
	public static int playerAgility {
		get {
			return ( PlayerClass.agility ); 
		}
		set {
			PlayerClass.agility = Mathf.Clamp (value, 0, 220);
		}
	}
	
	public static int playerVitality {
		get {
			return ( PlayerClass.vitality ); 
		}
		set {
			PlayerClass.vitality = value;
		}
	}
	
	public static int playerAccuracy {
		get {
			return ( PlayerClass.acuracy ); 
		}
		set {
			PlayerClass.acuracy = value;
		}
	}
	
	
	// WEAPON STUFF
	
	public static int playerDamage {
		get {
			return ( PlayerClass.weaponStat.damage ); 
		}
		set {
			PlayerClass.weaponStat.damage = value;
		}
	}
	
	public static int playerFireRate {
		get {
			return ( PlayerClass.weaponStat.firerate ); 
		}
		set {
			PlayerClass.weaponStat.firerate = value;
		}
	}
	
	public static int playerCritChance {
		get {
			return ( PlayerClass.weaponStat.critChance ); 
		}
		set {
			PlayerClass.weaponStat.critChance = Mathf.Clamp (value, 0, 100);
		}
	}
	
	public static int playerClipSize {
		get {
			return ( PlayerClass.weaponStat.clipSize ); 
		}
		set {
			PlayerClass.weaponStat.clipSize = value;
		}
	}
	
	public static float playerReloadTime {
		get {
			return ( PlayerClass.weaponStat.reloadTime ); 
		}
		set {
			PlayerClass.weaponStat.reloadTime = value;
		}
	}
}
