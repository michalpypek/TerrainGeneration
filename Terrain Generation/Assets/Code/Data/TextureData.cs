using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Terrain generation/TextureData")]
public class TextureData : UpdatableData
{
	[SerializeField]
	private Color[] baseColors;
	[SerializeField]
	[Range(0,1)]
	private float[] baseStartHeights;

	private float savedMinHeight;
	private float savedMaxHeight;

	public void ApplyToMaterial(Material mat)
	{
		mat.SetInt("baseColorCount", baseColors.Length);
		mat.SetColorArray("baseColors", baseColors);
		mat.SetFloatArray("baseStartHeights", baseStartHeights);

		UpdateMeshHeights(mat, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material mat, float minHeight, float maxHeight)
	{
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		mat.SetFloat("minHeight", minHeight);
		mat.SetFloat("maxHeight", maxHeight);
	}
}
