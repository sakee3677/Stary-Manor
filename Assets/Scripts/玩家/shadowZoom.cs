using UnityEngine;
using IsoTools;
using IsoTools.Physics;

public class shadowZoom : MonoBehaviour
{
    public Transform player; // 角色 Transform
    public SpriteRenderer shadowSprite; // 影子的 SpriteRenderer
    public Material shadowMaterial;
    public LayerMask groundLayer;

    public float initialZoom = 0.9f; // 影子的初始 Zoom 值
    public float minZoom = 1f; // 影子最小大小
    public float maxDistance = 2.5f; // 角色與地板的最大距離
    public float maxRaycastDistance = 50f; // 射線最大距離

    // IsoShadowFollow
    public IsoObject playerIso; // 角色的 IsoObject
    public IsoObject shadowIso; // 影子的 IsoObject (要手動在 Inspector 設定)
    public float x, y;
    public Vector3 shadowRayOffset = new Vector3(0f, 0f, 1f);

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground"); // 確保正確設定

        if (shadowSprite != null)
        {
            shadowMaterial = shadowSprite.material;
        }
        else
        {
            Debug.LogError("shadowSprite 未設置！");
        }
    }

    void Update()
    {
        if (player == null || shadowMaterial == null || playerIso == null || shadowIso == null) return;

        // **正確解構 tuple**
        (Vector3 hitPoint, float groundDistance) = GetGroundInfo();
        float zoomValue = Mathf.Lerp(initialZoom, minZoom, groundDistance / maxDistance);
        zoomValue = Mathf.Clamp(zoomValue, initialZoom, minZoom);
        //Debug.Log($"Ground Distance: {groundDistance}, Ground Height: {hitPoint.z}, Zoom: {zoomValue}");

        // **套用 Shader 變數**
        shadowMaterial.SetFloat("_ZoomUvAmount", zoomValue);

        // **讓影子貼合地面**
        shadowIso.position = new Vector3(playerIso.position.x + x, playerIso.position.y + y, hitPoint.z);
    }

    (Vector3, float) GetGroundInfo()
    {
        IsoObject isoObject = player.GetComponent<IsoObject>();
        if (isoObject == null)
        {
            Debug.LogError("找不到 IsoObject，請確認角色是否有附加！");
            return (Vector3.zero, maxDistance);
        }

        // 定義射線的起點和 Iso 世界方向
        Vector3 offset = shadowRayOffset;// 向上偏移 1 單位，以防止射線從內部發射
        Vector3 rayOrigin = isoObject.position + offset; // 使用 IsoObject 位置發射射線
        Vector3 rayDirection = new Vector3(0, 0, -1f).normalized; // 向 Iso 世界的「下方」發射

        float testRaycastDistance = 20f; // 增加 Raycast 偵測範圍
        IsoRaycastHit hitInfo;

        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * maxRaycastDistance, Color.blue, 5f);

        bool isHit = IsoPhysics.Raycast(
            new Ray(rayOrigin, rayDirection),
            out hitInfo,
            testRaycastDistance,
            groundLayer.value
        );

        if (isHit)
        {
            float groundHeight = hitInfo.point.z;
            float distance = Mathf.Max(isoObject.position.z - groundHeight, 0);
            return (hitInfo.point, distance);
        }

        Debug.LogWarning("未偵測到地板，返回最大距離");
        return (Vector3.zero, maxDistance);
    }
}