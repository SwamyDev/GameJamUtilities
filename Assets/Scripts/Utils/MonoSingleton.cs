using UnityEngine;

namespace JamUtilities {

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T instance = null;
	
	public static T Instance 
	{
		get 
		{
			instance = instance ?? FindObjectOfType<T>();
			if (instance == null)
			{
				Debug.LogError(string.Format("Couldn't find {0}. Please add it to the scene.", typeof(T).Name));
			}

			return instance;
		}
	}
}

}	// namespace JamUtilities