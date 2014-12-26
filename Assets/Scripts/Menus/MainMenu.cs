using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public string assaultDesc;
	public string tankDesc;
	public string sniperDesc;
	public Text descText;

	public Color bnColor;
	
	void Start () {
		GameObject.Find ("Assault").GetComponent<Image>().color = bnColor;
		PlayerStats.SelectedClass = "Assault";
	}

	public void PlayGame () {
		StartCoroutine (_PlayGame());
	}
	private IEnumerator _PlayGame () {
		float waitTime = Fade.FadeInstance.BeginFade(1);
		yield return new WaitForSeconds (waitTime);
		
		Application.LoadLevel (Application.loadedLevel + 1);
	}

	public void QuitGame () {
		Application.Quit();
	}

	public void LoadQualitySettingsMenu () {
		PlayerPrefs.SetInt ("firstLoad", 1);
		Application.LoadLevel ("QualitySettingsMenu");
	}

	public void SelectClass (string cName) {
		PlayerStats.SelectedClass = cName;
		
		GameObject[] bns = GameObject.FindGameObjectsWithTag ("ClassButtons");
		
		for (int i = 0; i < bns.Length; i++) {
			if (bns[i].name == cName)
				bns[i].GetComponent<Image>().color = bnColor;
			else
				bns[i].GetComponent<Image>().color = Color.white;
				
				
			
			if (cName == "Assault")
				descText.text = assaultDesc;
			else if (cName == "Tank")
				descText.text = tankDesc;
			else
				descText.text = sniperDesc;
		}
	}
}
