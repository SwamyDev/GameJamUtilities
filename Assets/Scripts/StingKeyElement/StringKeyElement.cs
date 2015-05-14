using UnityEngine;
using System.Collections;
using System;


namespace JamUtilities
{

[Serializable]
public class StringKeyElement : IStringKeyElement
{
	[SerializeField]
	private string key;

	public string GetKey()
	{
		return key;
	}
}

} /// namespace JamUtilities

