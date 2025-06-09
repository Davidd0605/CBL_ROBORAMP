using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class that displays the queuing system information in real-time.
 * @author Daria
 */

public class QueuingDisplay : MonoBehaviour
{
    public TextMeshProUGUI queuingText;
    public queuingSystem queuingSystem;

    // Update is called once per frame
    void Update()
    {
        int size = queuingSystem.getGoalQueueCount();
        Vector3 unityGoal = queuingSystem.getUnityGoal();
        queuingText.text = "Queue size: " + size.ToString()
                    + "\nCurrent goal: \n" + unityGoal.ToString()
                    + "\nDistance to goal: "
                    + "\nStatus: ";
    }
}
