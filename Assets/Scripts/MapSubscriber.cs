using UnityEngine;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;

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


    //manually set the offset
    private Vector3 ManualOffset = new Vector3(0, 0, 0);

    [SerializeField]
    private float x;

    [SerializeField]
    private float z;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(mapTopic, MapCallback);
    }

    private void Update()
    {
        ManualOffset = new Vector3(x, 0, z);
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

                    GameObject obj = Instantiate(obstaclePrefab, rotatedPos + ManualOffset, Quaternion.identity, mapParent);
                    obj.transform.localScale = new Vector3(mapResolution, obstacleHeight, mapResolution);

                    spawnedObstacles.Add(obj);
                }
            }
        }
    }
}
