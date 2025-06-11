using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosticsUI : MonoBehaviour
{
    [Header("Battery UI")]
    public Text batteryPercentageText;
    public Image batteryIcon;

    [Header("Diagnostics Log UI")]
    public Transform logContent;
    public GameObject logEntryPrefab;

    // Track active logs and their timestamps
    private Dictionary<string, LogEntryData> activeLogs = new();

    // Configurable timeout
    public float warningTimeoutSeconds = 2.0f;

    void Update()
    {
        float now = Time.time;
        List<string> keysToRemove = new();

        foreach (var pair in activeLogs)
        {
            if (now - pair.Value.lastSeenTime > warningTimeoutSeconds)
            {
                Destroy(pair.Value.uiObject);
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            activeLogs.Remove(key);
        }
    }

    public void UpdateBatteryStatus(float percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            batteryPercentageText.text = "Battery: N/A";
            batteryIcon.color = Color.gray;
            return;
        }

        batteryPercentageText.text = $"Battery: {percentage:F0}%";
        batteryIcon.color = percentage switch
        {
            > 50 => Color.green,
            > 20 => Color.yellow,
            _ => Color.red
        };
    }

    public void UpdateDiagnostics(List<DiagnosticEntry> entries)
    {
        float now = Time.time;

        foreach (var entry in entries)
        {
            if (!activeLogs.ContainsKey(entry.key))
            {
                GameObject logGO = Instantiate(logEntryPrefab, logContent);
                var text = logGO.GetComponentInChildren<Text>();
                if (text != null)
                    text.text = $"{entry.level}: {entry.message}";

                activeLogs[entry.key] = new LogEntryData
                {
                    uiObject = logGO,
                    lastSeenTime = now
                };
            }
            else
            {
                // Just update the time, UI already exists
                activeLogs[entry.key].lastSeenTime = now;
            }
        }
    }

    private class LogEntryData
    {
        public GameObject uiObject;
        public float lastSeenTime;
    }
}
