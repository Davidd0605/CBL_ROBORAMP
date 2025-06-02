using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class UpdateYAML : MonoBehaviour
{
public void UpdateYamlFile(string filePath, bool useAStar)
{
    Debug.Log("UpdateYamlFile called with filePath: " + filePath + " and useAStar: " + useAStar);

    // Read the file
    string text = File.ReadAllText(filePath);

    // Use a regular expression to find the line with the use_astar parameter
    Regex regex = new Regex(@"\s*use_astar: \w+");

    // Check if use_astar already exists in the file
    if (regex.IsMatch(text))
    {
        // If it exists, replace it
        string replacement = "\n      use_astar: " + (useAStar ? "true" : "false");
        text = regex.Replace(text, replacement);
    }
    else
    {
        // If it doesn't exist, add it to a new line at the end of the file
        text += "\n       use_astar: " + (useAStar ? "true" : "false");
    }

    // Write the file back out
    File.WriteAllText(filePath, text);

    Debug.Log("UpdateYamlFile completed");
}

public void CheckFileAccess(string filePath)
{
    if (File.Exists(filePath))
    {
        Debug.Log("File exists at: " + filePath);
        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                Debug.Log("File can be opened for reading and writing");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to open file for reading and writing: " + e.Message);
        }
    }
    else
    {
        Debug.Log("File does not exist at: " + filePath);
    }
}
}