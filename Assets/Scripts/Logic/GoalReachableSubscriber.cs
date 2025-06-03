using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class GoalReachableSubscriber : MonoBehaviour
{

    public bool canReach;
    [SerializeField]
    private string topicName = "/goal_reachable";


    private int bufferCan = 0;
    private int bufferCannot = 0;

    private taskCompletionManager taskManager;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<BoolMsg>(topicName, OnGoalReachableReceived);
        taskManager = gameObject.GetComponent<taskCompletionManager>();
    }

    void OnGoalReachableReceived(BoolMsg msg)
    {
        if (msg.data)
        {
            bufferCan++;
        }
        else
        {
            bufferCannot++;
        }
    }

    private void Update()
    {
        //Debug.LogWarning(bufferCan +" "+ bufferCannot);
        if(bufferCan + bufferCannot > 40)
        {
            if(bufferCan > bufferCannot)
            {
                canReach = true;
            } else
            {
                canReach = false;
            }
            resetBuffer();
        } else
        {
            canReach = true;
        }
    }

    public void resetBuffer()
    {
        bufferCan = bufferCannot = 0;
    }
}
