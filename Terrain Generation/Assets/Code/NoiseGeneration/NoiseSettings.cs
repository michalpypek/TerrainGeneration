using UnityEngine;

[CreateAssetMenu(menuName = "Terrain Generation/NoiseSettings")]
public class NoiseSettings : UpdatableData
{
	public float scale = 50;
	public bool enabled = true;
	[Tooltip("Number of noise layers")]
	public int octaves = 6;
	[Tooltip("Controls decrease in amplitude of octaves")]
	[Range(0, 1)]
	public float persistance = .6f;
	[Tooltip("controls increase in frequency of octaves")]
	public float lacunarity = 2;

	public int seed;
	public Vector2 offset;

	public void ValidateValues()
	{
		scale = Mathf.Max(scale, 0.01f);
		octaves = Mathf.Max(octaves, 1);
		lacunarity = Mathf.Max(lacunarity, 1);
		persistance = Mathf.Clamp01(persistance);
	}
}