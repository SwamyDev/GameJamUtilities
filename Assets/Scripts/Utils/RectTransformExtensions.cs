using UnityEngine;
using System.Collections.Generic;

public static class RectTransformExtensions 
{
	public static void FitPowerFlower(this RectTransform rectTrans)
	{
		RectTransform parentRectTrans = rectTrans.parent as RectTransform;

		if (parentRectTrans)
		{
			Vector2 parentInvertScale = new Vector2(1.0f / parentRectTrans.rect.width
				, 1.0f / parentRectTrans.rect.height);

			Vector2 newAnchorsMin = rectTrans.anchorMin + Vector2.Scale(rectTrans.offsetMin, parentInvertScale);
			Vector2 newAnchorsMax = rectTrans.anchorMax + Vector2.Scale(rectTrans.offsetMax, parentInvertScale);

			rectTrans.anchorMin = newAnchorsMin;
			rectTrans.anchorMax = newAnchorsMax;
			rectTrans.offsetMin = rectTrans.offsetMax = Vector2.zero;
		}
	}

	public static void CopyData(this RectTransform targetTrans, RectTransform sourceTrans)
	{
		targetTrans.position = sourceTrans.position;
		targetTrans.sizeDelta = sourceTrans.sizeDelta;
		targetTrans.anchoredPosition = sourceTrans.anchoredPosition;
		targetTrans.anchoredPosition3D = sourceTrans.anchoredPosition3D;
		targetTrans.pivot = sourceTrans.pivot;
		targetTrans.anchorMin = sourceTrans.anchorMin;
		targetTrans.anchorMax = sourceTrans.anchorMax;
		targetTrans.offsetMin = sourceTrans.offsetMin;
		targetTrans.offsetMax = sourceTrans.offsetMax;
	}
}
