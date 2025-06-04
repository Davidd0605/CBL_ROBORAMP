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

    private Vector3 currentPosition;
    private Vector3 goalPosition;

    private bool hasOdom = false;
    public bool hasGoal = false;
    public bool isSleeping = false;
    private bool hasVisual = false;
    public float threshold = 0.4f;

    public GameObject goalMarker;

    private MissionCooldown missionCooldown;
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<OdometryMsg>(odomTopic, OdomCallback);

        missionCooldown = GameObject.FindGameObjectWithTag("logic").GetComponent<MissionCooldown>();
    }

    void OdomCallback(OdometryMsg msg)
    {
        var pos = msg.pose.pose.position;
        currentPosition = CoordinateConverter.ROSToUnityPosition(pos);
        hasOdom = true;
    }
    public void setCurrentGoal(Vector3 goal)
    {
        goalPosition = goal;
        hasGoal = true;
    }

    void Update()
    {
        if (hasOdom && hasGoal)
        {
            if (!hasVisual)
            {
                hasVisual = true;
                Instantiate(goalMarker, goalPosition, Quaternion.identity);
            }

            float distance = Vector3.Distance(currentPosition, goalPosition);
            if (distance < threshold)
            {
                missionCooldown.startSleeping();
                hasGoal = false;
                hasVisual = false;
            }
        }
        isSleeping = missionCooldown.isSleeping;
    }
}