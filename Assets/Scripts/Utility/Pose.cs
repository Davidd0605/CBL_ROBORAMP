using RosMessageTypes.Geometry;

/**
 * 
 * Class for the Pose data structure.
 * Contains the rotation and position for a Pose.
 * These are stored in the ROS2 coordinate system.
 * 
 */
public class Pose
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

        if (other.position.x == this.position.x &&
                other.position.y == this.position.y &&
                other.position.z == this.position.z &&
                other.rotation.x == this.rotation.x &&
                other.rotation.y == this.rotation.y &&
                other.rotation.z == this.rotation.z &&
                other.rotation.w == this.rotation.w)
        {
            return true;
        }
        return false;
    }
}
