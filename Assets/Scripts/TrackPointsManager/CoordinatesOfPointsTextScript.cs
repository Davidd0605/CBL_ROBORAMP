using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoordinatesOfPointsTextScript : MonoBehaviour
{

    private Text text;

    [Header("Track Endpoints")]
    public Transform track1Start;
    public Transform track1End;
    public Transform track2Start;
    public Transform track2End;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "TRACK 1 (T1) COORDINATES:"
            + "\nSTART: " + track1Start.position.ToString()
            + "\nEND: " + track1End.position.ToString()
            + "\n\nTRACK 2 (T2) COORDINATES:"
            + "\nSTART: " + track2Start.position.ToString()
            + "\nEND: " + track2End.position.ToString();
    }
}
