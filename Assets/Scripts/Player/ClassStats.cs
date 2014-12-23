using UnityEngine;
using System.Collections;

public class ClassStats {

	public string name;
	
	public int vitality;
	public int agility;
	public int acuracy;
	
	public WeaponStats weaponStat;
	
	public ClassStats (string className) {
	
		if (className == "Assault") {
			name = "Assault";
			
			vitality = 40;
			agility = 80;
			acuracy = 50;
			
			weaponStat = new WeaponStats(5, 10, 7, 20, 1f);
		}
		
		if (className == "Tank") {
			name = "Tank";
			
			vitality = 110;
			agility = 8;
			acuracy = 90;
			
			weaponStat = new WeaponStats(14, 4, 3, 12, 2f);
		}
		
		if (className == "Sniper") {
			name = "Sniper";
			
			vitality = 20;
			agility = 70;
			acuracy = 250;
			
			weaponStat = new WeaponStats(40, 0, 30, 6, 2f);
		}
	}
}

public class WeaponStats {

	public int damage;
	public int firerate;
	public int critChance;
	public int clipSize;
	public float reloadTime;
	
	public WeaponStats (int dmg, int rate, int cc, int clip, float reload) {
		damage = dmg;
		firerate = rate;
		critChance = cc;
		clipSize = clip;
		reloadTime = reload;
	}
	
	public WeaponStats () {
		damage = 1;
		firerate = 1;
		critChance = 1;
		clipSize = 1;
		reloadTime = 1f;
	}
}