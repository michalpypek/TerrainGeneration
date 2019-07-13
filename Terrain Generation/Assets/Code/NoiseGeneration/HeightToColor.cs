using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HeightToColor 
{
	public float minHeight;
	public float maxHeight;
	public Color color;

	public bool IsInRange(float height)
	{
		return height <= maxHeight && height >= minHeight;
	}
}
