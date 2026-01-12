using UnityEngine;
using System.Collections.Generic;
using IsoTools;

public class MovingFloor : MonoBehaviour
{
    public List<Vector3> targetPositions; // 使用 Iso 坐标
    public float speed = 2f;              // 移动速度
    private int currentTargetIndex = 0;   // 当前目标索引
    private IsoObject isoObject;          // 当前 IsoObject

    public float stopDuration = 1f; // 每个点的停留时间
    private float stopTimer = 0f;
    void Start()
    {
        isoObject = GetComponent<IsoObject>();
        if (isoObject == null)
        {
            Debug.LogError("Missing IsoObject component!");
        }

        if (targetPositions == null || targetPositions.Count == 0)
        {
            Debug.LogError("No target positions assigned!");
        }
    }
    

    void Update()
    {
        if (isoObject == null || targetPositions == null || targetPositions.Count == 0)
        {
            return;
        }

        if (stopTimer > 0f)
        {
            stopTimer -= Time.deltaTime;
            return; // 停留期间不移动
        }

        // 获取当前目标点
        Vector3 targetPosition = targetPositions[currentTargetIndex];

        // 移动逻辑（同上）
        Vector3 direction = (targetPosition - isoObject.position).normalized;
        Vector3 movement = direction * speed * Time.deltaTime;

        if (Vector3.Distance(isoObject.position, targetPosition) < movement.magnitude)
        {
            isoObject.position = targetPosition;
            currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Count;
            stopTimer = stopDuration; // 开始停留计时
        }
        else
        {
            isoObject.position += movement;
        }
    }


}
