using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMeshNav : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform point;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (point != null)
        {
            navMeshAgent.destination = point.position;

            
            if (navMeshAgent.hasPath)
            {
                Vector3[] corners = navMeshAgent.path.corners;

                for (int i = 0; i < corners.Length; i++)
                {
                    Debug.LogWarning($"Waypoint {i}: {corners[i]}");

                    
                    if (i < corners.Length - 1)
                    {
                        Debug.DrawLine(corners[i], corners[i + 1], Color.green);
                    }

                    Debug.DrawRay(corners[i], Vector3.up * 0.5f, Color.red);
                }
            }
        }
    }
}
