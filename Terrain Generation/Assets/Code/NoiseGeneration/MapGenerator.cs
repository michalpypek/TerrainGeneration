using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

public class MapGenerator : Singleton<MapGenerator>
{
	public bool autoUpdate;

	public const int chunkSize = 241;

	[SerializeField]
	private MeshSettings meshSettings;
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

	private Queue<GeneratorThreadInfo<MapData>> mapThreadCallbacksQueue = new Queue<GeneratorThreadInfo<MapData>>();
	private Queue<GeneratorThreadInfo<MeshData>> meshThreadCallbacksQueue = new Queue<GeneratorThreadInfo<MeshData>>();

	public void DrawMaps()
	{
		MapData mapData = GenerateMapData(Vector2.zero);
		var heightTex = TextureGenerator.NoiseToTexture(mapData.heightMap);
		var colorTex = TextureGenerator.ColorsToTexture(mapData.colorMap, chunkSize, chunkSize);

		SetTextureAndSize(heightMapRenderer, heightTex);
		SetTextureAndSize(colorMapRenderer, colorTex);

		var meshD = MeshGenerator.GenerateMesh(mapData.heightMap, meshSettings, meshSettings.lod);
		DrawMesh(meshRenderer, meshD, colorTex);
	}

	public void RequestMapData(Action<MapData> callback, Vector2 center)
	{
		ThreadStart threadStart = delegate
		{
			MapDataThread(callback, center);
		};

		new Thread(threadStart).Start();
	}

	public void RequestMeshData(MapData mapData, Action<MeshData> callback, int lod)
	{
		ThreadStart threadStart = delegate
		{
			MeshDataThread(mapData, callback, lod);
		};

		new Thread(threadStart).Start();
	}

	private void MeshDataThread(MapData mapData, Action<MeshData> callback, int lod)
	{
		MeshData data = MeshGenerator.GenerateMesh(mapData.heightMap, meshSettings, lod);
		lock(meshThreadCallbacksQueue)
		{
			meshThreadCallbacksQueue.Enqueue(new GeneratorThreadInfo<MeshData>(callback, data));
		}
	}

	private void MapDataThread(Action<MapData> callback, Vector2 center)
	{
		MapData mapData = GenerateMapData(center);
		lock (mapThreadCallbacksQueue)
		{
			mapThreadCallbacksQueue.Enqueue(new GeneratorThreadInfo<MapData>(callback, mapData));
		}
	}

	private void Update()
	{
		CheckThreadQueues();
	}

	private void CheckThreadQueues()
	{
		if (mapThreadCallbacksQueue.Count > 0)
		{
			var threadInfo = mapThreadCallbacksQueue.Dequeue();
			threadInfo.callback(threadInfo.parameter);
		}

		if(meshThreadCallbacksQueue.Count > 0)
		{
			var threadInfo = meshThreadCallbacksQueue.Dequeue();
			threadInfo.callback(threadInfo.parameter);
		}
	}

	private MapData GenerateMapData(Vector2 center)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, settings, center);

		Color[] colorMap = new Color[chunkSize * chunkSize];

		for (int y = 0; y < chunkSize; y++)
		{
			for (int x = 0; x < chunkSize; x++)
			{
				float height = noiseMap[x, y];
				var col = colorMappers.FirstOrDefault(cm => cm.IsInRange(height)).color;
				colorMap[y * chunkSize + x] = col;
			}
		}

		return new MapData(noiseMap, colorMap);
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
		settings.ValidateValues();
	}
}
