using UnityEngine;
using UnityEditor;
using System.Collections;

namespace JamUtilities {

public static class PlayerDataSettingsMenuItem 
{
	[MenuItem("Assets/Create/StringKeySettings/PlayerDataSettings")]
	private static void CreatePlayerDataSetting()
	{
		ScriptableObjectUtility.CreateAsset<PlayerDataSettings>();
	}
}

}	// namespace JamUtilities
