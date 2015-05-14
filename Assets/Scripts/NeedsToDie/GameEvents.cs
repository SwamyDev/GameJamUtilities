using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JamUtilities {

public class GameEvents : MonoBehaviour 
{
	public enum Events
	{
		ON_DAMAGED,
		ON_ENGERY_CHANGED,
		ON_INSTANCED_IN,
		ON_INSTANCED_OUT,
		COUNT
	}

	private List<System.Action<object, GameObject> >[] eventDelegates;

	void Awake()
	{
		eventDelegates = new List<System.Action<object, GameObject>>[(int)Events.COUNT];
		for (int i = 0; i < eventDelegates.Length; ++i)
		{
			eventDelegates[i] = new List<System.Action<object, GameObject> >();
		}
	}

	public void RegisterFor(Events newEvent, System.Action<object, GameObject> onEvent)
	{
		eventDelegates[(int)newEvent].Add(onEvent);
	}

	public void Notify(Events eventType, object data, GameObject sender)
	{
		foreach (System.Action<object, GameObject> action in eventDelegates[(int)eventType])
		{
			action(data, sender);
		}
	}
}

}	// namespace JamUtilities