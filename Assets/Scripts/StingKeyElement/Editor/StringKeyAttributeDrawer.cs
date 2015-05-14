using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace JamUtilities
{

public class StringKeyAttributeDrawer<T> : PropertyDrawer where T : IStringKeyElement
{
	private const string UNSELECTED = "UNSELECTED";
	private const string UNSELECTED_KEY = "";
	
	private string[] availableKeys;
	private SerializedProperty serializedProperty;
	private string value;
	
	private void InitializeKey()
	{
		if (availableKeys == null)
		{
			StringKeySettings<T> settings = StringKeySettingsSetting.Instance.GetSetting<T>();
			List<string> keys = settings.GetKeys();
			keys.Insert(0,UNSELECTED);
			availableKeys = keys.ToArray();
		}
	}
	
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		InitializeKey();
		
		if (availableKeys != null)
		{
			EditorGUI.BeginProperty(pos, label, prop);
			DrawPopup(pos, prop, label);
			EditorGUI.EndProperty();
		}
		else
		{
			base.OnGUI(pos, prop, label);
		}
	}
	
	private void DrawPopup(Rect position, SerializedProperty prop, GUIContent label)
	{
		serializedProperty = prop.FindPropertyRelative("key");
		value = serializedProperty.stringValue;
		int oldIndex = GetOldIndex();
		int newIndex = EditorGUI.Popup(position, label.text, oldIndex, availableKeys);
		SetIndex(oldIndex, newIndex);
	}

	private int GetOldIndex()
	{
		for (int i = 0; i < availableKeys.Length; ++i)
		{
			if (value.Equals(availableKeys[i]))
			{
				return i;
			}
		}
		return 0;
	}
	
	private void SetIndex(int oldIndex, int newIndex)
	{
		if ((oldIndex != newIndex) && (newIndex < availableKeys.Length))
		{
			if (newIndex == 0)
			{
				serializedProperty.stringValue = UNSELECTED_KEY;
			}
			else
			{
				serializedProperty.stringValue = availableKeys[newIndex];
			}
		}
	}
}

} /// namespace JamUtilities
