using System.Collections.Generic;
using UnityEngine;

public class StoneColorAssigner : MonoBehaviour
{
    [Header("石柱列表（由左到右）")]
    public List<GameObject> stones;

    [Header("每根石柱應該顯示的目標顏色")]
    public List<Sprite> targetColorSprites;

    [Header("可選擇的顏色（會循環切換）")]
    public List<Sprite> applyColorOptions;

    [Header("預設圖片")]
    public Sprite defaultSprite;

    public NextLevel Door;

    private Dictionary<GameObject, int> stoneColorIndex = new Dictionary<GameObject, int>();

    void Start()
    {
        foreach (var stone in stones)
        {
            stoneColorIndex[stone] = -1; // 初始設為預設圖片
        }
    }

    public void TouchStone(GameObject stone)
    {
        if (!stoneColorIndex.ContainsKey(stone)) return;

        int index = stoneColorIndex[stone];
        index = (index + 1) % applyColorOptions.Count;
        stoneColorIndex[stone] = index;

        Sprite newColor = applyColorOptions[index];

        // 更換圖片
        SpriteRenderer sr = stone.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = newColor;
        }

        // 顯示對應光
        StoneLightController lightController = stone.GetComponentInChildren<StoneLightController>();
        if (lightController != null)
        {
            lightController.ShowColorLight(index);
        }

        CheckResult(); // 每次改顏色都檢查
    }

    private void CheckResult()
    {
        for (int i = 0; i < stones.Count; i++)
        {
            var stone = stones[i];
            SpriteRenderer sr = stone.GetComponentInChildren<SpriteRenderer>();

            if (sr == null || sr.sprite != targetColorSprites[i])
            {
                return; // 有一根不對就不通過
            }
        }

        Debug.Log("正確組合！");
        Door.openDoor();
    }

    public void ResetStones()
    {
        foreach (var stone in stones)
        {
            SpriteRenderer sr = stone.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = defaultSprite;
            }

            StoneLightController lightController = stone.GetComponentInChildren<StoneLightController>();
            if (lightController != null)
            {
                lightController.ResetLights();
            }

            stoneColorIndex[stone] = -1;
        }
    }
}
