using UnityEngine;
using System.Collections;


namespace JamUtilities
{

public class StringKeyMask<T> where T : class, IStringKeyElement  
{
	private const int MASK_SIZE = sizeof(int) * 8;

	[SerializeField]
	private string[] keys;

	private int[] indexMasks = null;

	public bool Contains(StringKeyAttribute<T> attribute)
	{
		return Contains(attribute.GetIndex());
	}

	public bool Contains(T attribute)
	{
		int index = StringKeySettings<T>.Instance.GetIndex(attribute);
		return Contains(index);
	}

	private void InitMask()
	{
		StringKeySettings<T> settings = StringKeySettings<T>.Instance;

		int maskIndexFields = Mathf.CeilToInt(((float)settings.GetKeyCount()) / ((float)MASK_SIZE));

		indexMasks = new int[maskIndexFields];
		foreach (string key in keys)
		{
			int index = settings.GetIndex(key);
			AddIndex(index);
		}
	}

	private void AddIndex(int index)
	{
		int maskIndex = GetMaskIndex(index);
		int relativeIndex = index % MASK_SIZE;
		indexMasks[maskIndex] |= GetFlagValue(relativeIndex);
	}
	
	public bool Contains(int index)
	{
		if (indexMasks == null)
			InitMask();
		int maskIndex = GetMaskIndex(index);
		int relativeIndex = index % MASK_SIZE;
		return (indexMasks[maskIndex] & GetFlagValue(relativeIndex)) > 0;
	}

	private int GetFlagValue(int relativeIndex)
	{
		return 1 << relativeIndex;
	}

	private int GetMaskIndex(int index)
	{
		return Mathf.FloorToInt(((float)index) / ((float)MASK_SIZE));
	}
}

} /// namespace JamUtilities
