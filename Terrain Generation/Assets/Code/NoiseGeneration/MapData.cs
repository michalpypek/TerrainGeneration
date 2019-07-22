using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData
{
	public float[,] heightMap;

	public MapData(float[,] heightMap)
	{
		this.heightMap = heightMap;
	}
}
