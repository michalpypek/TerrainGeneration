using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODMesh
{
	public Mesh mesh;

	public bool requestedMesh;
	public bool hasMesh;
	private int lod;
	private Action onMeshReceived;

	public LODMesh(int lod, Action onMeshReceived)
	{
		this.lod = lod;
		this.onMeshReceived = onMeshReceived;
	}

	public void RequestMeshData(MapData mapData)
	{
		MapGenerator.get.RequestMeshData(mapData, OnMeshDataReceived, lod);
		requestedMesh = true;
	}

	private void OnMeshDataReceived(MeshData meshData)
	{
		mesh = meshData.GetMesh();
		hasMesh = true;
		onMeshReceived();
	}
}
