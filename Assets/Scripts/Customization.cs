using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Customization : MonoBehaviour {
	
	public static bool CustomizationMenuEnabled = false;

	// UPGRADE COSTS
	
	const int agilityCost = 65;
	const int vitalityCost = 55;
	const int accuracyCost = 55;
	
	const int damageCost = 45;
	const int fireRateCost = 70;
	const int critChanceCost = 60;
	const int clipSizeCost = 30;
	const int reloadTimeCost = 35;
	
	// TEXT REFERENCES
	
	public Text moneyText;
	
	public Text agilityText;
	public Text vitalityText;
	public Text accuracyText;
	
	public Text damageText;
	public Text fireRateText;
	public Text critChanceText;
	public Text clipSizeText;
	public Text reloadTimeText;
	
	// TEXT COST REFERENCES
	
	public Text agilityCostText;
	public Text vitalityCostText;
	public Text accuracyCostText;
	
	public Text damageCostText;
	public Text fireRateCostText;
	public Text critChanceCostText;
	public Text clipSizeCostText;
	public Text reloadTimeCostText;
	
	// BUTTON REFERENCES
	
	public Button agilityButton;
	public Button vitalityButton;
	public Button accuracyButton;
	
	public Button damageButton;
	public Button fireRateButton;
	public Button critChanceButton;
	public Button clipSizeButton;
	public Button reloadTimeButton;
	
	private GameObject cv;
	
	void Start () {
		cv = transform.FindChild ("Canvas").gameObject;
		if (cv == null) {
			Debug.LogError ("No canvas under Customization?");
		}
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.U)) {
			ToggleMenu();
		}
		
		if (!CustomizationMenuEnabled)
			return;
		
		
		moneyText.text = "Money: " + PlayerStats.PlayerMoney.ToString() + "$";
		
		agilityCostText.text = agilityCost.ToString();
		vitalityCostText.text = vitalityCost.ToString();
		accuracyCostText.text = accuracyCost.ToString();
		
		damageCostText.text = damageCost.ToString();
		fireRateCostText.text = fireRateCost.ToString();
		critChanceCostText.text = critChanceCost.ToString();
		clipSizeCostText.text = clipSizeCost.ToString();
		reloadTimeCostText.text = reloadTimeCost.ToString();
		
		agilityText.text = PlayerStats.playerAgility.ToString();
		vitalityText.text = PlayerStats.playerVitality.ToString();
		accuracyText.text = PlayerStats.playerAccuracy.ToString();
		
		damageText.text = PlayerStats.playerDamage.ToString();
		fireRateText.text = PlayerStats.playerFireRate.ToString();
		critChanceText.text = PlayerStats.playerCritChance.ToString() + "%";
		clipSizeText.text = PlayerStats.playerClipSize.ToString();
		reloadTimeText.text = PlayerStats.playerReloadTime.ToString() + " sec";
		
		agilityButton.GetComponentInChildren<Text>().text = agilityCost.ToString() + "$";
		if (agilityCost >= PlayerStats.PlayerMoney || PlayerStats.playerAgility >= 220) {
			agilityButton.interactable = false;
		} else
			agilityButton.interactable = true;
			
		vitalityButton.GetComponentInChildren<Text>().text = vitalityCost.ToString() + "$";
		if (vitalityCost >= PlayerStats.PlayerMoney) {
			vitalityButton.interactable = false;
		} else
			vitalityButton.interactable = true;
			
		accuracyButton.GetComponentInChildren<Text>().text = accuracyCost.ToString() + "$";
		if (accuracyCost >= PlayerStats.PlayerMoney) {
			accuracyButton.interactable = false;
		} else
			accuracyButton.interactable = true;
			
		damageButton.GetComponentInChildren<Text>().text = damageCost.ToString() + "$";
		if (damageCost >= PlayerStats.PlayerMoney) {
			damageButton.interactable = false;
		} else
			damageButton.interactable = true;
			
		fireRateButton.GetComponentInChildren<Text>().text = fireRateCost.ToString() + "$";
		if (fireRateCost >= PlayerStats.PlayerMoney) {
			fireRateButton.interactable = false;
		} else if (PlayerStats.playerFireRate != 0)
			fireRateButton.interactable = true;
			
		critChanceButton.GetComponentInChildren<Text>().text = critChanceCost.ToString() + "$";
		if (critChanceCost >= PlayerStats.PlayerMoney) {
			critChanceButton.interactable = false;
		} else
			critChanceButton.interactable = true;
			
		clipSizeButton.GetComponentInChildren<Text>().text = clipSizeCost.ToString() + "$";
		if (clipSizeCost >= PlayerStats.PlayerMoney) {
			clipSizeButton.interactable = false;
		} else
			clipSizeButton.interactable = true;
			
		reloadTimeButton.GetComponentInChildren<Text>().text = reloadTimeCost.ToString() + "$";
		if (reloadTimeCost >= PlayerStats.PlayerMoney) {
			reloadTimeButton.interactable = false;
		} else
			reloadTimeButton.interactable = true;
	}
	
	public void ToggleMenu () {
		if (!CustomizationMenuEnabled) {
			Time.timeScale = 0f;
			cv.SetActive (true);
			CustomizationMenuEnabled = true;
		}
		else {
			Time.timeScale = 1f;
			cv.SetActive (false);
			CustomizationMenuEnabled = false;
		}
	}
	
	public void UpgradeAgility (int amount) {
		PlayerStats.playerAgility += amount;
		PlayerStats.PlayerMoney -= agilityCost;
	}
	public void UpgradeVitality (int amount) {
		PlayerStats.playerVitality += amount;
		PlayerStats.PlayerMoney -= vitalityCost;
	}
	public void UpgradeAccuracy (int amount) {
		PlayerStats.playerAccuracy += amount;
		PlayerStats.PlayerMoney -= accuracyCost;
	}
	
	public void UpgradeDamage (int amount) {
		PlayerStats.playerDamage += amount;
		PlayerStats.PlayerMoney -= damageCost;
	}
	public void UpgradeFireRate (int amount) {
		PlayerStats.playerFireRate += amount;
		PlayerStats.PlayerMoney -= fireRateCost;
	}
	public void UpgradeCritChance (int amount) {
		if (PlayerStats.playerCritChance == 100)
			return;
	
		PlayerStats.playerCritChance += amount;
		PlayerStats.PlayerMoney -= critChanceCost;
	}
	public void UpgradeClipSize (int amount) {
		PlayerStats.playerClipSize += amount;
		PlayerStats.PlayerMoney -= clipSizeCost;
	}
	public void UpgradeReloadTime (int amount) {
		PlayerStats.playerReloadTime = Mathf.Round( (PlayerStats.playerReloadTime - (float)amount * PlayerStats.playerReloadTime/100f) * 1000f) / 1000f;
		PlayerStats.PlayerMoney -= reloadTimeCost;
	}
	
}
