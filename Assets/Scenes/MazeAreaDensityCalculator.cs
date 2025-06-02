using UnityEngine;
using System.Collections.Generic;

public class MazeAreaDensityCalculator : MonoBehaviour
{
    public GameObject maze; // Your maze object
    public List<GameObject> obstacleParents; // Your list of obstacle parent objects
    private GameObject activeParent; // The currently active parent

    public UpdateYAML updateyaml;
    void Start()
    {
        activeParent = GetActiveParent();
        CalculateAndLogDensity();
    }

    void Update()
    {
        GameObject newActiveParent = GetActiveParent();

        // If the active parent has changed, recalculate the density
        if (newActiveParent != activeParent)
        {
            activeParent = newActiveParent;
            CalculateAndLogDensity();
        }
    }

    GameObject GetActiveParent()
    {
        foreach (GameObject parent in obstacleParents)
        {
            if (parent.activeInHierarchy)
            {
                return parent;
            }
        }

        return null;
    }

    void CalculateAndLogDensity()
    {
        float mazeArea = CalculateMazeArea(maze);

        // Get the children of the active parent
        List<GameObject> activeObstacles = new List<GameObject>();
        if (activeParent != null)
        {
            foreach (Transform child in activeParent.transform)
            {
                activeObstacles.Add(child.gameObject);
            }
        }

        float obstacleArea = CalculateObstacleArea(activeObstacles);

        float density = obstacleArea / mazeArea;

        Debug.Log("Obstacle Area Density: " + density);

        // Log the algorithm to use based on the density
        if (density > 0.1)
        {
            Debug.Log("Use A* algorithm");
            //updateyaml.CheckFileAccess("/home/mazyar/set-lab-digital-twin-main/My project/Assets/turtlebot3-foxy-devel/turtlebot3_navigation2/param/burger.yaml");
            updateyaml.UpdateYamlFile("/home/mazyar/set-lab-digital-twin-main /My project/Assets/turtlebot3-foxy-devel/turtlebot3_navigation2/param/burger.yaml", true);
        }
        else
        {
            Debug.Log("Use Dijkstra's algorithm");
           // updateyaml.CheckFileAccess("/home/mazyar/set-lab-digital-twin-main/My project/Assets/turtlebot3-foxy-devel/turtlebot3_navigation2/param/burger.yaml");
            updateyaml.UpdateYamlFile("/home/mazyar/set-lab-digital-twin-main/My project/Assets/turtlebot3-foxy-devel/turtlebot3_navigation2/param/burger.yaml", false);
        }
    }

    float CalculateMazeArea(GameObject maze)
    {
        Vector3 size = maze.GetComponent<Renderer>().bounds.size;
        return size.x * size.z; // Assuming y is the height
    }

    float CalculateObstacleArea(List<GameObject> obstacles)
    {
        float totalArea = 0;

        foreach (GameObject obstacle in obstacles)
        {
            Vector3 size = obstacle.GetComponent<Renderer>().bounds.size;

            if (obstacle.tag == "cuboid")
            {
                // If the obstacle is a cuboid, calculate the area as length * width
                totalArea += size.x * size.z; // Assuming y is the height
            }
            else if (obstacle.tag == "cylinder")
            {
                // If the obstacle is a cylinder, calculate the area as π * radius^2
                float radius = size.x / 2; // Assuming the diameter is along the x-axis
                totalArea += Mathf.PI * Mathf.Pow(radius, 2);
            }
        }

        return totalArea;
    }
}
