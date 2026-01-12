using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeActiveSprite : MonoBehaviour
{
    [Header("Sprites to Disable (On Enter)")]
    public SpriteRenderer[] disableSprites;

    [Header("Sprites to Fade (On Enter)")]
    public SpriteRenderer[] fadeSprites;

    [Header("Fade Settings")]
    [Range(0f, 1f)]
    public float targetAlpha = 0.3f;

    [Header("Sprites to Enable (On Enter)")]
    public SpriteRenderer[] spritesToEnable;

    [Header("Sprites to Disable (On Enter)")]
    public SpriteRenderer[] spritesToDisable;

    // 儲存原始透明度
    private float[] originalAlphas;
    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    // Start is called before the first frame update
    private void Awake()
    {
        originalAlphas = new float[fadeSprites.Length];
        for (int i = 0; i < fadeSprites.Length; i++)
        {
            if (fadeSprites[i] != null)
                originalAlphas[i] = fadeSprites[i].color.a;
        }
    }

    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        if (isoCollider.CompareTag("Player"))
        {
            // 關閉 disableSprites
            foreach (var sr in disableSprites)
                if (sr != null) sr.enabled = false;

            // 改透明度
            for (int i = 0; i < fadeSprites.Length; i++)
            {
                if (fadeSprites[i] != null)
                {
                    Color c = fadeSprites[i].color;
                    c.a = targetAlpha;
                    fadeSprites[i].color = c;
                }
            }

            // 開啟 spritesToEnable
            foreach (var sr in spritesToEnable)
                if (sr != null) sr.enabled = true;

            // 關閉 spritesToDisable
            foreach (var sr in spritesToDisable)
                if (sr != null) sr.enabled = false;
        }
    }


void OnIsoTriggerExit(IsoCollider isoCollider)
{
    if (isoCollider.CompareTag("Player"))
    {
        // 恢復 disableSprites
        foreach (var sr in disableSprites)
            if (sr != null) sr.enabled = true;

        // 恢復透明度
        for (int i = 0; i < fadeSprites.Length; i++)
        {
            if (fadeSprites[i] != null)
            {
                Color c = fadeSprites[i].color;
                c.a = originalAlphas[i];
                fadeSprites[i].color = c;
            }
        }

        // 關閉 spritesToEnable
        foreach (var sr in spritesToEnable)
            if (sr != null) sr.enabled = false;

        // 開啟 spritesToDisable
        foreach (var sr in spritesToDisable)
            if (sr != null) sr.enabled = true;
    }
}
}
    
