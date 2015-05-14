using UnityEngine;
using System.Collections;

public static class TimeProvider 
{
	public static float CurrentLevelTime()
	{
		float startTime = SettingsManager.GetInstance().levelSettings.GetStartingTime();
		return startTime + Time.timeSinceLevelLoad;
	}

	public static float GetMyDeltaTime()
	{
		return Time.deltaTime;
	}

	public static IEnumerator WaitForMySeconds(float seconds)
	{
		float t = 0;
		while (t < seconds)
		{
			t += GetMyDeltaTime();
			yield return new WaitForEndOfFrame();
		}
	}

	public static IEnumerator WaitForLevelTime(float time)
	{
		float t = SettingsManager.GetInstance().levelSettings.GetStartingTime();
		while (t > time)
		{
			yield return new WaitForEndOfFrame();
		}

		while (t < time)
		{
			t += GetMyDeltaTime();
			yield return new WaitForEndOfFrame();
		}
	}
}
