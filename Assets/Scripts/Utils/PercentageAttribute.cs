using UnityEngine;
using System.Collections.Generic;

public class PercentageAttribute : PropertyAttribute 
{
	public readonly bool isConfinedToRange;
	public readonly float minPercentage;
	public readonly float maxPercentage;

	public PercentageAttribute()
	{
		isConfinedToRange = false;
	}

	public PercentageAttribute(float min, float max)
	{
		isConfinedToRange = true;
		minPercentage = min * 100.0f;
		maxPercentage = max * 100.0f;
	}
}
