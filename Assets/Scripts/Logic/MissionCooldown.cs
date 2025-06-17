using System.Collections;
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
        isSleeping = true;
        StartCoroutine(SleepCooldownRoutine());
    }
    private IEnumerator SleepCooldownRoutine()
    {
        yield return new WaitForSeconds(sleepingCooldown);
        isSleeping = false;
    }
}
