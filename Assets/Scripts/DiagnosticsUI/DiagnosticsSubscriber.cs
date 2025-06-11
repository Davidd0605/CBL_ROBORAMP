using UnityEngine;
using RosMessageTypes.Diagnostic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System.Collections.Generic;

public class DiagnosticsSubscriber : MonoBehaviour
{
    public DiagnosticsUI diagnosticsUI;

    ROSConnection ros;

    // Store last known good battery percentage
    private float lastBatteryPercentage = -1f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<DiagnosticArrayMsg>("/diagnostics", OnDiagnosticsReceived);
    }

    void OnDiagnosticsReceived(DiagnosticArrayMsg msg)
    {
        float batteryPercentage = -1f;
        bool batteryFound = false;

        List<DiagnosticEntry> diagnosticEntries = new();

        foreach (var status in msg.status)
        {
            // Battery Info Detection
            if (!batteryFound && IsBatteryStatus(status))
            {
                foreach (var kv in status.values)
                {
                    if (kv.key.ToLower().Contains("percentage") || kv.key.ToLower().Contains("battery %"))
                    {
                        if (float.TryParse(kv.value, out float parsed))
                        {
                            batteryPercentage = parsed;
                            batteryFound = true;
                            break;
                        }
                    }
                }
            }

            // Collect Warnings/Errors
            if (status.level == 1 || status.level == 2)
            {
                string key = $"{status.name}_{status.level}";
                diagnosticEntries.Add(new DiagnosticEntry
                {
                    key = key,
                    message = status.message,
                    level = status.level == 1 ? "WARN" : "ERROR"
                });
            }
        }

        // Only update battery if we found a valid value
        if (batteryFound)
        {
            lastBatteryPercentage = batteryPercentage;
            diagnosticsUI.UpdateBatteryStatus(batteryPercentage);
        }
        else if (lastBatteryPercentage >= 0)
        {
            diagnosticsUI.UpdateBatteryStatus(lastBatteryPercentage);
        }

        diagnosticsUI.UpdateDiagnostics(diagnosticEntries);
    }

    bool IsBatteryStatus(DiagnosticStatusMsg status)
    {
        string lowerName = status.name.ToLower();
        string lowerID = status.hardware_id.ToLower();

        if (lowerName.Contains("battery") || lowerID.Contains("battery") || lowerID.Contains("opencr"))
            return true;

        foreach (var kv in status.values)
        {
            if (kv.key.ToLower().Contains("percentage") || kv.key.ToLower().Contains("battery %"))
                return true;
        }

        return false;
    }
}

