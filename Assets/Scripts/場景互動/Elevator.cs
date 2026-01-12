using IsoTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public List<Vector3> targetPositions;
    public float speed = 2f;
    public float startDelay = 2f;
    public bool isActivated = false; // 由壓力板控制

    public GameObject activeLight;
    private bool forceDeactivate = false;
    private IsoObject isoObject;
    private Coroutine blinkCoroutine;
    private enum ElevatorState { Idle, PreparingMove, MovingToTarget, PreparingReturn, Returning, Disabled }
    private ElevatorState currentState = ElevatorState.Disabled;

    private int direction = 1; // 1:去  -1:回

    void Start()
    {
        isoObject = GetComponent<IsoObject>();
        if (isoObject == null || targetPositions.Count < 2)
            Debug.LogError("IsoObject or targetPositions not properly set.");

    }

    void Update()
    {
        if (currentState == ElevatorState.MovingToTarget)
        {
            Vector3 target = direction == 1 ? targetPositions[1] : targetPositions[0];
            Vector3 directionVec = (target - isoObject.position).normalized;
            Vector3 movement = directionVec * speed * Time.deltaTime;

            if (Vector3.Distance(isoObject.position, target) <= movement.magnitude)
            {
                isoObject.position = target;

                // 到達後切換到下一階段
                if (Vector3.Distance(isoObject.position, target) <= movement.magnitude)
                {
                    isoObject.position = target;

                    if (currentState == ElevatorState.MovingToTarget)
                    {
                        StartCoroutine(PrepareReturn());
                    }
                    else if (currentState == ElevatorState.Returning)
                    {
                        if (forceDeactivate)
                        {
                            currentState = ElevatorState.Disabled;
                            forceDeactivate = false;
                        }
                        else
                        {
                            StartCoroutine(PrepareMove());
                        }
                    }
                }
            }
            else
            {
                isoObject.position += movement;
            }
        }
     else   if (currentState == ElevatorState.Returning)
        {
            Vector3 target = targetPositions[0];
            Vector3 directionVec = (target - isoObject.position).normalized;
            Vector3 movement = directionVec * speed * Time.deltaTime;

            if (Vector3.Distance(isoObject.position, target) <= movement.magnitude)
            {
                isoObject.position = target;

                if (forceDeactivate)
                {
                    currentState = ElevatorState.Disabled;
                    forceDeactivate = false; // 重置旗標
                    if (isActivated)
                    {
                        StartCoroutine(PrepareMove()); // 自動再出發
                    }

                    return;
                   
                }

                StartCoroutine(PrepareMove());
            }
            else
            {
                isoObject.position += movement;
            }
        }
    }

    public void ToggleActivation()
    {
        isActivated = true;
        if (currentState == ElevatorState.Disabled)
        {
            StartCoroutine(PrepareMove());
        }
    }

    public void UnToggleActivation()
    {
        isActivated = false;
        forceDeactivate = true;
        StopAllCoroutines();
        activeLight.SetActive(false);
        currentState = ElevatorState.Returning;
        direction = -1;
    }

    IEnumerator PrepareMove()
    {
        currentState = ElevatorState.PreparingMove;
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkLight());
        yield return blinkCoroutine;

        activeLight.SetActive(true);
        direction = 1;
        currentState = ElevatorState.MovingToTarget;
    }
    IEnumerator PrepareReturn()
    {
        currentState = ElevatorState.PreparingReturn;
        yield return StartCoroutine(BlinkLight());
        activeLight.SetActive(true);
        direction = -1;
        currentState = ElevatorState.Returning;
    }

    IEnumerator BlinkLight()
    {
        float blinkDuration = startDelay;
        float elapsed = 0f;
        float blinkSpeed = 1f;

        while (elapsed < blinkDuration)
        {
            activeLight.SetActive(!activeLight.activeSelf);
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed;
            blinkSpeed = Mathf.Max(0.2f, blinkSpeed * 0.2f);
        }

        activeLight.SetActive(true);
    }
}
