using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 100;          // Width of the terrain grid
    public int height = 100;         // Height of the terrain grid
    public float scale = 10f;        // Scale of the Perlin noise
    public float heightMultiplier = 10f;  // Multiplier for the terrain height
    public float offset = 0f;        // Offset for the Perlin noise
    [SerializeField] private GameObject blockPrefab;

    private void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (float)x / width * scale + offset;
                float sampleY = (float)y / height * scale + offset;

                float heightValue = Mathf.PerlinNoise(sampleX, sampleY) * heightMultiplier;

                Vector3 position = new Vector3(x, heightValue, y);

                // Instantiate your block prefab or create your own terrain representation here
                // Example: Instantiate(blockPrefab, position, Quaternion.identity);
                Instantiate(blockPrefab, position, Quaternion.identity);
            }
        }
    }
}
