using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;


public class GoalFeasibilitySubscriber : MonoBehaviour
{
    ROSConnection ros;
    private int bufferCounter;

    [SerializeField]
    private int bufferCap;

    private queuingSystem queuingSystem;
    public string topicName = "/goal_feasible";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<BoolMsg>(topicName, GoalFeasibilityCallback);
        queuingSystem = GetComponent<queuingSystem>();
        bufferCap = 750;
    }

    void GoalFeasibilityCallback(BoolMsg msg)
    {
        if (msg.data)
        {

        }
        else
        {
            bufferCounter++;
        }
    }

    void FixedUpdate()
    {
        if (bufferCounter >= bufferCap)
        {
            queuingSystem.skipCurrentGoal();
        }
    }

    public void resetBuffer()
    {
        bufferCounter = 0;
    }
}
