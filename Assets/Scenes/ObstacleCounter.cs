using UnityEngine;

public class ObstacleCounter : MonoBehaviour
{
    public LayerMask obstacleLayer;

    void Start()
    {
        CountObstacles();
    }

    void CountObstacles()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int obstacleCount = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy && ((1 << go.layer) & obstacleLayer) != 0)
            {
                obstacleCount++;
            }
        }

        Debug.Log("There are " + obstacleCount + " obstacles in the scene.");
        if (obstacleCount > 5)
        {
        Debug.Log("Obstacle density is high. Use Dijkstra");
        }
        else
        {
        Debug.Log("Obstacle density is sparse. Use A*");
        }
    }
}
