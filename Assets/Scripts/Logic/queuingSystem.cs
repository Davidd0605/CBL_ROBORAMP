using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
public class queuingSystem : MonoBehaviour
{
    /**
     * 
     * Reads coordinates + rotation from specified topic.
     * These are stored in a queue, prioritized by the time rev.
     * I want to die lowkey hehehehaw.
     * Currently has a somewhat harmless bug, it enqueues requests 2 times however they get processed one after the other.
     * 
     * 
     */
    private enum pathingMode
    {
        physicalOnly,
        virtualPathing
    };
    [SerializeField]
    private pathingMode currentMode = pathingMode.physicalOnly;

    ROSConnection ros;

    [SerializeField]
    private string receiveTopic = "/topic_queuing";

    

    public struct Pose
    {
        public PointMsg position;
        public QuaternionMsg rotation;

        public Pose(PointMsg pos, QuaternionMsg rot)
        {
            position = pos;
            rotation = rot;
        }
        
        public bool equals(Pose other)
        {
            if (position == null || rotation == null)
            {
                return false;
            }
            
            return position.Equals(other.position) && rotation.Equals(other.rotation);
        }
    }

    private Queue<Pose> goalQueue = new Queue<Pose>();

    private Pose currentGoal;
    private Pose lastInserted;

    private taskCompletionManager taskCompletion;
    private GoalPosePublisher publisher;

    private bool skippedGoal = false;
    void Start()
    {
        //ghetto style
        lastInserted.position = null;
        lastInserted.rotation = null;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseStampedMsg>(receiveTopic, GoalCallback);

        taskCompletion = GetComponent<taskCompletionManager>();
        publisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
    }

    void Update()
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

            //Set current goal in task manager
            taskCompletion.setCurrentGoal(unityGoal);

            //Choosed the pathing mode
            switch (currentMode) {
                case pathingMode.physicalOnly:
                    publisher.PublishPose(currentGoal.position, currentGoal.rotation);
                    break;
                case pathingMode.virtualPathing:
                    //TODO: Improve virtual pathing tuesday
                    GameObject.FindGameObjectWithTag("Searcher").GetComponent<Madness>().SetTarget(unityGoal);
                    break;

            }
        }
    }

    void GoalCallback(PoseStampedMsg msg)
    {
        Pose newGoal = new Pose(msg.pose.position, msg.pose.orientation);
        if (!newGoal.equals(lastInserted))
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
        } else
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
                PointMsg currentPosition = CoordinateConverter.UnityToROSPosition(GameObject.FindGameObjectWithTag("Robot").transform.position);
                QuaternionMsg currentRotation = CoordinateConverter.UnityToROSRotation(GameObject.FindGameObjectWithTag("Robot").transform.rotation);
                goalQueue.Enqueue(new Pose(currentPosition, currentRotation));
            }
            skippedGoal = true;
        }
    }

}