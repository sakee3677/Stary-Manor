using UnityEngine;
using System.Collections;

public class teacherDraw : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material material;
    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;

    // 可設定不同 shader 效果的變化範圍
    [System.Serializable]
    public class ShaderPropertyFade
    {
        public string propertyName;
        public float fromValue;
        public float toValue;
    }

    public ShaderPropertyFade[] fadeInProperties;
    public ShaderPropertyFade[] fadeOutProperties;

    public void FadeIn()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(1f, fadeInProperties));
        DialogueManager.Instance.RegisterEventCallback();
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(0f, fadeOutProperties));
        DialogueManager.Instance.RegisterEventCallback();
    }

    private IEnumerator FadeTo(float targetAlpha, ShaderPropertyFade[] shaderFades)
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float time = 0f;

        // 儲存起始值
        float[] startValues = new float[shaderFades.Length];
        for (int i = 0; i < shaderFades.Length; i++)
        {
            startValues[i] = material.GetFloat(shaderFades[i].propertyName);
        }

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);

            // 淡入淡出
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);

            // Shader 效果
            for (int i = 0; i < shaderFades.Length; i++)
            {
                float value = Mathf.Lerp(shaderFades[i].fromValue, shaderFades[i].toValue, t);
                material.SetFloat(shaderFades[i].propertyName, value);
            }

            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, targetAlpha);
        for (int i = 0; i < shaderFades.Length; i++)
        {
            material.SetFloat(shaderFades[i].propertyName, shaderFades[i].toValue);
        }
    }
}
