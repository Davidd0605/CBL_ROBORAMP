using UnityEngine;
using UnityEngine.AI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;


public class ActorController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Vector3 targetPosition;
    private bool touchingRobot = false;
    private GoalPosePublisher publisher;

    // ROS
    private ROSConnection ros;
    private bool destinationSet = false;
    private bool destinationInserted = false;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(targetPosition);
        agent.isStopped = true;

        // Connect to ROS
        ros = ROSConnection.GetOrCreateInstance();
        publisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
    }

    void Update()
    {
        if (!destinationSet && destinationInserted)
        {
            agent.SetDestination(targetPosition);
            destinationSet = true;
        } else
        {
            movingState();
        }

    }

    private void movingState()
    {
        bool isMoving = touchingRobot;
        agent.isStopped = !isMoving;

        if (isMoving)
        {
            //PublishPosition();
        }

        if (agent.remainingDistance <= 0.1)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Debug.LogWarning("Allhu akbar" + transform.position);
                Destroy(gameObject);
            }
        }
    }
    public void setTarget(Vector3 destination)
    {
        Debug.LogWarning("This is where the actor was set to go" + destination);
        destinationInserted = true;
        targetPosition = destination;
    }
    private void PublishPosition()
    {
        Vector3 unityPos = transform.position;
        Quaternion unityRot = transform.rotation;

        PointMsg position = CoordinateConverter.UnityToROSPosition(unityPos);
        QuaternionMsg orientation = CoordinateConverter.UnityToROSRotation(unityRot);
        Quaternion correctedRot = Quaternion.Euler(-90f, 0f, 0f) * unityRot;

        publisher.PublishPose(position, orientation);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            touchingRobot = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            touchingRobot = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            touchingRobot = true;
        }
    }
}