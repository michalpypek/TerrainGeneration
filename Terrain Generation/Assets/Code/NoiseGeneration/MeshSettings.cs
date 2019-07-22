﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain Generation/Mesh Settings")]
public class MeshSettings : UpdatableData
{
	public bool useFalloff;

	public float heightMultiplier;
	public AnimationCurve heightMultiplicationCurve;
	[Range(0,6)]
	public int lod = 0;

	public float GetHeight(float heightMapValue)
	{
		return heightMultiplicationCurve.Evaluate(heightMapValue) * heightMultiplier;
	}
}
