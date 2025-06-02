using UnityEngine;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Nav;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class MapSubscriber : MonoBehaviour
{
    public string mapTopic = "/map";
    public GameObject obstaclePrefab;
    public float obstacleHeight = 0.1f;
    public Transform mapParent;

    private float mapResolution;
    private int mapWidth;
    private int mapHeight;
    private Quaternion mapRotation;
    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(mapTopic, MapCallback);
    }

    void MapCallback(OccupancyGridMsg mapMsg)
    {
        mapResolution = mapMsg.info.resolution;
        mapWidth = (int)mapMsg.info.width;
        mapHeight = (int)mapMsg.info.height;

        mapRotation = Quaternion.Euler(0f, 270f, 0f);

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

        Vector3 centerOffset = new Vector3(mapWidth * mapResolution / 2f, 0, mapHeight * mapResolution / 2f);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = x + y * mapWidth; //matrix to array position in map data
                sbyte value = data[i];

                if (value > 50) //black
                {
                    Vector3 localPos = new Vector3(x * mapResolution, 0, y * mapResolution);
                    Vector3 centeredPos = localPos - centerOffset;
                    Vector3 rotatedPos = mapRotation * centeredPos;

                    GameObject obj = Instantiate(obstaclePrefab, rotatedPos, Quaternion.identity, mapParent);
                    obj.transform.localScale = new Vector3(mapResolution, obstacleHeight, mapResolution);

                    spawnedObstacles.Add(obj);
                }
            }
        }
    }
}
