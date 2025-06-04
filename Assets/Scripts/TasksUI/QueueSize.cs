using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class that displays the queue size in real time.
 * @author Daria
 */

public class QueueSize : MonoBehaviour
{
    public TextMeshProUGUI queueSizeText;
    public queuingSystem queuingSystem;

    // Update is called once per frame
    void Update()
    {
        int size = queuingSystem.getGoalQueueCount();
        queueSizeText.text = "Queue size: " + size;
    }
}
