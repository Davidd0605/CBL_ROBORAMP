using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

/**
 *
 * Queuing system for the ramp robot.
 * Queued positions/rotations are passed through a certain topics.
 * They are stored in a queue structure and dequeued when the robot is available.
 * The robot can be set on either physical only or physical + virtual obstacles.
 *
 */
public class queuingSystem : MonoBehaviour
{
    private enum PathingMode
    {
        PhysicalOnly,
        VirtualPathing
    };

    [SerializeField] private PathingMode currentMode = PathingMode.PhysicalOnly;

    ROSConnection ros;

    [SerializeField] private string receiveTopic = "/topic_queuing";
    
    private Queue<Pose> goalQueue = new Queue<Pose>();

    private Pose currentGoal;
    private Pose lastInserted;

    private taskCompletionManager taskCompletion;
    private GoalPosePublisher publisher;

    private bool skippedGoal = false;

    private void Start()
    {
        lastInserted.position = null;
        lastInserted.rotation = null;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseStampedMsg>(receiveTopic, GoalCallback);

        taskCompletion = GetComponent<taskCompletionManager>();
        publisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            skipCurrentGoal();
        }

        if ((goalQueue.Count > 0 && taskCompletion.isReady) || skippedGoal)
        {
            skippedGoal = false;
            currentGoal = goalQueue.Dequeue();

            Vector3 unityGoal = CoordinateConverter.ROSToUnityPosition(currentGoal.position);
            taskCompletion.setCurrentGoal(unityGoal);

            switch (currentMode)
            {
                case PathingMode.PhysicalOnly:
                    publisher.PublishPose(currentGoal.position, currentGoal.rotation);
                    break;
                case PathingMode.VirtualPathing:
                    //TODO: Improve virtual pathing tuesday
                    GameObject.FindGameObjectWithTag("Searcher").GetComponent<Madness>().SetTarget(unityGoal);
                    break;
            }
        }
    }

    private void GoalCallback(PoseStampedMsg msg)
    {
        Pose newGoal = new Pose(msg.pose.position, msg.pose.orientation);
        if (!newGoal.Equals(lastInserted))
        {
            goalQueue.Enqueue(newGoal);
        }

        lastInserted = newGoal;
    }

    public int getQueueSize()
    {
        return goalQueue.Count;
    }

    public Vector3 getCurrentGoal()
    {
        if (currentGoal.position != null)
        {
            return CoordinateConverter.ROSToUnityPosition(currentGoal.position);
        }
        else
        {
            return Vector3.up;
        }
    }

    public void skipCurrentGoal()
    {
        if (skippedGoal == false)
        {
            if (goalQueue.Count == 0)
            {
                PointMsg currentPosition =
                    CoordinateConverter.UnityToROSPosition(GameObject.FindGameObjectWithTag("Robot").transform
                        .position);
                QuaternionMsg currentRotation =
                    CoordinateConverter.UnityToROSRotation(GameObject.FindGameObjectWithTag("Robot").transform
                        .rotation);
                goalQueue.Enqueue(new Pose(currentPosition, currentRotation));
            }

            skippedGoal = true;
        }
    }
}