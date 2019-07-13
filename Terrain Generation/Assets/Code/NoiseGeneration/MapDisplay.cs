using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapDisplay : MonoBehaviour
{
	public bool autoUpdate;

	[SerializeField]
	private int mapWidth;
	[SerializeField]
	private int mapHeight;
	[SerializeField]
	private NoiseSettings settings;
	[SerializeField]
	private Renderer heightMapRenderer;
	[SerializeField]
	private Renderer colorMapRenderer;
	[SerializeField]
	private GameObject meshRenderer;
	[SerializeField]
	private List<HeightToColor> colorMappers;

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

		var heightTex = TextureGenerator.NoiseToTexture(noiseMap);
		var colorTex = TextureGenerator.ColorsToTexture(colorMap, mapWidth, mapHeight);

		SetTextureAndSize(heightMapRenderer, heightTex);
		SetTextureAndSize(colorMapRenderer, colorTex);

		var meshD = MeshGenerator.GenerateMesh(noiseMap);
		DrawMesh(meshRenderer, meshD, colorTex);
	}

	private void SetTextureAndSize(Renderer toSet, Texture2D texture)
	{
		toSet.sharedMaterial.mainTexture = texture;
		toSet.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	private void DrawMesh(GameObject meshObj, MeshData meshData, Texture2D texture)
	{
		var meshFilter = meshObj.GetComponent<MeshFilter>();
		var meshRenderer = meshObj.GetComponent<MeshRenderer>();

		meshFilter.sharedMesh = meshData.GetMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
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
