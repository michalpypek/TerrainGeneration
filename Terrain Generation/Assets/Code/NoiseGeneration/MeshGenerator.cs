using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateMesh(float[,] heightMap, MeshSettings settings, int levelOfDetail)
	{
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);

		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int simplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / simplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y+= simplificationIncrement)
		{
			for (int x = 0; x < width; x+= simplificationIncrement)
			{
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, settings.GetHeight(heightMap[x,y]), topLeftZ - y);
				meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1)
				{
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + verticesPerLine + 1);
				}

				vertexIndex++;
			}
		}

		return meshData;
	}
}
