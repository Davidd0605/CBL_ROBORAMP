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
    public string receiveTopic = "/topic_queuing";

    private taskCompletionManager taskCompletion;
    private GenerateGuide guideGenerator;

    [SerializeField]
    private ButtonAction nextButton;

    // Struct here to make it easy to add time stamps for our requests or type (getting on the train of off)
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
            if (other.position == null)
            {
                return false;
            }

            if (other.position.x == this.position.x &&
                other.position.y == this.position.y &&
                other.position.z == this.position.z &&
                other.rotation.x == this.rotation.x &&
                other.rotation.y == this.rotation.y &&
                other.rotation.z == this.rotation.z &&
                other.rotation.w == this.rotation.w)
                return true;
            return false;
        }
    }

    private Queue<Pose> goalQueue = new Queue<Pose>();
    private Pose currentGoal;
    private Pose lastInserted;
    private GoalPosePublisher publisher;
    private Vector3 unityGoal;

    void Start()
    {
        //ghetto style
        lastInserted.position = null;
        lastInserted.rotation = null;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseStampedMsg>(receiveTopic, GoalCallback);

        nextButton = GetComponent<ButtonAction>();
        taskCompletion = GetComponent<taskCompletionManager>();
        guideGenerator = GetComponent<GenerateGuide>();
        publisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
    }

    void Update()
    {
        //Debug.Log("Number of entries in the queue currently: " + goalQueue.Count);
        if (goalQueue.Count > 0 &&
            ((!taskCompletion.hasGoal && !taskCompletion.isSleeping)
            || Input.GetKeyDown(KeyCode.G)
            || nextButton.pressedNext))
        {
            currentGoal = goalQueue.Dequeue();
            nextButton.pressedNext = false;
            unityGoal = CoordinateConverter.ROSToUnityPosition(currentGoal.position);

            //Debug.LogWarning("Current goal set: " + unityGoal);

            taskCompletion.setCurrentGoal(unityGoal);
            switch (currentMode) {
                case pathingMode.physicalOnly:
                    publisher.PublishPose(currentGoal.position, currentGoal.rotation);
                    break;
                case pathingMode.virtualPathing:
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

    public int getGoalQueueCount()
    {
        return this.goalQueue.Count;
    }

    public Vector3 getUnityGoal()
    {
        return this.unityGoal;
    }
}