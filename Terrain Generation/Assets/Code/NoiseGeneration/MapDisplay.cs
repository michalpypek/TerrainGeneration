using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapDisplay : MonoBehaviour
{
	public int mapWidth;
	public int mapHeight;
	public NoiseSettings settings;
	public Renderer heightMapRenderer;
	public Renderer colorMapRenderer;
	[SerializeField]
	private List<HeightToColor> colorMappers;

	public bool autoUpdate;

	public void GenerateMap()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, settings);

		Color[] colorMap = new Color[mapWidth * mapHeight];

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float height = noiseMap[x, y];
				var col = colorMappers.FirstOrDefault(cm => cm.IsInRange(height)).color;
				colorMap[y * mapWidth + x] = col;
			}
		}

		DrawNoiseMap(noiseMap);
		DrawColorMap(colorMap, mapWidth, mapHeight);
	}

	public void DrawNoiseMap(float[,] noiseMap)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		Texture2D texture = new Texture2D(width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
			}
		}
		texture.SetPixels(colourMap);
		texture.Apply();

		heightMapRenderer.sharedMaterial.mainTexture = texture;
		heightMapRenderer.transform.localScale = new Vector3(width, 1, height);
	}

	public void DrawColorMap(Color[] colors, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.SetPixels(colors);
		texture.Apply();
		colorMapRenderer.sharedMaterial.mainTexture = texture;
		colorMapRenderer.transform.localScale = new Vector3(width, 1, height);
	}

	void OnValidate()
	{
		if (mapWidth < 1)
		{
			mapWidth = 1;
		}
		if (mapHeight < 1)
		{
			mapHeight = 1;
		}
		settings.ValidateValues();
	}
}
