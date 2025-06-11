using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QueuingDIsplay : MonoBehaviour
{
    private Text queuingText;
    private queuingSystem queSys;
    private taskCompletionManager taskManager;

    private void Start()
    {
        queuingText = GetComponent<Text>();
        queSys = GameObject.FindGameObjectWithTag("logic").GetComponent<queuingSystem>();
        taskManager  = GameObject.FindGameObjectWithTag("logic").GetComponent<taskCompletionManager>();
    }
    // Update is called once per frame
    void Update()
    {
        queuingText.text = "Queue size: " + queSys.getQueueSize().ToString()
            + "\nCurrent goal: \n" + queSys.getCurrentGoal().ToString()
            + "\nDistance:" + taskManager.getDistance()
            + "\nStatus: " + taskManager.getStatus();
    }
}
