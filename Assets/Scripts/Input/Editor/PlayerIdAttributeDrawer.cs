using UnityEngine;
using UnityEditor;
using System.Collections;

namespace JamUtilities {

[CustomPropertyDrawer(typeof(PlayerIdAttribute))]
public class PlayerIdAttributeDrawer : StringKeyAttributeDrawer<PlayerData>
{
	
}

}	// namespace JamUtilities