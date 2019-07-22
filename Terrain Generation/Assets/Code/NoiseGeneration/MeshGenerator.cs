using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateMesh(float[,] heightMap, MeshSettings settings, int levelOfDetail)
	{
		AnimationCurve heightCurve = new AnimationCurve(settings.heightMultiplicationCurve.keys);
		int simplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

		int borderSize = heightMap.GetLength(0);
		int meshSize = borderSize - 2 * simplificationIncrement;
		int meshSizeUnsimplified = borderSize - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

		int verticesPerLine = (meshSize - 1) / simplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine);
		int[,] vertexIndicesMap = new int[borderSize, borderSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = -1;

		for (int y = 0; y < borderSize; y += simplificationIncrement)
		{
			for (int x = 0; x < borderSize; x += simplificationIncrement)
			{
				bool isBorderVertex = y == 0 || y == borderSize - 1 || x == 0 || x == borderSize - 1;

				if (isBorderVertex)
				{
					vertexIndicesMap[x, y] = borderVertexIndex;
					borderVertexIndex--;
				}
				else
				{
					vertexIndicesMap[x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}


		for (int y = 0; y < borderSize; y += simplificationIncrement)
		{
			for (int x = 0; x < borderSize; x += simplificationIncrement)
			{
				int vertexIndex = vertexIndicesMap[x, y];
				Vector2 uv = new Vector2((x - simplificationIncrement) / (float)meshSize, (y - simplificationIncrement) / (float)meshSize);
				float height = heightCurve.Evaluate(heightMap[x, y]) * settings.heightMultiplier;

				Vector3 vertexPosition = new Vector3(topLeftX + uv.x * meshSizeUnsimplified, height, topLeftZ - uv.y * meshSizeUnsimplified);

				meshData.AddVertex(vertexPosition, uv, vertexIndex);

				if (x < borderSize - 1 && y < borderSize - 1)
				{
					int a = vertexIndicesMap[x, y];
					int b = vertexIndicesMap[x + simplificationIncrement, y];
					int c = vertexIndicesMap[x, y + simplificationIncrement];
					int d = vertexIndicesMap[x + simplificationIncrement, y + simplificationIncrement];

					meshData.AddTriangle(a,d,c);
					meshData.AddTriangle(a,b,d);
				}

				vertexIndex++;
			}
		}
		meshData.BakeNormals();

		return meshData;
	}
}
