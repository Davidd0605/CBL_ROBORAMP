using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Nav;

public class CubeScript : MonoBehaviour
{
    ROSConnection ros;
    public string odomTopic = "/odom";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>(odomTopic, OdomCallback);
    }

    void OdomCallback(OdometryMsg msg)
    {
        var pos = msg.pose.pose.position;
        var ori = msg.pose.pose.orientation;

        Vector3 unityPosition = new Vector3(
            (float)pos.y * -1f,  
            transform.position.y,                  
            (float)pos.x         
        );

        // Convert ROS quaternion to Unity quaternion
        // May require axis corrections due to coordinate system differences
        Quaternion unityRotation = new Quaternion(
            -(float)ori.x,
            -(float)ori.z,
            (float)ori.y,
            (float)ori.w
        );

        // Apply to Unity GameObject
        transform.position = unityPosition;
        transform.rotation = unityRotation;
    }
}