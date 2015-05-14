using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace JamUtilities
{

public class StringKeySettingsSetting : MonoSingleton<StringKeySettingsSetting>
{
	[SerializeField]
	private ScriptableObject[] settings;

	public StringKeySettings<T> GetSetting<T>() where T : IStringKeyElement
	{
		ScriptableObject result  = null;
		foreach (ScriptableObject setting in settings)
		{
			StringKeySettings<T> s = setting as StringKeySettings<T>;
			if (s != null)
			{
				CheckForMultiDefinitions<T>(result);
				result = s;
			}
		}
		CheckForNoDefinition<T>(result);
		return result as StringKeySettings<T>;
	}

	private void CheckForMultiDefinitions<T>(ScriptableObject result)
	{
		if (result != null) 
		{
			Debug.LogError("More then one setting for type " + typeof(T).ToString() + " found!");
		}
	}

	private void CheckForNoDefinition<T>(ScriptableObject result)
	{
		if (result == null) 
		{
			Debug.LogError("No setting for type " + typeof(T).ToString() + " found!");
		}
	}
}

}	// namespace JamUtilities