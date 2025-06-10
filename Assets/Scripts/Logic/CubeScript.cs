using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Nav;

public class CubeScript : MonoBehaviour
{
    private GameObject baseFootprint;
    void Start()
    {
        baseFootprint = GameObject.Find("base_footprint");
    }

    private void Update()
    {
        if(baseFootprint != null)
        {
            transform.position = new Vector3(baseFootprint.transform.position.x, 0.2f, baseFootprint.transform.position.z);
            transform.rotation = baseFootprint.transform.rotation;
        } else
        {
            baseFootprint = GameObject.Find("base_footprint");
        }
    }
}