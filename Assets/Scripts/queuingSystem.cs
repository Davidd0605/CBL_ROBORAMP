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
    ROSConnection ros;
    public string receiveTopic = "/topic_queuing";

    private taskCompletionManager taskCompletion;
    private GoalPosePublisher goalPosePublisher;
    private GoalReachableSubscriber goalReachable;
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
    }

    private Queue<Pose> goalQueue = new Queue<Pose>();
    private Pose currentGoal;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseStampedMsg>(receiveTopic, GoalCallback);

        taskCompletion = GetComponent<taskCompletionManager>();
        goalPosePublisher = GameObject.FindGameObjectWithTag("publisher").GetComponent<GoalPosePublisher>();
        goalReachable = GameObject.FindGameObjectWithTag("logic").GetComponent<GoalReachableSubscriber>();
    }

    void Update()
    {
            if ( (goalQueue.Count > 0 && taskCompletion.hasGoal == false)  || Input.GetKeyDown(KeyCode.G))
            {
                currentGoal = goalQueue.Dequeue();
                goalPosePublisher.PublishPose(currentGoal.position, currentGoal.rotation);
            }
            else
            {
               Debug.LogWarning($"Cannot send goal: Queue Count = {goalQueue.Count}, hasGoal = {taskCompletion.hasGoal}");
            }
    }

    void GoalCallback(PoseStampedMsg msg)
    {
        Pose newGoal = new Pose(msg.pose.position, msg.pose.orientation);
        goalQueue.Enqueue(newGoal);
    }
}