using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicPillar : MonoBehaviour
{

    public Sprite newSprite; // 指定的目標 Sprite
    public GameObject arrow;
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        // 獲取子項目中的 SpriteRenderer 組件
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("未找到 SpriteRenderer！請確認子項目中是否存在 SpriteRenderer 組件。");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changesprite()
    {
        // 更改 Sprite
        if (newSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
            arrow.SetActive(true);
        }
    }
}
