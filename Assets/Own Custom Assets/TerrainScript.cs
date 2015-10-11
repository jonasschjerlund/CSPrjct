using UnityEngine;
using System.Collections;

public class TerrainScript : MonoBehaviour
{
	
	// Initiate variables
	TerrainData TD;
	public float hillVariance = 10.0f;

	// Method runs on program launch
	void Start ()
	{

		// Fetch terrain data of current playing field
		TD = transform.GetComponent<Terrain> ().terrainData;

		printTerrainSize();

		// Creates 2-dimensional array of floats that is the size of the width and height of the heightmap and contains the current height values
		float[,] heightMap = TD.GetHeights (0, 0, TD.heightmapWidth, TD.heightmapHeight);

		for (int x = 0; x < TD.heightmapWidth; x++) {
			for (int y = 0; y < TD.heightmapHeight; y++) {

				// Standard formula to apply perlin noise to 3D terrain from http://wiki.unity3d.com/index.php/TerrainPerlinNoise
				// By using the width and height on each position we ensure that adjacent positions are adjusted smoothly
				heightMap [x, y] = Mathf.PerlinNoise (((float)x / (float)TD.heightmapWidth) * hillVariance, ((float)y / (float)TD.heightmapHeight) * hillVariance) / 10.0f;

			}

		}
		
		// Pushes the new height map to the terrain data
		TD.SetHeights (0, 0, heightMap);
		

	}

	// Small method which logs the resolution of the active heightmap for troubleshooting
	void printTerrainSize() {
		Debug.Log ("X size: " + TD.heightmapWidth);
		Debug.Log ("Y size: " + TD.heightmapHeight);
	}
}