using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData
{
	public float[,] heightMap;
	public Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap)
	{
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}
