using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

public class MapGenerator : Singleton<MapGenerator>
{
	public bool autoUpdate;

	public const int chunkSize = 239;

	[SerializeField]
	private MeshSettings meshSettings;
	[SerializeField]
	private NoiseSettings noiseSettings;
	[SerializeField]
	private TextureData textureData;
	[SerializeField]
	private Material terrainMaterial;
	[SerializeField]
	private Renderer heightMapRenderer;
	[SerializeField]
	private Renderer colorMapRenderer;
	[SerializeField]
	private GameObject meshRenderer;

	private float[,] falloffMap;

	private Queue<GeneratorThreadInfo<MapData>> mapThreadCallbacksQueue = new Queue<GeneratorThreadInfo<MapData>>();
	private Queue<GeneratorThreadInfo<MeshData>> meshThreadCallbacksQueue = new Queue<GeneratorThreadInfo<MeshData>>();

	public void DrawMaps()
	{
		MapData mapData = GenerateMapData(Vector2.zero);
		falloffMap = Noise.GenerateFalloffMap(chunkSize + 2);
		var heightTex = TextureGenerator.NoiseToTexture(mapData.heightMap);

		SetTextureAndSize(heightMapRenderer, heightTex);

		var meshD = MeshGenerator.GenerateMesh(mapData.heightMap, meshSettings, meshSettings.lod);
		DrawMesh(meshRenderer, meshD);
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

	private void OnValuesUpdated()
	{
		if (!Application.isPlaying)
		{
			DrawMaps();
		}
	}

	private void OnTextureValuesUpdated()
	{
		textureData.ApplyToMaterial(terrainMaterial);
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
		float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize + 2 , chunkSize + 2, noiseSettings, center);

		if(meshSettings.useFalloff)
		{
			if(falloffMap == null)
			{
				falloffMap = Noise.GenerateFalloffMap(chunkSize + 2);
			}
		}

		for (int y = 0; y < chunkSize + 2; y++)
		{
			for (int x = 0; x < chunkSize + 2; x++)
			{
				if(meshSettings.useFalloff)
				{
					noiseMap[x, y] =  Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}
			}
		}

		return new MapData(noiseMap);
	}

	private void SetTextureAndSize(Renderer toSet, Texture2D texture)
	{
		toSet.sharedMaterial.mainTexture = texture;
		toSet.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	private void DrawMesh(GameObject meshObj, MeshData meshData)
	{
		var meshFilter = meshObj.GetComponent<MeshFilter>();
		var meshRenderer = meshObj.GetComponent<MeshRenderer>();

		meshFilter.sharedMesh = meshData.GetMesh();
	}

	void OnValidate()
	{

		if (meshSettings != null)
		{
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (noiseSettings != null)
		{
			noiseSettings.OnValuesUpdated -= OnValuesUpdated;
			noiseSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}

	}

}
