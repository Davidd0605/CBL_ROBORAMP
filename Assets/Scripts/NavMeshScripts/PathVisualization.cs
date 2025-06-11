using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathVisualization : MonoBehaviour
{

    public Madness madnessScript;

    void Start()
    {

        if (madnessScript == null)
        {
            Debug.LogError("PathVisualization: Madness Script is not assigned in the Inspector. Disabling script.");
            enabled = false;
            return;
        }

        if (madnessScript.AgentInstance == null)
        {
            Debug.LogError("PathVisualization: NavMeshAgent in Madness Script is null. Make sure Madness Start() runs first.");
            enabled = false;
            return;
        }
    }

    void Update()
    {

        if (madnessScript == null || madnessScript.AgentInstance == null)
        {
            return;
        }

        NavMeshAgent agentToVisualize = madnessScript.AgentInstance;

        if (agentToVisualize.hasPath)
        {
            Vector3[] corners = agentToVisualize.path.corners;

            for (int i = 0; i < corners.Length; i++)
            {


                if (i < corners.Length - 1)
                {
                    Debug.DrawLine(corners[i], corners[i + 1], Color.green);
                }


                Debug.DrawRay(corners[i], Vector3.up * 0.5f, Color.red);
            }
        }

        else
        {
            Debug.Log("NavMeshAgent has no path to visualize.");
        }
    }
}