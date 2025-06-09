using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{

    public bool pressedNext = false;
    public void switchToNextTask()
    {
        pressedNext = true;
    }
}
