using UnityEngine;
using RosMessageTypes.Geometry;

public static class CoordinateConverter
{
    /// <summary>
    /// Convert Unity position to ROS (Z-up, Right-handed)
    /// Unity (x, y, z) → ROS (z, -x, y)
    /// </summary>
    public static PointMsg UnityToROSPosition(Vector3 unityPosition)
    {
        return new PointMsg(
            unityPosition.z,     // ROS X = Unity Z
            -unityPosition.x,    // ROS Y = -Unity X
            unityPosition.y      // ROS Z = Unity Y
        );
    }

    /// <summary>
    /// Convert ROS position to Unity (Left-handed, Y-up)
    /// ROS (x, y, z) → Unity (-y, z, x)
    /// </summary>
    public static Vector3 ROSToUnityPosition(PointMsg rosPosition)
    {
        return new Vector3(
            (float)-rosPosition.y,      // Unity X = -ROS Y
            (float)rosPosition.z,       // Unity Y = ROS Z
            (float)rosPosition.x        // Unity Z = ROS X
        );
    }

    /// <summary>
    /// Convert Unity rotation to ROS rotation
    /// Apply rotation to match coordinate axis: -90 degrees around X
    /// </summary>
    public static QuaternionMsg UnityToROSRotation(Quaternion unityRotation)
    {
        Quaternion correctedRotation = Quaternion.Euler(-90f, 0f, 0f) * unityRotation;

        return new QuaternionMsg(
            correctedRotation.x,
            correctedRotation.y,
            correctedRotation.z,
            correctedRotation.w
        );
    }

    /// <summary>
    /// Convert ROS rotation to Unity rotation
    /// Invert the coordinate correction used when publishing to ROS
    /// </summary>
    public static Quaternion ROSToUnityRotation(QuaternionMsg rosRotation)
    {
        Quaternion q = new Quaternion(
            (float)rosRotation.x,
            (float)rosRotation.y,
            (float)rosRotation.z,
            (float)rosRotation.w
        );

        return Quaternion.Inverse(Quaternion.Euler(-90f, 0f, 0f)) * q;
    }
}
