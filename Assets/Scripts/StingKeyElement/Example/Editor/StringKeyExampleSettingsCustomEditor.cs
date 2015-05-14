using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;
using JamUtilities;

public static class StringKeyExampleSettingsCustomEditor
{
	[MenuItem("Assets/Create/StringKeySettings/StringKeyExampleSettings")]
	public static void CreateStringKeyExampleSettings()
	{
		ScriptableObjectUtility.CreateAsset<StringKeyExampleSettings>();
	}
}
