using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiagnosticEntry
{
    public string key;     // Unique ID
    public string message; // Human-readable
    public string level;   // "WARN" or "ERROR"
}
