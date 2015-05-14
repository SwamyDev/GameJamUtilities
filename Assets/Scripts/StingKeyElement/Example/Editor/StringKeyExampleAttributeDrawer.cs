using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using JamUtilities;

[CustomPropertyDrawer(typeof(StringKeyExampleAttribute))]
public class StringKeyExampleAttributeDrawer : StringKeyAttributeDrawer<StringKeyExample>
{

}
