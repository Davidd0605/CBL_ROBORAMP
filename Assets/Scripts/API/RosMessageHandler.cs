using UnityEngine;

public class RosMessageHandler : MonoBehaviour
{

    [Header("Track Endpoints")]
    public Transform track1Start;
    public Transform track1End;
    public Transform track2Start;
    public Transform track2End;
    
    public RosPublisherExample publisher;
    
    void Start()
    {
    	
    }

    public void OnRosMessageReceived(string message)
    {
        string[] parts = message.Split(':');
        int.TryParse(parts[0], out int track);
        float.TryParse(parts[1], out float distanceAlong);

        Transform start = null;
        Transform end = null;

        switch (track)
        {
            case 1:
                start = track1Start;
                end = track1End;
                break;
            case 2:
                start = track2Start;
                end = track2End;
                break;
        }

        if (start == null || end == null)
        {
            Debug.LogError($"Start or end transform is not assigned for track {track}");
            return;
        }

        Vector3 platformDirection = (end.position - start.position).normalized;

        Vector3 targetLocation = Vector3.Lerp(start.position, end.position, distanceAlong);
        Debug.Log($"Target robot position: {targetLocation}");

        Vector3 targetRotation = Vector3.Cross(platformDirection, Vector3.up);
        Debug.Log($"Target robot rotation: {targetRotation}");

        publisher.sendGoal(targetLocation, Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z));
    }
}
