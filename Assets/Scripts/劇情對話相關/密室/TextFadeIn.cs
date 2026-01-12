using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI text;  // 如果是 UI 文字
    public float fadeDuration = 2f;

    private void Start()
    {
       
    }
    public void FADEIN()
    {
        StartCoroutine(FadeInText());
      
    }
    private IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(2f);
        if (text == null) yield break;

        // 確保從完全透明開始
        Color originalColor = text.color;
        originalColor.a = 0f;
        text.color = originalColor;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 確保最終完全不透明
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        yield return new WaitForSeconds(3f);
        LoadSceneByName("menu");
    }
    public void LoadSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("場景名稱不能為空！");
        }
    }
}
