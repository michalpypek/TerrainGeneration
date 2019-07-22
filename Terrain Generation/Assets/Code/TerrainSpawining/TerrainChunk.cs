using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainChunk
{
	private GameObject meshObject;
	private Vector2 position;
	private Bounds bounds;

	private MapData mapData;
	private bool hasMapData;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;

	private LODInfo[] detailLevels;
	private LODMesh[] lodMeshes;
	private LODMesh collisionLODMesh;

	private int previousLod = -1;

	public TerrainChunk(Vector2 coord, int size,LODInfo[] detailLevels, Material mat)
	{
		position = coord * size;
		bounds = new Bounds(position, Vector2.one * size);
		Vector3 worldPos = new Vector3(position.x, 0, position.y);

		meshObject = new GameObject($"Chunk {position}");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = mat;
		meshObject.transform.position = worldPos * TerrainChunkGenerator.get.TerrainScale;
		meshObject.transform.localScale = Vector3.one * TerrainChunkGenerator.get.TerrainScale;

		this.detailLevels = detailLevels;
		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < lodMeshes.Length; i++)
		{
			lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateChunk);
			if(detailLevels[i].useForCollision)
			{
				collisionLODMesh = lodMeshes[i];
			}
		}

		SetVisible(false);
		MapGenerator.get.RequestMapData(OnMapDataReceived, position);
	}

	void OnMapDataReceived(MapData mapData)
	{
		this.mapData = mapData;
		hasMapData = true;

		UpdateChunk();
	}

	void OnMeshDataReceived(MeshData meshData)
	{
		meshFilter.mesh = meshData.GetMesh();
	}

	public void UpdateChunk()
	{
		if (hasMapData == false)
			return;

		var viewerPos = TerrainChunkGenerator.get.ViewerPos;
		var maxViewDistance = TerrainChunkGenerator.get.MaxViewDistance;

		float viewerDsistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
		bool visible = viewerDsistFromNearestEdge <= maxViewDistance;

		if (visible)
		{
			int lodIndex = 0;
			for (int i = 0; i < detailLevels.Length - 1; i++)
			{
				if (viewerDsistFromNearestEdge > detailLevels[i].viewDistanceThreshold)
					lodIndex = i + 1;
				else
					break;
			}
			if(lodIndex != previousLod)
			{
				var lodMesh = lodMeshes[lodIndex];
				if (lodMesh.hasMesh)
				{
					previousLod = lodIndex;
					meshFilter.mesh = lodMesh.mesh;
				}
				else if (lodMesh.requestedMesh == false)
					lodMesh.RequestMeshData(mapData);
			}

			if(lodIndex == 0)
			{
				if(collisionLODMesh.hasMesh)
				{
					meshCollider.sharedMesh = collisionLODMesh.mesh;
				}
				else if (!collisionLODMesh.requestedMesh)
				{
					collisionLODMesh.RequestMeshData(mapData);
				}
			}
		}

		SetVisible(visible);
	}

	public void SetVisible(bool visible)
	{
		meshObject.SetActive(visible);
		if(visible)
		{
			TerrainChunkGenerator.get.OnChunkBecameVisible(this);
		}
	}

	public bool IsVisible()
	{
		return meshObject.activeSelf;
	}

}

