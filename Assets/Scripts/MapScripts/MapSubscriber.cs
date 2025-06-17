using UnityEngine;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using Unity.Robotics.ROSTCPConnector.ROSGeometry; // for From<FLU>()
using UnityEngine.AI;

public class MapSubscriber : MonoBehaviour
{
    public string mapTopic = "/map";
    public GameObject obstaclePrefab;
    public float obstacleHeight = 0.1f;
    public int occupancyThreshold = 50;

    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private float mapResolution;
    private int mapWidth;
    private int mapHeight;
    private Vector3 origin;
    private Quaternion rotation;

    private NavMeshSurface surface;

    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(mapTopic, MapCallback);
    }

    void MapCallback(OccupancyGridMsg mapMsg)
    {
        // Read map metadata
        mapResolution = mapMsg.info.resolution;
        mapWidth = (int)mapMsg.info.width;
        mapHeight = (int)mapMsg.info.height;

        // Origin and rotation from message
        origin = mapMsg.info.origin.position.From<FLU>();
        rotation = mapMsg.info.origin.orientation.From<FLU>();

        // Apply -90 deg to match ROS to Unity visual convention
        rotation *= Quaternion.Euler(0, -90, 0);

        // Half-cell offset
        Vector3 drawOrigin = origin - rotation * new Vector3(mapResolution * 0.5f, 0, mapResolution * 0.5f);

        // Set position/rotation of this parent GameObject
        transform.position = drawOrigin;
        transform.rotation = rotation;

        GenerateObstacles(mapMsg.data);
    }

    void GenerateObstacles(sbyte[] data)
    {
        // Clear previous obstacles
        foreach (var obj in spawnedObstacles)
        {
            Destroy(obj);
        }
        spawnedObstacles.Clear();

        int count = 0;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = x + y * mapWidth;
                sbyte value = data[i];

                if (value >= occupancyThreshold)
                {
                    // Position each cube centered in its cell
                    Vector3 localPos = new Vector3(
                        x * mapResolution + mapResolution / 2f,
                        0,
                        y * mapResolution + mapResolution / 2f
                    );

                    GameObject obj = Instantiate(obstaclePrefab, transform);
                    obj.transform.localPosition = localPos;
                    obj.transform.localScale = new Vector3(mapResolution, obstacleHeight, mapResolution);

                    spawnedObstacles.Add(obj);
                    count++;
                }
            }
        }

    }
}