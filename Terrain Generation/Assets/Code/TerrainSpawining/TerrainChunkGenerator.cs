using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkGenerator : Singleton<TerrainChunkGenerator>
{
	public Vector2 ViewerPos => viewerPos;
	public float MaxViewDistance => maxViewDistance;
	public float TerrainScale => terrainScale;

	[SerializeField]
	private float terrainScale = 1f;
	[SerializeField]
	private LODInfo[] detailLevels;
	[SerializeField]
	private Transform viewer;
	[SerializeField]
	private Material mapMaterial;

	private const float minMoveToUpdate = 10f;
	private const float sqrMinMove = minMoveToUpdate * minMoveToUpdate;
	private float maxViewDistance = 300;
	private Vector2 viewerPos;
	private int chunkSize;
	private int chunksVisibleInViewDst;
	private Dictionary<Vector2, TerrainChunk> posToChunk = new Dictionary<Vector2, TerrainChunk>();
	private List<TerrainChunk> lastVisibleChunks = new List<TerrainChunk>();

	public void OnChunkBecameVisible(TerrainChunk chunk)
	{
		lastVisibleChunks.Add(chunk);
	}

	private void Start()
	{
		chunkSize = MapGenerator.chunkSize - 1;
		maxViewDistance = detailLevels[detailLevels.Length - 1].viewDistanceThreshold;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDistance / chunkSize);
		UpdateVisibleChunks();
	}

	private void Update()
	{
		var viewerPosOld = viewerPos;
		viewerPos = new Vector2(viewer.position.x, viewer.position.z) / terrainScale;
		if (Vector2.SqrMagnitude(viewerPos - viewerPosOld) >= sqrMinMove)
		{
			UpdateVisibleChunks();
		}
	}

	private void UpdateVisibleChunks()
	{
		lastVisibleChunks.ForEach(chunk => chunk.SetVisible(false));

		int currentXCoord = Mathf.RoundToInt(viewerPos.x / chunkSize);
		int currentYCoord = Mathf.RoundToInt(viewerPos.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentXCoord + xOffset, currentYCoord + yOffset);

				if (posToChunk.ContainsKey(viewedChunkCoord))
				{
					var chunk = posToChunk[viewedChunkCoord];
					chunk.UpdateChunk();
				}
				else
				{
					posToChunk[viewedChunkCoord] = new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, mapMaterial);
				}

			}
		}
	}
}
