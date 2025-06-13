using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;

public class LocalCostmapVisualiser : MonoBehaviour
{
    [Header("ROS Settings")]
    public string topicName = "/local_costmap/costmap";

    [Header("Visualization Settings")]
    public GameObject cellPrefab;
    public Transform mapParent;
    public float cellHeight = 0.05f;
    public float updateInterval = 0.5f;

    private float resolution;
    private int width, height;
    private Vector3 origin;
    private GameObject[,] cells;
    private sbyte[] costmapData;

    private bool dataReady = false;
    private float timeSinceLastUpdate = 0f;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(topicName, OnCostmapReceived);
    }

    void Update()
    {
        if (dataReady && Time.time - timeSinceLastUpdate >= updateInterval)
        {
            RenderCostmap();
            dataReady = false;
            timeSinceLastUpdate = Time.time;
        }
    }

    void OnCostmapReceived(OccupancyGridMsg msg)
    {
        resolution = msg.info.resolution;
        width = (int)msg.info.width;
        height = (int)msg.info.height;

        origin = new Vector3(
            (float)msg.info.origin.position.x,
            0f,
            (float)msg.info.origin.position.y
        );

        costmapData = msg.data;
        dataReady = true;
    }

    void RenderCostmap()
    {
        if (cells == null)
        {
            cells = new GameObject[width, height];
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                sbyte cost = costmapData[index];

                // Skip unknown or free space
                if (cost < 0 || cost < 10)
                    continue;

                Vector3 cellPos = origin + new Vector3(x * resolution, 0, y * resolution);

                if (cells[x, y] == null)
                {
                    GameObject newCell = Instantiate(cellPrefab, cellPos, Quaternion.identity, mapParent);
                    newCell.transform.localScale = new Vector3(resolution, cellHeight, resolution);

                    Renderer rend = newCell.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        float intensity = Mathf.InverseLerp(0, 100, cost);
                        rend.material.color = Color.Lerp(Color.white, Color.black, intensity);
                    }

                    cells[x, y] = newCell;
                }
            }
        }
    }
}