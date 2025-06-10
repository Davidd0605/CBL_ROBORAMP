using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotModel : MonoBehaviour
{

    public GameObject rightWheel;
    public GameObject leftWheel;

    private GameObject leftWheelLink;
    private GameObject rightWheelLink;
    private GameObject baseLink;

    // Update is called once per frame
    void Update()
    {

        if (baseLink != null)
        {
            transform.position = new Vector3(baseLink.transform.position.x, 0.2f, baseLink.transform.position.z);
            transform.rotation = baseLink.transform.rotation;
        }
        else
        {
            baseLink = GameObject.Find("base_footprint");
        }

    }
}
