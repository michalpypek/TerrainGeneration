using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshSettings
{
	public float heightMultiplier;
	public AnimationCurve heightMultiplicationCurve;

	public float GetHeight(float heightMapValue)
	{
		return heightMultiplicationCurve.Evaluate(heightMapValue) * heightMultiplier;
	}
}
