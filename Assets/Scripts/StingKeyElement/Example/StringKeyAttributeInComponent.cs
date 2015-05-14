using UnityEngine;
using System.Collections;
using JamUtilities;

public class StringKeyAttributeInComponent : MonoBehaviour
{
	[SerializeField]
	StringKeyExampleAttribute stringKeyExampleAttribute;
	[SerializeField]
	StringKeyExampleMask stringKeyExampleMask;

	void Start()
	{
		if (stringKeyExampleMask.Contains(stringKeyExampleAttribute))
		{
			Debug.Log("Contained!");
		}
		Debug.Log(stringKeyExampleAttribute.Get().GetKey() + ": " + stringKeyExampleAttribute.Get().hufer);
	}
}
