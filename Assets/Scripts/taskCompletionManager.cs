using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;

public class taskCompletionManager : MonoBehaviour
{

    /**
     * 
     * The purpose of this class is to get status on the current mission of the robot.
     *  hasGoal = false when the robot is not path finding.
     *  hasGoal = true when the robot is path finding.
     *  
     *  @TODO
     *      Make some way of determining when the robot is just not doing the thing.
     *      Also remove debugging lol.
     * 
     * @author David
     */
    ROSConnection ros;

    //monitored topics guess what they do lol.
    public string odomTopic = "/odom";
    public string goalTopic = "/goal_pose";

    private Vector3 currentPosition;
    private Vector3 goalPosition;

    private bool hasOdom = false;
    public bool hasGoal = false;

    public float threshold = 0.4f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<OdometryMsg>(odomTopic, OdomCallback);
        ros.Subscribe<PoseStampedMsg>(goalTopic, GoalCallback);
    }

    void OdomCallback(OdometryMsg msg)
    {
        var pos = msg.pose.pose.position;
        currentPosition = new Vector3(
            (float)pos.y * -1f,
            0f,
            (float)pos.x
        );
        hasOdom = true;
    }

    void GoalCallback(PoseStampedMsg msg)
    {
        var pos = msg.pose.position;
        goalPosition = new Vector3(
            (float)pos.y * -1f,
            0f,
            (float)pos.x
        );
        hasGoal = true;
    }

    void Update()
    {
        if (hasOdom && hasGoal)
        {
            float distance = Vector3.Distance(currentPosition, goalPosition);
            //Debug.Log("Distance to goal: " + distance.ToString("F3"));

            if (distance < threshold)
            {
                hasGoal = false;
            }
        }
    }
}