using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	private Vector3[] bakedNormals;
	private Vector3[] vertices;
	private Vector2[] uvs;
	private int[] triangles;
	private Vector3[] borderVertices;
	private int[] borderTriangles;

	private int lastTriangleIndex = 0;
	private int borderTriangleIndex;

	public MeshData(int verticesPerLine)
	{
		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

		borderVertices = new Vector3[verticesPerLine * 4 + 4];
		borderTriangles = new int[24 * verticesPerLine];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		//is border index
		if (vertexIndex < 0)
		{
			borderVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			vertices[vertexIndex] = vertexPosition;
			uvs[vertexIndex] = uv;
		}
	}

	public void AddTriangle(int v1, int v2, int v3)
	{
		if (v1 < 0 || v2 < 0 || v3 < 0)
		{
			borderTriangles[borderTriangleIndex] = v1;
			borderTriangles[borderTriangleIndex + 1] = v2;
			borderTriangles[borderTriangleIndex + 2] = v3;
			borderTriangleIndex += 3;
		}
		else
		{
			triangles[lastTriangleIndex] = v1;
			triangles[lastTriangleIndex + 1] = v2;
			triangles[lastTriangleIndex + 2] = v3;
			lastTriangleIndex += 3;
		}
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

		int borderTriangleCount = borderTriangles.Length / 3;

		for (int i = 0; i < borderTriangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if (vertexIndexA >= 0)
				vertexNormals[vertexIndexA] += triangleNormal;
			if (vertexIndexB >= 0)
				vertexNormals[vertexIndexB] += triangleNormal;
			if (vertexIndexC >= 0)
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
		var vertA = indA < 0 ? borderVertices[-indA - 1] : vertices[indA];
		var vertB = indB < 0 ? borderVertices[-indB - 1] : vertices[indB];
		var vertC = indC < 0 ? borderVertices[-indC - 1] : vertices[indC];

		var edgeAB = vertB - vertA;
		var edgeAC = vertC - vertA;

		return Vector3.Cross(edgeAB, edgeAC).normalized;
	}

	public void BakeNormals()
	{
		bakedNormals = CalculateNormals();
	}

	public Mesh GetMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		//mesh.normals = bakedNormals;
		mesh.RecalculateNormals();
		return mesh;
	}
}
