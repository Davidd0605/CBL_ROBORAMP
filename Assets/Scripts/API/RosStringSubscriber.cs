using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class RosStringSubscriber : MonoBehaviour
{
    [SerializeField]
    private string topicName = "/request";

    [SerializeField]
    private RosMessageHandler messageHandler;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>(topicName, OnMessageReceived);
    }

    void OnMessageReceived(StringMsg msg)
    {
        if (messageHandler != null)
        {
            messageHandler.OnRosMessageReceived(msg.data);
        }
        else
        {
            Debug.LogWarning("RosMessageHandler is not assigned.");
        }
    }
}
