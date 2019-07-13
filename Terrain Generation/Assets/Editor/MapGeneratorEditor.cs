using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapDisplay))]
public class MapGeneratorEditor : Editor
{

	public override void OnInspectorGUI()
	{
		MapDisplay mapGen = (MapDisplay)target;

		if (DrawDefaultInspector())
		{
			if (mapGen.autoUpdate)
			{
				mapGen.GenerateMap();
			}
		}

		if (GUILayout.Button("Generate"))
		{
			mapGen.GenerateMap();
		}
	}
}