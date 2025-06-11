using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

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
public class taskCompletionManager : MonoBehaviour
{ 
    private enum MissionStatus
    {
        WaitingForGoal,
        NavigationToGoal,
        FinishedMission
    }

    ROSConnection ros;
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
    private MissionStatus currentStatus;
    
    void Start()
    {
        Robot = GameObject.FindGameObjectWithTag("Robot");
        missionCooldown = GameObject.FindGameObjectWithTag("logic").GetComponent<MissionCooldown>();

        currentStatus = MissionStatus.WaitingForGoal;
    }

    public void setCurrentGoal(Vector3 goal)
    {
        goalPosition = goal;
        hasGoal = true;

        if (currentMarker != null)
        {
            Destroy(currentMarker);
        }
        
        currentMarker = Instantiate(goalMarker, goalPosition, Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (hasGoal)
        {
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
            currentStatus = MissionStatus.NavigationToGoal;
        }

        if (!hasGoal && !isReady)
        {
            currentStatus = MissionStatus.FinishedMission;
        }

        if (isReady)
        {
            currentStatus = MissionStatus.WaitingForGoal;
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