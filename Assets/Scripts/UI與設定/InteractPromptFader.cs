using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class InteractPromptFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f; // 淡入淡出的時間

    [Header("位置偏移量 (相對於目標)")]
    public Vector3 positionOffset = new Vector3(0, 2f, 0); // 預設往上顯示

    public Image buttonImage; // 控制按鈕顏色的 Image

    public float pressScale = 0.95f; // 按下時縮小的比例
    public float animationDuration = 0.5f; // 變亮變暗的動畫時間

    private Transform followTarget;
    private Coroutine fadeCoroutine;
    private Coroutine blinkCoroutine;

    private Vector3 originalScale;
    private Color originalColor;

    private void Start()
    {
        originalScale = buttonImage.transform.localScale;
        originalColor = buttonImage.color;
    }

    private void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + positionOffset;
        }
    }

    /// <summary>
    /// 顯示圖示（淡入）
    /// </summary>
    public void Show(Transform target)
    {
        followTarget = target;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        canvasGroup.alpha = 0f;  // 顯示時確保 alpha 是從 0 開始
        fadeCoroutine = StartCoroutine(FadeCanvas(1f));

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    /// <summary>
    /// 隱藏圖示（淡出）
    /// </summary>
    public void Hide()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        fadeCoroutine = StartCoroutine(FadeCanvas(0f));
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator BlinkEffect()
    {
        while (canvasGroup.alpha > 0) // 只要圖示還是可見
        {
            // 按下效果
            yield return StartCoroutine(ScaleAndFade(buttonImage, pressScale, 0.7f));
            // 彈起效果
            yield return StartCoroutine(ScaleAndFade(buttonImage, 1f, 1f));
        }
    }

    private IEnumerator ScaleAndFade(Image image, float targetScale, float targetAlpha)
    {
        float time = 0f;
        Vector3 startScale = image.transform.localScale;
        Color startColor = image.color;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            // 平滑過渡縮放
            image.transform.localScale = Vector3.Lerp(startScale, originalScale * targetScale, t);
            // 平滑過渡顏色（透明度）
            image.color = Color.Lerp(startColor, originalColor * targetAlpha, t);

            yield return null;
        }

        // 確保最終值設定正確
        image.transform.localScale = originalScale * targetScale;
        image.color = originalColor * targetAlpha;
    }
}
