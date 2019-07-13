using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public Vector3[] vertices;
	public Vector2[] uvs;
	public int[] triangles;

	private int lastTriangleIndex = 0;

	public MeshData(int width, int height)
	{
		vertices = new Vector3[width * height];
		uvs = new Vector2[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];
	}

	public void AddTriangle(int v1, int v2, int v3)
	{
		triangles[lastTriangleIndex] = v1;
		triangles[lastTriangleIndex + 1] = v2;
		triangles[lastTriangleIndex + 2] = v3;
		lastTriangleIndex += 3;
	}

	public Mesh GetMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		return mesh;
	}
}
