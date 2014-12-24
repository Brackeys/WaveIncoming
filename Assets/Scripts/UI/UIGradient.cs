﻿//-----------------------------------------------------------------
// Adds a simple vertical gradient to an image/text element in 4.6+
//-----------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseVertexEffect {
	[SerializeField]
	private Color32 topColor = Color.white;
	[SerializeField]
	private Color32 bottomColor = Color.black;
	
	public override void ModifyVertices(List<UIVertex> vertexList) {
		if (!IsActive()) {
			return;
		}
		
		int count = vertexList.Count;
		float bottomY = vertexList[0].position.y;
		float topY = vertexList[0].position.y;
		
		for (int i = 1; i < count; i++) {
			float y = vertexList[i].position.y;
			if (y > topY) {
				topY = y;
			}
			else if (y < bottomY) {
				bottomY = y;
			}
		}
		
		float uiElementHeight = topY - bottomY;
		
		for (int i = 0; i < count; i++) {
			UIVertex uiVertex = vertexList[i];
			uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
			vertexList[i] = uiVertex;
		}
	}
}