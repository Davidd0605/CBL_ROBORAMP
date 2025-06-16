using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackPointSystemScript : MonoBehaviour
{
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    private Plane plane = new Plane(Vector3.down, 0);

    public Transform? toBeSet = null;
    public bool track1Visible = false;
    public bool track2Visible = false;

    [Header("Track Endpoints")]
    public Transform track1Start;
    public Transform track1End;
    public Transform track2Start;
    public Transform track2End;

    [Header("Visualization Objects")]
    public GameObject visTrack1Start;
    public GameObject visTrack1End;
    public GameObject visTrack2Start;
    public GameObject visTrack2End;
    public LineRenderer visTrack1Line;
    public LineRenderer visTrack2Line;

    [Header("Text")]
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        visTrack1Line.positionCount = 2;
        visTrack2Line.positionCount = 2;

        visTrack1Start.GetComponent<MeshRenderer>().material.color = Color.red;
        visTrack1End.GetComponent<MeshRenderer>().material.color = Color.red;
        visTrack2Start.GetComponent<MeshRenderer>().material.color = Color.blue;
        visTrack2End.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        string textToBeSet = "";
        string textTrack1Visible = track1Visible ? "Visible" : "Hidden";
        string textTrack2Visible = track2Visible ? "Visible" : "Hidden";

        if (toBeSet == null)
        {
            textToBeSet = "Nothing";
        }
        else if (toBeSet == track1Start)
        {
            textToBeSet = "Track 1 Start";
        }
        else if (toBeSet == track1End)
        {
            textToBeSet = "Track 1 End";
        }
        else if (toBeSet == track2Start)
        {
            textToBeSet = "Track 2 Start";
        }
        else if (toBeSet == track2End)
        {
            textToBeSet = "Track 2 End";
        }
        text.text = "Currently setting: " + textToBeSet + "\nTrack 1: " + textTrack1Visible + "\nTrack 2: " + textTrack2Visible + "\n";

        visTrack1Line.SetPosition(0, visTrack1Start.transform.position);
        visTrack1Line.SetPosition(1, visTrack1End.transform.position);
        visTrack2Line.SetPosition(0, visTrack2Start.transform.position);
        visTrack2Line.SetPosition(1, visTrack2End.transform.position);

        visTrack1Line.enabled = track1Visible;
        visTrack1Start.GetComponent<MeshRenderer>().enabled = track1Visible;
        visTrack1End.GetComponent<MeshRenderer>().enabled = track1Visible;

        visTrack2Line.enabled = track2Visible;
        visTrack2Start.GetComponent<MeshRenderer>().enabled = track2Visible;
        visTrack2End.GetComponent<MeshRenderer>().enabled = track2Visible;

        screenPosition = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.X))
        {
            Ray ray = Camera.allCameras[0].ScreenPointToRay(screenPosition);
            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
                worldPosition.y = 0.0f;
                if (toBeSet != null)
                {
                    toBeSet.position = worldPosition;
                }
            }
        }
    }

    public void SetTrack1Start()
    {
        toBeSet = track1Start;
    }

    public void SetTrack1End()
    {
        toBeSet = track1End;
    }

    public void SetTrack2Start()
    {
        toBeSet = track2Start;
    }

    public void SetTrack2End()
    {
        toBeSet = track2End;
    }

    public void OnToggleTrack1Visibility()
    {
        track1Visible = !track1Visible;
    }

    public void OnToggleTrack2Visibility()
    {
        track2Visible = !track2Visible;
    }
}
