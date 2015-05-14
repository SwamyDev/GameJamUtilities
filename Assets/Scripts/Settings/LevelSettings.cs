using UnityEngine;
using System.Collections;

public class LevelSettings : ScriptableObject 
{
	public enum AspectRatio
	{
		RATIO_16_9,
		RATIO_16_10,
		RATIO_4_3
	}

	public AspectRatio optimisedAspectRatio;

	[SerializeField]
	[HideInInspector]
	private float startingTime;

	public float GetStartingTime() { return startingTime; }
	public void SetStartingTime(float time)
	{
		if (Application.isPlaying == false)
		{
			startingTime = time;
		}
		else
		{
			Debug.LogWarning("Starting time can only be set in edit mode");
		}
	}
}
