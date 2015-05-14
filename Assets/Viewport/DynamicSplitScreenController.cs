using UnityEngine;
using System;
using System.Collections.Generic;

public class DynamicSplitScreenController : MonoBehaviour 
{
	private struct CalculationResult
	{
		public float edgeSqrDiff { get; set; } 			// squared length from the intersection point to the midpoint
		public Vector3 playerToViewpoint { get; set; } 	// vector from the player to the players camera view point
		public bool needScreenSplit { get; set; } 		// is the screen split
		
		public void Create(bool needScreenSplit, Vector3 playerToViewpoint, float edgeSqrDiff)
		{
			this.needScreenSplit = needScreenSplit;
			this.playerToViewpoint = playerToViewpoint;
			this.edgeSqrDiff = edgeSqrDiff;
		}
	}
	
	//Debug Code
	private enum CircleDebugVariables
	{
		LOOKAT_POINT,
		MAX_CAMERA_DISTANCE,
		EDGE_VIEW,
		CAMERA_OFFSET,
		INTERSECTION1,
		INTERSECTION2,
		MIDPOINT,
	};

	public bool showDebugView = false;

	public GameObject playerOne;
	public GameObject playerTwo;

	public Vector2 playerCameraBounds = Vector2.one * 0.5f;

	public GameObject cameraOne;
	public GameObject cameraTwo;

	public int surfaceLayer = -1;
	public int clippingLayer = -1;
	
	public Vector3 cameraOffset = new Vector3(0, 3, -3);
	public float maxSplitLineWidth = 0.001f;
	public float distForMaxWidth = 1;
	public Material lineMaterial;

	//public int playerDebugged = 1;
	
	private GameObject separationLine;
	private LineRenderer lineRenderer;
	
	private GameObject clippingPlane;
	private SplitScreenDebugView debugView;
	
	// Use this for initialization
	public void Start () 
	{
		bool validated = ValidateState();

		if (validated)
		{
			InitializeCameras();
			InitializeGeometry();
		}
	}

	private bool ValidateState()
	{
		bool errorFound = false;

		if (playerOne == null)
		{
			errorFound = true;
			Debug.LogError("Player one object reference missing in DynamicSplitScreenController property pane");
		}

		if (playerTwo == null)
		{
			errorFound = true;
			Debug.LogError("Player two object reference missing in DynamicSplitScreenController property pane");
		}

		if (cameraOne == null)
		{
			errorFound = true;
			Debug.LogError("Camera one object reference missing in DynamicSplitScreenController property pane");
		}

		if (cameraOne.GetComponent<Camera>() == null)
		{
			errorFound = true;
			Debug.LogError("Camera one object referenced in DynamicSplitScreenController property pane does not have a camera component");
		}

		if (cameraTwo == null)
		{
			errorFound = true;
			Debug.LogError("Camera two object reference missing in DynamicSplitScreenController property pane");
		}

		if (cameraTwo.GetComponent<Camera>() == null)
		{
			errorFound = true;
			Debug.LogError("Camera two object referenced in DynamicSplitScreenController property pane does not have a camera component");
		}

		if (clippingLayer == -1)
		{
			clippingLayer = 8;
			Debug.LogWarning("Clipping layer not set in DynamicSplitScreenController property pane, using \"" +LayerMask.LayerToName(clippingLayer) + "\" layer for the clipping masks");
		}

		if (lineMaterial == null)
		{
			lineMaterial = new Material(Shader.Find("Particles/Additive"));
			lineMaterial.color = Color.black;
			Debug.LogWarning("No line material set in DynamicSplitScreenController property pane.. Creating a default one");
		}

		if (showDebugView && surfaceLayer == -1)
		{
			showDebugView = false;
			Debug.LogWarning("Unable to initialize debug view without a valid surface layer set in DynamicSplitScreenController property pane");
		}

		if (errorFound)
		{
			this.enabled = false;
		}

		return !errorFound;
	}

	private void InitializeCameras()
	{
		// initialize the camera positions and orientation
		Vector3 cameraLookAt = playerOne.transform.position;
		cameraOne.transform.position = cameraLookAt + cameraOffset;
		cameraOne.transform.LookAt(cameraLookAt);
		
		cameraLookAt = playerTwo.transform.position;
		cameraTwo.transform.position = cameraLookAt + cameraOffset;
		cameraTwo.transform.LookAt(cameraLookAt);
		
		// if we are in debug build set up the debugger helpers
		if (showDebugView && Debug.isDebugBuild)
		{
			debugView = new SplitScreenDebugView(Enum.GetNames(typeof(CircleDebugVariables)).Length);
			debugView.CacheEdgePoints(cameraOne, cameraLookAt, cameraOffset, surfaceLayer);
		}
		else
		{
			debugView = null;
		}

		// update the camera two culling mask
		Camera camera = cameraTwo.GetComponent<Camera>();
		camera.cullingMask = camera.cullingMask ^ (1 << clippingLayer);
	}

	private void InitializeGeometry()
	{
		// create and initialize the helper objects
		separationLine = new GameObject("SeparationLine");
		separationLine.layer = clippingLayer;
		separationLine.transform.position = cameraOne.transform.position;
		separationLine.transform.rotation = cameraOne.transform.rotation;
		separationLine.transform.parent = cameraOne.transform;
		lineRenderer = separationLine.AddComponent<LineRenderer>() as LineRenderer;
		
		Camera camera = cameraOne.GetComponent<Camera>();
		
		lineRenderer.SetVertexCount(2);

		lineRenderer.material = new Material(lineMaterial);//(Shader.Find("Particles/Additive")
		lineRenderer.useWorldSpace = false;
		lineRenderer.SetPosition(0, new Vector3(0, -0.8f, camera.nearClipPlane + 0.1f));
		lineRenderer.SetPosition(1, new Vector3(0,  0.8f, camera.nearClipPlane + 0.1f));
		
		clippingPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		clippingPlane.name = "ClippingPlane";
		clippingPlane.transform.parent = cameraOne.transform;
		
		clippingPlane.layer = clippingLayer;
		clippingPlane.transform.localPosition = new Vector3(0, 0, camera.nearClipPlane + 0.1f);
		
		Material myNewMaterial = Resources.Load("ClippingMaterial", typeof(Material)) as Material;;
		myNewMaterial.renderQueue = 1;
		clippingPlane.GetComponent<Renderer>().material = myNewMaterial;
	}

	// Update is called once per frame
	public void Update () 
	{
		CalculationResult p1Result = UpdateCamera(cameraOne, playerOne, playerTwo, true);
		CalculationResult p2Result = UpdateCamera(cameraTwo, playerTwo, playerOne, false);

		bool hasCameraSplit = p1Result.needScreenSplit || p2Result.needScreenSplit;

		CalculationResult furthestPlayer = (p1Result.edgeSqrDiff > p2Result.edgeSqrDiff) ? p1Result : p2Result;
		UpdatePlanes(cameraOne, hasCameraSplit, furthestPlayer.edgeSqrDiff, p1Result.playerToViewpoint);
	}
	
	private CalculationResult UpdateCamera(GameObject cameraObject, GameObject playerToFollow, GameObject otherPlayer, bool debug)
	{
		CalculationResult result = new CalculationResult();

		Camera camera = cameraObject.GetComponent<Camera>();
		Vector3 playerPosition = playerToFollow.transform.position;

		cameraObject.transform.position = playerPosition + cameraOffset;

		Vector3 midpoint = (playerPosition + otherPlayer.transform.position) / 2;

		Vector3 playerToMidpoint = (otherPlayer.transform.position - playerPosition)/2;
		Vector3 dirToMidpoint = playerToMidpoint;
		dirToMidpoint.Normalize();

		Vector3 finalLookAtPoint;
		Vector3 playerToViewPoint = midpoint - playerPosition;

		Vector3 midpointInCameraCoord = camera.WorldToScreenPoint(midpoint);

		Rect cameraBounds = camera.pixelRect;
		Vector2 cameraCenter = new Vector2(cameraBounds.width/2, cameraBounds.height/2);

		Vector2 intersection1 = Vector2.zero, intersection2 = Vector2.zero, intersection;

		Vector2 midpointToElipseCoord = Vector2.zero;
		midpointToElipseCoord.x = midpointInCameraCoord.x - cameraCenter.x;
		midpointToElipseCoord.y = midpointInCameraCoord.y - cameraCenter.y;

		Vector2 cameraCircleDilation = playerCameraBounds;
		cameraCircleDilation.x *= cameraCenter.x;
		cameraCircleDilation.y *= cameraCenter.y;

		ElipseIntersection(cameraCircleDilation.x, cameraCircleDilation.y, midpointToElipseCoord, ref intersection1, ref intersection2);

		intersection = intersection2;
		Ray cameraRay = camera.ScreenPointToRay(intersection + cameraCenter);

		Vector3 closestPointOnMidpointVector;
		Vector3 closestPointOnRayVector;

		ClosestPointsOnTwoLines(out closestPointOnRayVector, out closestPointOnMidpointVector, cameraRay.origin, cameraRay.direction*100, playerPosition, -dirToMidpoint*100);

		Vector3 intersectionToPlayer = playerPosition - closestPointOnMidpointVector;
		Vector3 intersectionToMidpoint = midpoint - closestPointOnMidpointVector;

		if (intersectionToMidpoint.sqrMagnitude < playerToMidpoint.sqrMagnitude)
		{
			// The other point is the furthest..
			intersection = intersection1;
			cameraRay = camera.ScreenPointToRay(intersection + cameraCenter);
			ClosestPointsOnTwoLines(out closestPointOnRayVector, out closestPointOnMidpointVector, cameraRay.origin, cameraRay.direction*100, playerPosition, -dirToMidpoint*100);

			intersectionToPlayer = playerPosition - closestPointOnMidpointVector;
		}

		//float sqrMaxCameraDistance = intersectionToPlayer.sqrMagnitude;
		//float sqrDistance = dirToViewPoint.sqrMagnitude;
		bool isFar = playerToMidpoint.sqrMagnitude > intersectionToPlayer.sqrMagnitude;

//		if (debug && debugView != null)
//		{
//			debugView.DrawMoveCone(cameraObject, camera, surfaceLayer, cameraCircleDilation, Color.blue);
//			Debug.DrawRay(playerPosition, -dirToMidpoint*100, Color.green);
//			Debug.DrawRay(cameraRay.origin, cameraRay.direction*100, Color.green);
//			debugView.UpdateDebugElement((int)CircleDebugVariables.MIDPOINT, playerPosition + playerToMidpoint, 0.3f);
//
//			debugView.UpdateDebugElement((int)CircleDebugVariables.CAMERA_OFFSET, playerPosition + dirToMidpoint * intersectionToPlayer.magnitude, 0.25f, Color.white);
//
//			debugView.TraceCameraPoint(cameraObject, camera, surfaceLayer, intersection+cameraCenter, (int)CircleDebugVariables.INTERSECTION1, Color.cyan);
//			debugView.UpdateDebugElement((int)CircleDebugVariables.INTERSECTION2, closestPointOnMidpointVector, 0.2f, Color.red);
//		}

		if (isFar)
		{
			finalLookAtPoint = playerPosition + dirToMidpoint * intersectionToPlayer.magnitude;
		}
		else
		{
			finalLookAtPoint = midpoint;
		}

//		if (debug && debugView != null)
//		{
//			debugView.UpdateDebugElement((int)CircleDebugVariables.MIDPOINT, playerPosition + playerToMidpoint, 0.3f);
//			debugView.UpdateDebugElement((int)CircleDebugVariables.LOOKAT_POINT, finalLookAtPoint, 0.1f);
//			debugView.DrawCachedEdgePoints(cameraObject.transform.position);
//		}

		cameraObject.transform.position = finalLookAtPoint + cameraOffset;

		if (debug && debugView != null)
		{
			debugView.DrawMoveCone(cameraObject, camera, surfaceLayer, cameraCircleDilation, Color.cyan);
			debugView.DrawCachedEdgePoints(cameraObject.transform.position);
			Debug.DrawRay(cameraObject.transform.position, finalLookAtPoint - cameraObject.transform.position, Color.grey);
		}

		result.Create(isFar, playerToViewPoint, (playerToMidpoint.sqrMagnitude - intersectionToPlayer.sqrMagnitude));

		return result;
	}
	
	private void UpdatePlanes(GameObject cameraObj, bool isFar, float sqrDiff, Vector3 dirToViewPoint)
	{
		float scale = 0f;
		
		if (isFar)
		{
			float sqrLineMaxDist = distForMaxWidth*distForMaxWidth;

			if (distForMaxWidth == 0)
			{
				Debug.LogWarning("splitLineDistForMaxWidth value is 0! defaulting to 1");
				sqrLineMaxDist = 1;
			}

			float perc = sqrDiff/sqrLineMaxDist;
			scale = Mathf.Lerp(0, maxSplitLineWidth, perc);
		}
		
		lineRenderer.SetWidth(scale, scale);
		
		//dirToViewPoint.Normalize();
		//clipping_plane.transform.localRotation = Quaternion.FromToRotation(Vector3.right, dirToViewPoint);

		float angle = Vector3.Angle(Vector3.right, dirToViewPoint);
		if (dirToViewPoint.z < 0)
		{
			angle = 360 - angle;
		}
		clippingPlane.transform.localPosition = Vector3.zero;

		clippingPlane.transform.rotation = cameraObj.transform.rotation * Quaternion.AngleAxis(270, Vector3.right) * Quaternion.FromToRotation(Vector3.right, dirToViewPoint);
		separationLine.transform.rotation = cameraObj.transform.rotation * Quaternion.AngleAxis(angle, Vector3.forward);

		//dirToViewPoint *= 2;

		clippingPlane.transform.position = cameraObj.transform.position + clippingPlane.transform.right * clippingPlane.transform.localScale.x * 5 - clippingPlane.transform.up * 0.5f;
	}

	private bool ElipseIntersection(float a, float b, Vector2 p, ref Vector2 intersection1, ref Vector2 intersection2)
	{
		float coef = a * b / Mathf.Sqrt(a*a*p.y*p.y + b*b*p.x*p.x);
		
		intersection1.x = coef * p.x;
		intersection1.y = coef * p.y;
		
		intersection2.x = -intersection1.x;
		intersection2.y = -intersection1.y;
		
		return true;
	}

	//FROM: MATH3D
	//Two non-parallel lines which may or may not touch each other have a point on each line which are closest
	//to each other. This function finds those two points. If the lines are not parallel, the function 
	//outputs true, otherwise false.
	private bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
		
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
		
		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
		
		float d = a*e - b*b;
		
		//lines are not parallel
		if(d != 0.0f){
			
			Vector3 r = linePoint1 - linePoint2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
			
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
			
			closestPointLine1 = linePoint1 + lineVec1 * s;
			closestPointLine2 = linePoint2 + lineVec2 * t;
			
			return true;
		}
		
		else{
			return false;
		}
	}

	void OnDrawGizmos() 
	{
		if (showDebugView && debugView != null)
		{
			debugView.DrawGizmos();
		}
	}


}
