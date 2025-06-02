using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;

public class GoalPosePublisher : MonoBehaviour
{

    /**
     * 
     * The purpose of this class is to take a destination in the format Pose.
     * It sends the data to the goal_pose topic, sending the turtle bot on the mission.
     * 
     * @author David
     */
    ROSConnection ros;
    public string topicName = "goal_pose";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }
    
    //keep public, called from outside
    public void PublishPose(PointMsg position, QuaternionMsg orientation)
    {
        PoseMsg poseMsg = new PoseMsg(position, orientation);

        HeaderMsg header = new HeaderMsg();
        header.stamp = new TimeMsg(0, 0);
        header.frame_id = "map";

        PoseStampedMsg goalPose = new PoseStampedMsg
        {
            header = header,
            pose = poseMsg
        };
        string topicName = "/goal_pose";
        ros.Publish(topicName, goalPose);
    }
}