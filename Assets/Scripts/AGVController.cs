using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.UrdfImporter.Control;
using RosMessageTypes.Nav;
using Codice.Client.BaseCommands.WkStatus.Printers;



namespace RosSharp.Control
{
    public enum ControlMode { Keyboard, ROS, BaseLink};

    public class AGVController : MonoBehaviour
    {
        public GameObject wheel1;
        public GameObject wheel2;
        public ControlMode mode = ControlMode.ROS;

        private ArticulationBody wA1;
        private ArticulationBody wA2;

        public float maxLinearSpeed = 2; //  m/s
        public float maxRotationalSpeed = 1;//
        public float wheelRadius = 0.033f; //meters
        public float trackWidth = 0.288f; // meters Distance between tyres
        public float forceLimit = 10;
        public float damping = 10;

        public float ROSTimeout = 0.5f;
        private float lastCmdReceived = 0f;

        ROSConnection ros;
        private RotationDirection direction;

        private float rosLinear = 0f;
        private float rosAngular = 0f;

        //odom syncing with physical twin
        public string odomTopic = "/odom";
        private GameObject baseFootprint;
        void Start()
        {
            wA1 = wheel1.GetComponent<ArticulationBody>();
            wA2 = wheel2.GetComponent<ArticulationBody>();
            SetParameters(wA1);
            SetParameters(wA2);
            ros = ROSConnection.GetOrCreateInstance();
            ros.Subscribe<TwistMsg>("cmd_vel", ReceiveROSCmd);
            ros.Subscribe<OdometryMsg>(odomTopic, OdomCallback);
        }

        void ReceiveROSCmd(TwistMsg cmdVel)
        {
            if (cmdVel.linear.x==0 && cmdVel.angular.z==0)
            {
                rosLinear =0;
                rosAngular=0;
            }

            else if (cmdVel.linear.x<0)
            {
                rosLinear = (float)((float)cmdVel.linear.x-0.02);
            } 
            else if (cmdVel.angular.z<0)
            { 
                rosAngular =  (float)((float)cmdVel.angular.z-0.18);
            }
            else if (cmdVel.linear.x>0)
            {
                rosLinear = (float)((float)cmdVel.linear.x+0.02); //(float)cmdVel.linear.x ;//added 0.05 to account for friction 
            }
            else if (cmdVel.angular.z>0)
            {
                 rosAngular = (float)((float)cmdVel.angular.z+0.18); //(float)cmdVel.angular.z; //added 0.03 to acct for friction
            }
            lastCmdReceived = Time.time;
        }

        void OdomCallback(OdometryMsg msg)
        {
            var pos = msg.pose.pose.position;
            var ori = msg.pose.pose.orientation;

            Vector3 unityPosition = new Vector3(
                (float)pos.y * -1f,
                0f,                  
                (float)pos.x         
            );

            Quaternion unityRotation = new Quaternion(
                -(float)ori.x,
                -(float)ori.z,
                (float)ori.y,
                (float)ori.w
            );


        }
        void FixedUpdate()
        {
            switch (mode)
            {
                case ControlMode.Keyboard:
                    KeyBoardUpdate();
                    break;
                case ControlMode.ROS:
                    ROSUpdate();
                    break;
                case ControlMode.BaseLink:
                    Debug.LogWarning("Action not supported yet.");
                    break;
            }
         
        }

        private void SetParameters(ArticulationBody joint)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.forceLimit = forceLimit;
            drive.damping = damping;
            joint.xDrive = drive;
        }

        private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
        {
            ArticulationDrive drive = joint.xDrive;
            if (float.IsNaN(wheelSpeed))
            {
                drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
            }
            else
            {
                drive.targetVelocity = wheelSpeed;
            }
            joint.xDrive = drive;
        }

        private void KeyBoardUpdate()
        {
            float moveDirection = Input.GetAxis("Vertical");

            float inputSpeed;
            float inputRotationSpeed;
            if (moveDirection > 0)
            {
                inputSpeed = maxLinearSpeed;
            }
            else if (moveDirection < 0)
            {
                inputSpeed = maxLinearSpeed * -1;
            }
            else
            {
                inputSpeed = 0;
            }

            float turnDirction = Input.GetAxis("Horizontal");
            if (turnDirction > 0)
            {
                inputRotationSpeed = maxRotationalSpeed;
            }
            else if (turnDirction < 0)
            {
                inputRotationSpeed = maxRotationalSpeed * -1;
            }
            else
            {
                inputRotationSpeed = 0;
            }
            //Actually moves URDF
            RobotInput(inputSpeed, inputRotationSpeed);
        }



        private void ROSUpdate()
        {
        
            if (Time.time - lastCmdReceived > ROSTimeout)
            {
                rosLinear = 0f;
                rosAngular = 0f;
            }
            Debug.Log("linear: " + rosLinear);
            Debug.Log("angular: " + rosAngular);
            RobotInput(rosLinear, -rosAngular);
        }

        private void RobotInput(float speed, float rotSpeed) // m/s and rad/s
        {
            if (speed > maxLinearSpeed)
            {
                speed = maxLinearSpeed;
            }
            if (rotSpeed > maxRotationalSpeed)
            {
                rotSpeed = maxRotationalSpeed;
            }
            float wheel1Rotation = (speed / wheelRadius);
            float wheel2Rotation = wheel1Rotation;
            float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
            if (rotSpeed != 0)
            {
                wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
                wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
            }
            else
            {
                wheel1Rotation *= Mathf.Rad2Deg;
                wheel2Rotation *= Mathf.Rad2Deg;
            }
            SetSpeed(wA1, wheel1Rotation);
            SetSpeed(wA2, wheel2Rotation);
        }
    }
}
