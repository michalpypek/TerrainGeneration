using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Terrain generation/TextureData")]
public class TextureData : UpdatableData
{
	[SerializeField]
	private Layer[] layers;

	private float savedMinHeight;
	private float savedMaxHeight;

	public void ApplyToMaterial(Material mat)
	{
		var tints = layers.Select(l => l.tint).ToArray();
		var tintStrengths = layers.Select(l => l.tintStrength).ToArray();
		var startHeights = layers.Select(l => l.startHeight).ToArray();
		var blendStrengths = layers.Select(l => l.blendStrength).ToArray();
		var textureScales = layers.Select(l => l.textureScale).ToArray();
		var textureArray = GenerateTextureArray(layers.Select(l => l.texture).ToArray());

		mat.SetInt("layerCount", layers.Length);
		mat.SetColorArray("baseColors", tints);
		mat.SetFloatArray("baseStartHeights", startHeights);
		mat.SetFloatArray("baseBlends", blendStrengths);
		mat.SetFloatArray("baseColorStrength", tintStrengths);
		mat.SetFloatArray("baseTextureScales", textureScales);
		mat.SetTexture("baseTextures", textureArray);

		UpdateMeshHeights(mat, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material mat, float minHeight, float maxHeight)
	{
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		mat.SetFloat("minHeight", minHeight);
		mat.SetFloat("maxHeight", maxHeight);
	}

	private Texture2DArray GenerateTextureArray(Texture2D[] textures)
	{
		Texture2DArray textureArray = new Texture2DArray(512, 512, textures.Length, TextureFormat.RGB565, true);
		for (int i = 0; i < textures.Length; i++)
		{
			textureArray.SetPixels(textures[i].GetPixels(), i);
		}

		textureArray.Apply();
		return textureArray;
	}

	[Serializable]
	public class Layer
	{
		public Texture2D texture;
		public Color tint;
		[Range(0, 1)]
		public float tintStrength;
		[Range(0, 1)]
		public float startHeight;
		[Range(0, 1)]
		public float blendStrength;
		public float textureScale;
	}
}
