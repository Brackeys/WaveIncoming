using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangePlayerSprite : MonoBehaviour {

	public Sprite assaultSprite;
	public Sprite tankSprite;
	public Sprite sniperSprite;

	// Use this for initialization
	void Start () {
		Image img = GetComponent<Image>();
	
		if (PlayerStats.SelectedClass == "Assault")
			img.sprite = assaultSprite;
		else if (PlayerStats.SelectedClass == "Tank")
			img.sprite = tankSprite;
		else
			img.sprite = sniperSprite;	
	}

}
