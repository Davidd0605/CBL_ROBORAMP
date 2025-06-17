using UnityEngine;
using System.Collections.Generic;
using RosMessageTypes.Diagnostic;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;

public class DiagnosticsSubscriber : MonoBehaviour
{
    public DiagnosticsUI diagnosticsUI;

    private float lastBatteryPercentage = -1f;
    private float lastDiagnosticsBatteryUpdate = -10f;
    private float diagnosticsBatteryTimeout = 5f; // seconds

    void Start()
    {
        var ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<DiagnosticArrayMsg>("/diagnostics", OnDiagnosticsReceived);
        ros.Subscribe<BatteryStateMsg>("/battery_state", OnBatteryStateReceived);
    }

    void Update()
    {
        // In case diagnostics has stopped sending battery data
        if (Time.time - lastDiagnosticsBatteryUpdate > diagnosticsBatteryTimeout && lastBatteryPercentage >= 0)
        {
            diagnosticsUI.UpdateBatteryStatus(lastBatteryPercentage);
        }
    }

    void OnBatteryStateReceived(BatteryStateMsg msg)
    {
        if (msg.percentage >= 0 && msg.percentage <= 1.0)
        {
            float percentage = msg.percentage * 100f;
            lastBatteryPercentage = percentage;

            // Only use battery_state if diagnostics hasn't updated recently
            if (Time.time - lastDiagnosticsBatteryUpdate > diagnosticsBatteryTimeout)
            {
                diagnosticsUI.UpdateBatteryStatus(percentage);
            }
        }
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

            // Warnings & Errors
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

        // Prefer diagnostics battery value when valid
        if (batteryFound)
        {
            lastBatteryPercentage = batteryPercentage;
            lastDiagnosticsBatteryUpdate = Time.time;
            diagnosticsUI.UpdateBatteryStatus(batteryPercentage);
        }

        diagnosticsUI.UpdateDiagnostics(diagnosticEntries);
    }

    bool IsBatteryStatus(DiagnosticStatusMsg status)
    {
        string name = status.name.ToLower();
        return name.Contains("battery");
    }
}
