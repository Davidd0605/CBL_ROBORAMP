using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

/**
 *
 * Reads coordinates + rotation from specified topic.
 * These are stored in a queue, prioritized by the time rev.
 * I want to die lowkey hehehehaw.
 *
 */
public class queuingSystem : MonoBehaviour
{
    private enum PathingMode
    {
        PhysicalOnly,
        VirtualPathing
    };
    
    public struct Pose
    {
        public PointMsg position;
        public QuaternionMsg rotation;

        public Pose(PointMsg pos, QuaternionMsg rot)
        {
            position = pos;
            rotation = rot;
        }

        public bool Equals(Pose other)
        {
            if (position == null || rotation == null)
            {
                return false;
            }

            return position.Equals(other.position) && rotation.Equals(other.rotation);
        }
    }

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

            Debug.LogWarning("Current goal set: " + unityGoal);

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