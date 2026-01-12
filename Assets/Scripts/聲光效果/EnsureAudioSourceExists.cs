using UnityEngine;

public class EnsureAudioSourceExists : MonoBehaviour
{
    [Header("要檢查的名稱")]
    private string targetName = "Music Audio Source"; // 場景中要檢查的物件名稱

    [Header("要新增的Prefab")]
    public GameObject prefabToAdd; // 要新增的Prefab，請從Inspector拖進來
    private void Awake()
    {
        Application.targetFrameRate = -1;
    }
    void Start()
    {
        // 嘗試尋找該名稱的物件
        GameObject existing = GameObject.Find(targetName);

        if (existing == null)
        {
            if (prefabToAdd != null)
            {
                // 如果場景中沒有這個物件就從Prefab新增
                GameObject newObj = Instantiate(prefabToAdd);
                newObj.name = targetName;
                Debug.Log($"未找到「{targetName}」，已從Prefab新增到場景中。");
            }
            else
            {
                Debug.LogWarning("尚未指定要新增的Prefab！");
            }
        }
        else
        {
            Debug.Log($"場景中已存在「{targetName}」，不重複新增。");
        }
    }
}