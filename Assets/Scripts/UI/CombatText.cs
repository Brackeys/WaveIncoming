using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatText : MonoBehaviour {

	public enum CombatTextType {
		damage,
		money,
		heal
	};

	public static Transform TextPrefab;
	public Transform textPrefab;
	
	public static Color DamageColor;
	public Color damageColor;
	
	public static Color MoneyColor;
	public Color moneyColor;
	
	public static Color HealColor;
	public Color healColor;
	
	void Awake () {
		TextPrefab = textPrefab;
		DamageColor = damageColor;
		MoneyColor = moneyColor;
		HealColor = healColor;
	}

	public static void SpawnCombatText (Vector3 pos, int amount, CombatTextType type) {
	
		Vector3 finalPos = new Vector3 (pos.x + Random.Range (-0.1f, 0.1f), pos.y + Random.Range (-0.1f, 0.1f), 0);
	
		Transform clone = Instantiate (TextPrefab, finalPos, Quaternion.identity) as Transform;

		Text cText = clone.GetComponentInChildren<Text>();
		
		if (amount > 0)
			cText.text = "+" + amount.ToString();
		else
			cText.text = amount.ToString();
		
		if (type == CombatTextType.damage) {
			cText.color = new Color (DamageColor.r, DamageColor.g, DamageColor.b, cText.color.a);
		} else if (type == CombatTextType.money) {
			cText.color = new Color (MoneyColor.r, MoneyColor.g, MoneyColor.b, cText.color.a);
		} else {
			cText.color = new Color (HealColor.r, HealColor.g, HealColor.b, cText.color.a);
			clone.position = new Vector3 (clone.position.x + 0.5f, clone.position.y, clone.position.z);
		}
		
		cText.fontSize += Random.Range (-2, 5);
		
		Destroy (clone.gameObject, 1f);
	}
	
}
