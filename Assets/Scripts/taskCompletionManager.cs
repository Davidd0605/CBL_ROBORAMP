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

    private Vector3 goalPosition;

    private GameObject Robot;

    private bool hasGoal = false;
    private bool isSleeping = false;
    public bool isReady = false;

    public float threshold = 0.4f;

    public GameObject goalMarker;
    private GameObject currentMarker;

    private MissionCooldown missionCooldown;

    private float distance = -1.0f;
    private enum missionStatus
    {
        waiting_for_goal,
        navigation_to_goal,
        finished_mission
    }

    private missionStatus currentStatus;

    void Start()
    {
        Robot = GameObject.FindGameObjectWithTag("Robot");
        missionCooldown = GameObject.FindGameObjectWithTag("logic").GetComponent<MissionCooldown>();

        currentStatus = missionStatus.waiting_for_goal;
    }

    public void setCurrentGoal(Vector3 goal)
    {
        goalPosition = goal;
        hasGoal = true;

        if (currentMarker != null)
        {
            Destroy(currentMarker);
        }
    }

    void FixedUpdate()
    {
        if (hasGoal)
        {
            if (currentMarker == null)
            {
                currentMarker = Instantiate(goalMarker, goalPosition, Quaternion.identity);
            }
            distance = Vector3.Distance(Robot.transform.position, goalPosition);

            Debug.Log("Current remaining distance: " + distance);

            if (distance < threshold)
            {
                missionCooldown.startSleeping();
                hasGoal = false;
                Destroy(currentMarker, 2);
            }
        }
        isSleeping = missionCooldown.isSleeping;
        isReady = !hasGoal && !isSleeping;
    }

    private void Update()
    {
        if (hasGoal)
        {
            currentStatus = missionStatus.navigation_to_goal;
        }

        if (!hasGoal && !isReady)
        {
            currentStatus = missionStatus.finished_mission;
        }

        if (isReady)
        {
            currentStatus = missionStatus.waiting_for_goal;
        }
    }


    public string getStatus()
    {
        return currentStatus.ToString();
    }

    public float getDistance()
    {
        return distance;
    }
}