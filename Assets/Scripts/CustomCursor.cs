using UnityEngine;
using System.Collections;

public class CustomCursor : MonoBehaviour {

	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot = Vector2.zero;

	void OnMouseEnter () {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
	
	void OnMouseExit () {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
}