using UnityEngine;

public class TextureToObstacles : MonoBehaviour
{
    public Texture2D mapTexture;
    public GameObject obstaclePrefab;
    public float cellSize = 1f;
    public float heightOffset = 0f;

    void GenerateObstacles()
    {
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                Color pixel = mapTexture.GetPixel(x, y);

                if (pixel.grayscale < 0.5f)
                {
                    Vector3 position = new Vector3(x * cellSize, heightOffset, y * cellSize);
                    Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
                }
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GenerateObstacles();
        }
    }
}