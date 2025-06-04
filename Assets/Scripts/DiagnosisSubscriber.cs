using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Diagnostic;
using Unity.Robotics.Visualizations;

public class DiagnosisSubscriber : MonoBehaviour
{
    DiagnosticArrayMsg latestMsg;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<DiagnosticArrayMsg>(
            "/diagnostics",
            msg => latestMsg = msg
        );
    }

    void OnGUI()
    {
        if (latestMsg != null)
        {
            DiagnosticArrayDefaultVisualizer.GUI(latestMsg);
        }
        else
        {
            GUILayout.Label("Waiting for diagnostics...");
        }
    }
}
