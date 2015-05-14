using System.Collections;
using UnityEngine;

public class SplitScreenDebugView
{
	private class CircleDebugElement
	{
		public Vector3 position;
		public float size;
		public Color color;
		public bool isVisible;
	};

	private class ScreenEdgePoint
	{
		public Vector3 hitOffset { get; set; }
		public Vector3 positionLimitOffset { get; set; }
		public Vector3 offsetFromCamera { get; set; }
		public Vector3 direction { get; set; }
		public float distance { get; set; }
		public float angleFromCenter { get; set; }
	};

	private int segments;
	private ScreenEdgePoint[] edgePoints;
	private CircleDebugElement[] circleDebugElements;

	public SplitScreenDebugView(int numCircleElements)
	{
		segments = 50;
		edgePoints = new ScreenEdgePoint[4];
		circleDebugElements = new CircleDebugElement[numCircleElements];

		for (int i=0; i<numCircleElements; ++i)
		{
			circleDebugElements[i] = new CircleDebugElement();
		}

		for (int i=0; i<4; ++i)
		{
			edgePoints[i] = new ScreenEdgePoint();
		}
	}

	public void CacheEdgePoints(GameObject cameraObj, Vector3 cameraOffset, Vector3 cameraLookAt, int surfaceLayer)
	{
		SetEdgePoint(cameraObj, cameraLookAt, cameraOffset, surfaceLayer, new Vector3(0,0,0), edgePoints[0]);
		SetEdgePoint(cameraObj, cameraLookAt, cameraOffset, surfaceLayer, new Vector3(1,0,0), edgePoints[1]);
		SetEdgePoint(cameraObj, cameraLookAt, cameraOffset, surfaceLayer, new Vector3(1,1,0), edgePoints[2]);
		SetEdgePoint(cameraObj, cameraLookAt, cameraOffset, surfaceLayer, new Vector3(0,1,0), edgePoints[3]);
	}

	public void DrawEdgePoints(GameObject cameraObj, Vector3 cameraOffset, int surfaceLayer)
	{
		Camera cameraCmp = cameraObj.GetComponent<Camera>();
		CastAndDrawRayToScreenPoint(cameraCmp, new Vector3(0,0,0), cameraOffset, surfaceLayer);
		CastAndDrawRayToScreenPoint(cameraCmp, new Vector3(1,0,0), cameraOffset, surfaceLayer);
		CastAndDrawRayToScreenPoint(cameraCmp, new Vector3(0,1,0), cameraOffset, surfaceLayer);
		CastAndDrawRayToScreenPoint(cameraCmp, new Vector3(1,1,0), cameraOffset, surfaceLayer);
	}

	public void DrawCachedEdgePoints(Vector3 cameraPosition)
	{
		ScreenEdgePoint point = edgePoints[3];
		Vector3 edgePoint = Vector3.zero;
		Vector3 previousEdgePoint = cameraPosition + point.offsetFromCamera + point.direction * point.distance;
		//Vector3 previousHalfwayPoint = (finalLookAtPoint + previousEdgePoint) / 2;
		
		for(int i=0; i<4; ++i)
		{
			point = edgePoints[i];
			edgePoint = cameraPosition + point.offsetFromCamera + point.direction * point.distance;
			edgePoint.y += 0.01f; // for better viewing
			Debug.DrawRay(cameraPosition + point.offsetFromCamera, point.direction * point.distance, Color.red);
			Debug.DrawRay(previousEdgePoint, edgePoint - previousEdgePoint, Color.red);
			
			//halfwayPoint = (finalLookAtPoint + edgePoint) / 2;
			//Debug.DrawRay(previousHalfwayPoint, halfwayPoint - previousHalfwayPoint, Color.cyan);
			
			previousEdgePoint = edgePoint;
			//previousHalfwayPoint = halfwayPoint;
		}
	}

	private void SetEdgePoint(GameObject cameraObj, Vector3 cameraCenter, Vector3 cameraOffset, int surfaceLayer, Vector3 relativePoint, ScreenEdgePoint edgePoint)
	{
		Camera camera = cameraObj.GetComponent<Camera>();
		relativePoint.x *= camera.pixelWidth;
		relativePoint.y *= camera.pixelHeight;
		
		Ray ray = camera.ScreenPointToRay(relativePoint);
		RaycastHit rayHit = new RaycastHit();
		
		int layerMask = 1 << surfaceLayer;
		
		if (Physics.Raycast(ray, out rayHit, float.PositiveInfinity, layerMask))
		{
			edgePoint.hitOffset = rayHit.point - cameraCenter;
			edgePoint.positionLimitOffset = (cameraCenter + edgePoint.hitOffset) / 2;
			edgePoint.offsetFromCamera = ray.origin - cameraObj.transform.position;
			edgePoint.direction = ray.direction;
			edgePoint.distance = rayHit.distance;
			edgePoint.angleFromCenter = Vector3.Angle(Vector3.right, rayHit.point - cameraCenter);
			
			if (rayHit.point.z < cameraCenter.z)
			{
				edgePoint.angleFromCenter = 360 - edgePoint.angleFromCenter;
			}
		}
	}

	private void CastAndDrawRayToScreenPoint(Camera camera, Vector3 relativePoint, Vector3 cameraOffset, int surfaceLayer)
	{
		relativePoint.x *= camera.pixelWidth;
		relativePoint.y *= camera.pixelHeight;
		
		Ray ray = camera.ScreenPointToRay(relativePoint);
		RaycastHit rayHit = new RaycastHit();
		
		int layerMask = 1 << surfaceLayer;
		
		if (Physics.Raycast(ray, out rayHit, cameraOffset.sqrMagnitude, layerMask))
		{
			Debug.DrawRay(ray.origin, ray.direction * rayHit.distance, Color.cyan);
		}
	}

	public void TraceCameraPoint(GameObject cameraObject, Camera camera, int surfaceLayer, Vector2 point, int index, Color color)
	{
		if (index >= 0 && index < circleDebugElements.Length)
		{
			int layerMask = 1 << surfaceLayer;
			
			Ray ray = camera.ScreenPointToRay(point);
			RaycastHit rayHit = new RaycastHit();
			
			if (Physics.Raycast(ray, out rayHit, 100, layerMask))
			{
				Vector3 hit = rayHit.point;
				hit.y += 0.05f;
				
				UpdateDebugElement(index, hit, 0.5f, color);
			}
		}
	}

	public void DrawMoveCone(GameObject obj, Camera camera, int surfaceLayer, Vector2 radius, Color color)
	{
		int layerMask = 1 << surfaceLayer;
		float x;
		float y;
		float angle = 20f;
		float increment = (360f / segments);
		Vector3 currentHit = Vector2.zero;
		Vector3 previousHit = Vector2.zero;
		Vector3 firstHit = Vector2.zero;
		
		float halfWidth = camera.pixelWidth/2;
		float halfHeight = camera.pixelHeight/2;
		
		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius.x + halfWidth;
			y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius.y + halfHeight;
			angle += increment;
			
			Vector3 relativePoint = new Vector3(x, y, 0);
			Ray ray = camera.ScreenPointToRay(relativePoint);
			RaycastHit rayHit = new RaycastHit();
			
			if (Physics.Raycast(ray, out rayHit, 100, layerMask))
			{
				currentHit = rayHit.point;
				currentHit.y += 0.05f;
				
				if (i>1)
				{
					Debug.DrawRay(previousHit, currentHit - previousHit, color);
				}
				else
				{
					firstHit = currentHit;
				}
				
				previousHit = currentHit;
			}
		}
		
		Debug.DrawRay(previousHit, firstHit - previousHit, color);
	}

	public void UpdateDebugElement(int index, Vector3 position, float size)
	{
		UpdateDebugElement(index, position, size, Color.yellow);
	}
	
	public void UpdateDebugElement(int index, Vector3 position, float size, Color color)
	{
		if (index >= 0 && index < circleDebugElements.Length)
		{
			CircleDebugElement element = circleDebugElements[index];
			element.position = position;
			element.size = size;
			element.color = color;
			element.isVisible = true;
		}
	}
	
	public void DrawGizmos()
	{
		foreach (CircleDebugElement element in circleDebugElements)
		{
			if (element.isVisible)
			{
				Gizmos.color = element.color;
				Gizmos.DrawWireSphere(element.position, element.size);
			}
			
			element.isVisible = false;
		}
	}
}


