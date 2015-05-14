using UnityEngine;
using System.Collections.Generic;

namespace JamUtilities {

public static class GameObjectExtension 
{
	public static T GetInterface<T>(this GameObject go) where T : class
	{
		Component[] comps = go.GetComponents<Component>();
		for (int i = 0; i < comps.Length; ++i)
		{
			var obj = comps[i] as T;
			if (obj != null)
				return obj;
		}

		return null;
	}

	public static T[] GetInterfaces<T>(this GameObject go) where T : class
	{
		List<T> interfaces = new List<T>();
		Component[] comps = go.GetComponents<Component>();
		for (int i = 0; i < comps.Length; ++i)
		{
			var obj = comps[i] as T;
			if (obj != null)
				interfaces.Add(obj);
		}

		return interfaces.ToArray();
	}
}

}	// namespace JamUtilities