using UnityEngine;

public class TEST : MonoBehaviour
{
    [SerializeField]
    private RosMessageHandler messageHandler;

    void Update()
    {
        messageHandler.OnRosMessageReceived("1:0.5");
        messageHandler.OnRosMessageReceived("2:0.56");
    }
}
