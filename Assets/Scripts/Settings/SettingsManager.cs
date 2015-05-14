using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class SettingsManager : MonoBehaviour
{
	public LevelSettings levelSettings;

	private static SettingsManager instance;
	public static SettingsManager GetInstance() 
	{
		if (instance == null)
			instance = FindObjectOfType<SettingsManager>();

		return instance; 
	}

	void Awake()
	{
		instance = this;
	}
}

}	// namespace JamUtilities