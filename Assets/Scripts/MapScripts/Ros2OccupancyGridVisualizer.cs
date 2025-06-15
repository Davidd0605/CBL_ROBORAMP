using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Nav;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry; // For From<FLU>() conversion

public class Ros2OccupancyGridVisualizer : MonoBehaviour
{
    public string topicName = "/map";
    public GameObject wallPrefab;
    public int occupancyThreshold = 50;

    private ROSConnection ros;
    private List<GameObject> cubes = new List<GameObject>();
    private int mapWidth = 0;
    private int mapHeight = 0;
    private float cellSize = 1f; // will be set from message

    private Vector3 origin;
    private Quaternion rotation;
    private Vector3 offset = Vector3.zero; // can expose this if you want to tweak

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OccupancyGridMsg>(topicName, ReceiveMap);
    }

    void ReceiveMap(OccupancyGridMsg map)
    {
        mapWidth = (int)map.info.width;
        mapHeight = (int)map.info.height;
        cellSize = (float)map.info.resolution;

        sbyte[] data = map.data;

        // Match origin and rotation exactly as OccupancyGridDefaultVisualizer does:
        origin = map.info.origin.position.From<FLU>();
        rotation = map.info.origin.orientation.From<FLU>();
        rotation *= Quaternion.Euler(0, -90, 0);

        // Adjust origin with half cell offset, like the other script:
        Vector3 drawOrigin = origin - rotation * new Vector3(cellSize * 0.5f, 0, cellSize * 0.5f) + offset;

        // Apply to this GameObject's transform (parent for cubes)
        transform.position = drawOrigin;
        transform.rotation = rotation;

        UpdateMapVisualization(data, mapWidth, mapHeight);
    }

    void UpdateMapVisualization(sbyte[] data, int width, int height)
    {
        int requiredCubes = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = x + y * width;
                sbyte val = data[i];

                if (val >= occupancyThreshold)
                {
                    GameObject cube;
                    if (requiredCubes < cubes.Count)
                    {
                        cube = cubes[requiredCubes];
                        cube.SetActive(true);
                    }
                    else
                    {
                        cube = Instantiate(wallPrefab, this.transform);
                        cubes.Add(cube);
                    }

                    // Position cubes so each cube is centered on its corresponding pixel
                    // since origin is already offset, just place cubes at cell centers:
                    cube.transform.localPosition = new Vector3(
                        x * cellSize + cellSize / 2f,
                        cellSize / 2f, // to sit on ground, assuming cube height == cellSize
                        y * cellSize + cellSize / 2f
                    );

                    cube.transform.localScale = new Vector3(cellSize, wallPrefab.transform.localScale.y, cellSize);

                    requiredCubes++;
                }
            }
        }

        for (int i = requiredCubes; i < cubes.Count; i++)
        {
            cubes[i].SetActive(false);
        }
    }
}