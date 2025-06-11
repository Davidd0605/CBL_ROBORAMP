using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCooldown : MonoBehaviour
{
    public bool isSleeping;

    [SerializeField]
    private float sleepingCooldown = 5.0f;
    void Start()
    {
        isSleeping = false;   
    }

    public void startSleeping()
    {
        //TODO IN UI
        Debug.Log("START COOLDOWN");
        isSleeping = true;
        StartCoroutine(SleepCooldownRoutine());
    }
    private IEnumerator SleepCooldownRoutine()
    {
        yield return new WaitForSeconds(sleepingCooldown);
        isSleeping = false;
        //TODO IN UI
        Debug.Log("COOLDOWN ENDED");
    }
}
