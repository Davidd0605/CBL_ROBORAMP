using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;

public class RosPublisherExample : MonoBehaviour
{

    /**
     * 
     *  This class sends goal_pose structured data to a custom topic. 
     *  This topis is to be read by the queueing system and processed furter.
     *  Sending the data should be done similarly there.
     *  
     * @author David
     */
    ROSConnection ros;
    public string topicName = "/topic_queuing";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendRandomGoal();
        }
    }

    void SendRandomGoal()
    {
        float x = Random.Range(-2f, 2f);
        float y = Random.Range(-2f, 2f);

        float angle = Random.Range(0f, 360f);
        Quaternion unityQuat = Quaternion.Euler(0, 0, angle);

        PointMsg position = new PointMsg(x, y, 0.0);
        QuaternionMsg orientation = new QuaternionMsg(unityQuat.x, unityQuat.y, unityQuat.z, unityQuat.w);
        PoseMsg pose = new PoseMsg(position, orientation);

        HeaderMsg header = new HeaderMsg();
        header.stamp = new TimeMsg(0, 0); 
        header.frame_id = "map";

        PoseStampedMsg goalPose = new PoseStampedMsg();
        goalPose.header = header;
        goalPose.pose = pose;

        ros.Publish(topicName, goalPose);

    }

}