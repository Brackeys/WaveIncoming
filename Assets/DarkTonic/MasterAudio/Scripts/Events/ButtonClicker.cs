using UnityEngine;
using System.Collections;

[AddComponentMenu("Dark Tonic/Master Audio/ButtonClicker")]
public class ButtonClicker : MonoBehaviour {
	public bool resizeOnClick = true;
	public bool resizeOnHover = false;
	public string mouseDownSound = string.Empty;
	public string mouseUpSound = string.Empty;
	public string mouseClickSound = string.Empty;
	public string mouseOverSound = string.Empty;
	public string mouseOutSound = string.Empty;
	
	private Vector3 originalSize;
	private Vector3 smallerSize;
	private Vector3 largerSize;
	private Transform trans;

	// This script can be triggered from NGUI clickable elements only. You can change the OnPress method to another if you have another use for this.
	void Awake() {
		this.trans = this.transform;
		this.originalSize = trans.localScale;
		this.smallerSize = this.originalSize * 0.90f;
		this.largerSize = this.originalSize * 1.10f;
	}
	
	void OnPress(bool isDown) {
		if (isDown) {
			if (enabled) {
				MasterAudio.PlaySound(mouseDownSound);
				
				if (resizeOnClick) {			
					trans.localScale = this.smallerSize;
				}
			}
		} else {
			if (enabled) {
				MasterAudio.PlaySound(mouseUpSound);
			}
			// still want to restore size if disabled

			if (resizeOnClick) {			
				trans.localScale = this.originalSize;
			}
		}
	}

	void OnClick() {
		if (enabled) {
			MasterAudio.PlaySound(mouseClickSound);
		}
	}

	void OnHover(bool isOver) {
		if (isOver) {
			if (enabled) {
				MasterAudio.PlaySound(mouseOverSound);

				if (resizeOnHover) {
					trans.localScale = this.largerSize;
				}
			}
		} else {
			if (enabled) {
				MasterAudio.PlaySound(mouseOutSound);
			}
			// still want to restore size if disabled

			if (resizeOnHover) {
				trans.localScale = this.originalSize;
			}
		}
	}
}
