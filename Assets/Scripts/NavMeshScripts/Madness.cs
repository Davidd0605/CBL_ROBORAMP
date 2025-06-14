using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.AI;
using UnityEngine;

/**
* To be refactor if i had not done this already.
* This is the 5th solution I come up with for this today.
* My mental state has been degrading at a rapid pace.
*/
public class Madness : MonoBehaviour
{
    private NavMeshAgent agent;
    private ROSConnection ros;
    private GoalPosePublisher publisher;

    private bool isTouchingRobot = false;
    private bool hasGoal = false;

    public Material mat;

    public NavMeshAgent AgentInstance
    {
        get { return agent; }
    }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        ros = ROSConnection.GetOrCreateInstance();
        publisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
    }

    private void FixedUpdate()
    {
        if (!hasGoal)
        {
            transform.position = GameObject.FindGameObjectWithTag("Robot").transform.position;
        }
        else
        {
            PublishPosition();
            agent.isStopped = !isTouchingRobot;

            if (agent.remainingDistance < 0.05f && !agent.pathPending)
            {
                PublishPosition();
                Debug.LogWarning("Arrived at destination" + agent.destination);
                mat.color = new Color(0, 1, 0);
                agent.isStopped = true;
            }
            else
            {
                mat.color = new Color(1, 0, 0);
            }
        }
    }

    public void SetTarget(Vector3 target)
    {
        hasGoal = true;
        agent.SetDestination(target);
    }

    private void PublishPosition()
    {
        Vector3 unityPos = transform.position;
        Quaternion unityRot = transform.rotation;
        PointMsg position = CoordinateConverter.UnityToROSPosition(unityPos);
        QuaternionMsg orientation = CoordinateConverter.UnityToROSRotation(unityRot);
        publisher.PublishPose(position, orientation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            isTouchingRobot = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            isTouchingRobot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            isTouchingRobot = false;
        }
    }
}
