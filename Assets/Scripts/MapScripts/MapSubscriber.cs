 using UnityEngine;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using UnityEngine.AI;
public class MapSubscriber : MonoBehaviour
{
    public string mapTopic = "/map";
    public GameObject obstaclePrefab;
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    public float obstacleHeight = 0.1f;

    private NavMeshSurface surface;
    private float mapResolution;
    private int mapWidth;
    private int mapHeight;
    private Quaternion mapRotation;

    //Run time editing interface for map offset
    [SerializeField]
    private float deg = 270.0f;

    [SerializeField]
    private float xOffset = 0;

    [SerializeField]
    private float zOffset = 0;

    //Map center transform
    private Transform odomTransform;

    //assigned before runtime in unity editor
    private Transform mapTransform;
    private Transform currentTransform;

    private enum centeringMode
    {
        odom,
        custom
    }

    [SerializeField]
    centeringMode currentMode;
    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(mapTopic, MapCallback);
        currentMode = centeringMode.custom;
        mapTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        //Odom object appears at run time, needs assign
        if (odomTransform == null)
        {
            odomTransform = GameObject.Find("odom").transform;
        }

        //Switch between map centering modes
        switch (currentMode)
        {
            case centeringMode.custom:
                currentTransform = mapTransform;
                break;
            case centeringMode.odom:
                currentTransform = odomTransform;
                break;
        }
    }
    void MapCallback(OccupancyGridMsg mapMsg)
    {
        mapResolution = mapMsg.info.resolution;
        mapWidth = (int)mapMsg.info.width;
        mapHeight = (int)mapMsg.info.height;

        mapRotation = Quaternion.Euler(0f, deg, 0f);
        if (currentTransform != null)
        {
            GenerateObstacles(mapMsg.data);
        } else
        {
            Debug.LogError("Centering object is null");
        }
    }

    void GenerateObstacles(sbyte[] data)
    {
        // Clear previous obstacles
        foreach (var obj in spawnedObstacles)
        {
            Destroy(obj);
        }
        spawnedObstacles.Clear();

        Vector3 mapCenterOffset = new Vector3(mapWidth * mapResolution / 2f, 0f, mapHeight * mapResolution / 2f);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = x + y * mapWidth;
                sbyte value = data[i];

                if (value > 50)
                {
                    Vector3 localPos = new Vector3(x * mapResolution, 0, y * mapResolution);
                    Vector3 centeredLocalPos = localPos - mapCenterOffset;

                    Vector3 rotatedLocalPos = mapRotation * centeredLocalPos;
                    Vector3 worldPos = currentTransform.position + rotatedLocalPos;

                    GameObject obj = Instantiate(obstaclePrefab, worldPos + new Vector3(xOffset, 0, zOffset), Quaternion.identity, currentTransform);
                    obj.transform.localScale = new Vector3(mapResolution, obstacleHeight, mapResolution);

                    spawnedObstacles.Add(obj);
                }
            }
        }
    }
}
