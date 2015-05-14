using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace JamUtilities
{

public class StringKeySettings<T> : ScriptableObject where T : IStringKeyElement
{
	public class ElementAndIndex
	{
		public int index;
		public T element;

		public ElementAndIndex(int index, T element)
		{
			this.index = index;
			this.element = element;
		}
	}

	private static StringKeySettings<T> instance;

	public static StringKeySettings<T> Instance
	{
		get
		{
			if (instance == null)
			{
				instance = StringKeySettingsSetting.Instance.GetSetting<T>();
			}
			return instance;
		}
	}

	[SerializeField]
	private T[] settings;

	private ElementAndIndex[] indexedSettings = null;

	public List<string> GetKeys ()
	{
		List<string> keys = new List<string>();
		foreach (T setting in settings)
		{
			keys.Add(setting.GetKey());
		}
		return keys;
	}

	public int GetKeyCount()
	{
		return settings.Length;
	}

	public ElementAndIndex Get(string key)
	{
		if (indexedSettings == null)
		{
			InitIndex();
		}
		foreach (ElementAndIndex indexedSetting in indexedSettings)
		{
			if (indexedSetting.element.GetKey().Equals(key))
			{
				return indexedSetting;
			}
		}

		KeyNotFoundError(key);
		return null;
	}

	public int GetIndex(T element)
	{
		for (int i = 0; i < indexedSettings.Length; ++i)
		{
			if (settings[i].GetKey().Equals(element.GetKey()))
			{
				return i;
			}
		}		
		KeyNotFoundError(element.GetKey());
		return -1;
	}

	public int GetIndex(string key)
	{
		ElementAndIndex elementAndIndex = Get(key);
		if (elementAndIndex == null)
		{
			return -1;
		}
		return elementAndIndex.index;
	}
	
	private void KeyNotFoundError(string key)
	{
		Debug.LogError("Unknown key \"" + key + "\" for " + name);
	}

	void InitIndex ()
	{
		indexedSettings = new ElementAndIndex[settings.Length];
		for (int i = 0; i < indexedSettings.Length; ++i)
		{
			indexedSettings[i] = new ElementAndIndex(i, settings[i]);
		}
	}

	public T Get(int index)
	{
		return settings[index];
	}

	public Type GetSettingType()
	{
		return typeof(T);
	}
}

} /// namespace JamUtilies