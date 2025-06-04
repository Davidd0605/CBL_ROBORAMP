using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGuide : MonoBehaviour
{
    public GameObject guide;
    public GameObject robot;

    public void generate(Vector3 target)
    {
        Instantiate(guide, robot.transform.position, Quaternion.identity);
        guide.GetComponent<Madness>().SetTarget(target);
    }
}
