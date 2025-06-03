using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestSubgoal : MonoBehaviour
{
    public Transform point;
    private NavMeshAgent agent;
    public float stepSize = 0.2f; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (point != null)
        {
            agent.destination = point.position;

            if (agent.hasPath)
            {
                Vector3[] corners = agent.path.corners;
                List<Vector3> detailedPath = new List<Vector3>();

                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Vector3 start = corners[i];
                    Vector3 end = corners[i + 1];
                    float distance = Vector3.Distance(start, end);
                    int steps = Mathf.CeilToInt(distance / stepSize);
                    Debug.DrawLine(corners[i], corners[i + 1], Color.green);
                    Debug.LogWarning($"Waypoint {i}: {corners[i]}");

                    for (int j = 0; j < steps; j++)
                    {
                        float t = j / (float)steps;
                        Vector3 interpolated = Vector3.Lerp(start, end, t);
                        detailedPath.Add(interpolated);

                       
                        Debug.DrawRay(interpolated, Vector3.up * 0.5f, Color.red);
                    }
                }

           
                detailedPath.Add(corners[corners.Length - 1]);
            }
        }
    }
}
