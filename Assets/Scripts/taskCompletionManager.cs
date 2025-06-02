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
     *  This class can be removed by tweaking the package that gets goal reachable. The topic that package is reading also says stuff abt the goal status (reached)
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
    public bool isSleeping = false;

    public float threshold = 0.4f;


    private MissionCooldown missionCooldown;
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<OdometryMsg>(odomTopic, OdomCallback);
        ros.Subscribe<PoseStampedMsg>(goalTopic, GoalCallback);

        missionCooldown = GameObject.FindGameObjectWithTag("logic").GetComponent<MissionCooldown>();
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
            //TODO INCLUDE IN UI
            //Debug.Log("Distance to goal: " + distance.ToString("F3"));

            if (distance < threshold)
            {
                missionCooldown.startSleeping();
                hasGoal = false;
            }
        }
        isSleeping = missionCooldown.isSleeping;
    }
}