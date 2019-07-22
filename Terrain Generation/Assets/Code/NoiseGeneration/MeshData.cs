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

	private Vector3[] CalculateNormals()
	{
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;

		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		for (int i = 0; i < vertexNormals.Length; i++)
		{
			vertexNormals[i].Normalize();
		}

		return vertexNormals;
	}

	private Vector3 SurfaceNormalFromIndices(int indA, int indB, int indC)
	{
		var vertA = vertices[indA];
		var vertB = vertices[indB];
		var vertC = vertices[indC];

		var edgeAB = vertB - vertA;
		var edgeAC = vertC - vertA;

		return Vector3.Cross(edgeAB, edgeAC).normalized;
	}

	public Mesh GetMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.normals = CalculateNormals();
		mesh.RecalculateNormals();
		return mesh;
	}
}
