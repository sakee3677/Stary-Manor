using UnityEngine;
using IsoTools;
using IsoTools.Physics;
using System.Collections.Generic;


public class GroundProjection : MonoBehaviour
{
    
    public Transform player;               // 玩家
    public IsoObject playerIso;            // 玩家 IsoObject
    public GameObject dotPrefab;           // 點的 Prefab
    public LayerMask groundLayer;          // 地板 Layer
    public IsoWorld isoWorld;              // 手動指定 IsoWorld
    public Vector3 rayOriginOffset = Vector3.zero; // 射線起點偏移
    public float dotSpacing = 0.2f;        // 點之間的間距
    public float maxDistance = 50f;        // 最大射線距離

    private List<IsoObject> dots = new List<IsoObject>();
    private bool isActive = true;          // 射線啟用狀態

    void Start()
    {
        if (playerIso == null)
        {
            Debug.LogError("請設定 playerIso！");
            return;
        }

        if (dotPrefab == null)
        {
            Debug.LogError("請設定 dotPrefab！");
            return;
        }

        if (isoWorld == null)
        {
            Debug.LogError("請設定 IsoWorld！");
            return;
        }

        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        if (!isActive || playerIso == null || dotPrefab == null || isoWorld == null) return;

        // **清空舊的點**
        ClearDots();

        // **進行 Iso 座標射線檢測**
        Vector3 rayOrigin = playerIso.position + rayOriginOffset;
        Vector3 rayDirection = new Vector3(0, 0, -1f); // 垂直向下

        IsoRaycastHit hitInfo;
        bool isHit = IsoPhysics.Raycast(new Ray(rayOrigin, rayDirection), out hitInfo, maxDistance, groundLayer.value);

        if (isHit)
        {
            Vector3 start = rayOrigin;
            Vector3 end = hitInfo.point;
            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            // **根據間距生成點**
            for (float d = dotSpacing; d < distance; d += dotSpacing)
            {
                Vector3 pointPosition = start + direction * d;

                // **生成點並設定父物件為 IsoWorld**
                GameObject dotInstance = Instantiate(dotPrefab);
                dotInstance.transform.SetParent(isoWorld.transform, false); // 設為 IsoWorld 子物件

                IsoObject dotIso = dotInstance.GetComponent<IsoObject>();
                if (dotIso != null)
                {
                    dotIso.position = pointPosition;
                    dots.Add(dotIso);
                }
                else
                {
                    Debug.LogError("dotPrefab 缺少 IsoObject 組件！");
                    Destroy(dotInstance);
                }
            }
        }
    }
    public void SetDotColor()
    {
        Debug.Log("改變點顏色");
        foreach (IsoObject dot in dots)
        {
            SpriteRenderer spriteRenderer = dot.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sharedMaterial != null)
            {
                spriteRenderer.sharedMaterial.SetFloat("_Glow", 4f);
                spriteRenderer.sharedMaterial.SetFloat("_HsvSaturation", 1f);
            }
        }
    }

    public void UNSetDotColor()
    {
        foreach (IsoObject dot in dots)
        {
            SpriteRenderer spriteRenderer = dot.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sharedMaterial != null)
            {
                spriteRenderer.sharedMaterial.SetFloat("_Glow", 0f);
                spriteRenderer.sharedMaterial.SetFloat("_HsvSaturation", 0f);
            }
        }
    }



    public void EnableProjection()
    {
        isActive = true;
    }

    public void DisableProjection()
    {
        isActive = false;
        ClearDots();
    }

    private void ClearDots()
    {
        foreach (IsoObject dot in dots)
        {
            if (dot != null)
            {
                Destroy(dot.gameObject);
            }
        }
        dots.Clear();
    }
}
