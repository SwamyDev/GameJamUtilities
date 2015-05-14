using UnityEngine;
using System.Collections.Generic;

public static class VectorExtensions
{
	public static bool Approximately(this Vector2 lhs, Vector2 rhs)
	{
		return Mathf.Approximately(lhs.x, rhs.x)
			&& Mathf.Approximately(lhs.y, rhs.y);
	}

	public static bool Approximately(this Vector3 lhs, Vector3 rhs)
	{
		return Mathf.Approximately(lhs.x, rhs.x)
			&& Mathf.Approximately(lhs.y, rhs.y)
			&& Mathf.Approximately(lhs.z, rhs.z);
	}

	public static bool Approximately(this Vector4 lhs, Vector4 rhs)
	{
		return Mathf.Approximately(lhs.x, rhs.x)
			&& Mathf.Approximately(lhs.y, rhs.y)
			&& Mathf.Approximately(lhs.z, rhs.z)
			&& Mathf.Approximately(lhs.w, rhs.w);
	}
}
