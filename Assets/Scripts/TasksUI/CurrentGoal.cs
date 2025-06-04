using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentGoal : MonoBehaviour
{
    public queuingSystem queuingSystem;
    public TextMeshProUGUI currentGoalText;

    // Update is called once per frame
    void Update()
    {
        Vector3 unityGoal = queuingSystem.getUnityGoal();
        currentGoalText.text = "Current goal: \n" + unityGoal.ToString();
    }
}
