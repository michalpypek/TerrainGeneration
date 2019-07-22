using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LODInfo
{
	public int lod;
	public float viewDistanceThreshold;
	public bool useForCollision;
}
