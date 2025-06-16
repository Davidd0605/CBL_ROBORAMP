using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynchronizePositionScript : MonoBehaviour
{

    private Transform self;

    [Header("Transform to Synchronize with")]
    public Transform toSynchronizeWith;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        self.position = toSynchronizeWith.position;
        Vector3 newPosition = self.position;
        newPosition.y += 0.1f;
        self.position = newPosition;
    }
}
