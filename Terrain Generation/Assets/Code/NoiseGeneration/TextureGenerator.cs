using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
	public static Texture2D NoiseToTexture(float[,] noiseMap)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
			}
		}

		return ColorsToTexture(colorMap, width, height);
	}

	public static Texture2D ColorsToTexture(Color[] colors, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.SetPixels(colors);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Point;
		texture.Apply();

		return texture;
	}
}
