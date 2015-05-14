using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace JamUtilities
{

public class StringKeyMaskDrawer<T> : PropertyDrawer where T : IStringKeyElement
{
	private const int INT_BIT_SIZE = sizeof(int) * 8 - 1;
	private const float dropDownWidth = 0.8f;
	private const int border = 2;
	private const float ENTRY_HEIGHT= 25.0f;
	
	private List<string> availableKeys;
	private string postFix = "";
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		InitializeKey();
		return GetMaskCount() * ENTRY_HEIGHT; ;
	}
	
	private int GetMaskCount()
	{
		return Mathf.CeilToInt((float)availableKeys.Count / INT_BIT_SIZE);
	}
	
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		InitializeKey();
		
		if (availableKeys != null)
		{
			EditorGUI.BeginProperty(pos, label, prop);
			DrawPopupMasks(pos, prop, label);
			EditorGUI.EndProperty();
		}
		else
		{
			base.OnGUI(pos, prop, label);
		}
	}
	
	private void InitializeKey()
	{
		if (availableKeys == null)
		{
			StringKeySettings<T> settings = StringKeySettingsSetting.Instance.GetSetting<T>();
			availableKeys = settings.GetKeys();
		}
	}
	
	private void DrawPopupMasks(Rect pos, SerializedProperty prop, GUIContent label)
	{
		SerializedProperty keyArray = prop.FindPropertyRelative("keys");
		int[] oldMasks = GetOldMasks(keyArray);
		int[] newMasks = GetNewMasks(oldMasks, pos, prop, label);
		SetPropertyValue(keyArray, newMasks);
	}
	
	private int[] GetOldMasks(SerializedProperty keyArray)
	{
		int[] oldMasks = new int[GetMaskCount()];
		
		for (int i = 0; i < keyArray.arraySize; ++i)
		{
			string key = keyArray.GetArrayElementAtIndex(i).stringValue;
			int index = availableKeys.IndexOf(key);
			int maskIndex = Mathf.FloorToInt(index / INT_BIT_SIZE);
			int relativeIndex = index % INT_BIT_SIZE;
			oldMasks[maskIndex] += 1 << relativeIndex;
		}
		return oldMasks;
	}
	
	private int[] GetNewMasks(int[] oldMasks, Rect pos, SerializedProperty prop, GUIContent label)
	{
		int[] newMasks = new int[oldMasks.Length];
		if (oldMasks.Length > 1)
		{
			postFix = " 0";
		}
		else
		{
			postFix = "";
		}
		for (int i = 0; i < oldMasks.Length; ++i )
		{
			Rect rect = new Rect(pos.x, pos.y + i * ENTRY_HEIGHT, pos.width, ENTRY_HEIGHT);
			newMasks[i] = EditorGUI.MaskField(
				rect,
				prop.displayName + postFix, oldMasks[i], GetKeysInRange(i * INT_BIT_SIZE, INT_BIT_SIZE)
				);
			postFix = " " + (i + 1);
		}
		return newMasks;		
	}
	
	private string[] GetKeysInRange(int from, int range)
	{
		int to = Mathf.Min(availableKeys.Count, from + range);
		string[] keysInRange = new string[to - from];
		
		int index = 0;
		for (int i = from; i < to; ++i)
		{
			keysInRange[index] = availableKeys[i];
			++index;
		}
		return keysInRange;
	}
	
	
	private void SetPropertyValue(SerializedProperty keyArray, int[] newMasks)
	{
		keyArray.ClearArray();
		for (int i = 0; i < newMasks.Length; ++i)
		{
			SetPropertyValue(keyArray, i * INT_BIT_SIZE, newMasks[i]);
		}
	}
	
	private void SetPropertyValue(SerializedProperty keyArray, int from, int mask)
	{
		for (int i = 0; i < INT_BIT_SIZE; ++i)
		{
			if (
				((mask == -1) || ((mask & (1 << i)) > 0)) &&
				(from + i < availableKeys.Count)
				)
			{
				keyArray.InsertArrayElementAtIndex(0);
				keyArray.GetArrayElementAtIndex(0).stringValue = availableKeys[from + i];
			}
		}
	}
}

} /// namespace JamUtilities
