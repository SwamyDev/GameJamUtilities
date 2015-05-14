using UnityEngine;
using System.Collections;
using System;

namespace JamUtilities
{

[Serializable]
public class StringKeyAttribute<T> : IEquatable<StringKeyAttribute<T>> where T : class, IStringKeyElement
{
	[SerializeField]
	private string key;
	
	private StringKeySettings<T>.ElementAndIndex elementAndIndex = null;

	public T Get()
	{
		if (elementAndIndex == null)
		{
			InitElement();
		}
		return elementAndIndex.element;
	}

	public int GetIndex()
	{
		if (elementAndIndex == null)
		{
			InitElement();
		}
		return elementAndIndex.index;
	}

	private void InitElement()
	{
		StringKeySettings<T> settings = StringKeySettings<T>.Instance;
		elementAndIndex = settings.Get(key);
	}

	
	public static bool operator !=(StringKeyAttribute<T> x, StringKeyAttribute<T> y) 
	{
		return !(x == y);
	}

	public static bool operator ==(StringKeyAttribute<T> x, StringKeyAttribute<T> y) 
	{
		if ((y == null) && (x == null))
			return true;

		if (x == null)
			return false;

		return x.Equals(y);
	}

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
		{
			return false;
		}

		StringKeyAttribute<T> p = obj as StringKeyAttribute<T>;
		if (p == null)
		{
			return false;
		}

		return p.elementAndIndex == elementAndIndex;
	}
	
	public bool Equals(StringKeyAttribute<T> p)
	{
		if (p == null)
		{
			return false;
		}
		
		return p.elementAndIndex == elementAndIndex;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

} /// namespace JamUtilities
