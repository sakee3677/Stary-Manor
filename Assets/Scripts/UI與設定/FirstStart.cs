using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[System.Serializable]
public class FadeTarget
{
    public GameObject obj;
    public SpriteRenderer spriteRenderer;
    public Image uiImage;
    public Button uiButton;
    public Text uiText;
    public TextMeshProUGUI tmpText;
    public Color originalColor;
}
[System.Serializable]
public class DialogueEvent
{
    public string lineText;                     // 顯示的文字
    public AudioClip voiceClip;                // 話語音效
    public UnityEngine.Events.UnityEvent onLineStart; // 每句話開始前要觸發的事件
}


public class FirstStart : MonoBehaviour
{
    [Header("UI roots for fade out")]
    public List<GameObject> parentRoots;

    [Header("Floating text顯示用")]
    public GameObject floatingTextPrefab;
    public Transform floatingTextSpawnPoint;

    [Header("對話資料")]
    public List<DialogueEvent> dialogueSequence = new List<DialogueEvent>();

    public List<AudioClip> dialogueClips; // 和上面 index 對應
    public float delayBetweenLines = 1.5f;

    public float fadeDuration = 2f;

    private float timer = 0f;
    private bool isFading = false;

   private float atkLerpTimer ;
    public float Duration = 1f;
    public Material lobbyMaterial;
    public SpriteRenderer lobbySprite;

    public AudioMixer mainMixer;
 
   
    public Animator animationImageANIM;

    private List<FadeTarget> fadeTargets = new List<FadeTarget>();

    void Awake()
    {
       
        foreach (GameObject root in parentRoots)
        {
            Component[] allComponents = root.GetComponentsInChildren<Component>(true);

            foreach (Component comp in allComponents)
            {
                GameObject obj = comp.gameObject;

                // 已處理過就略過
                if (fadeTargets.Exists(t => t.obj == obj)) continue;

                FadeTarget target = new FadeTarget();
                target.obj = obj;

                target.spriteRenderer = obj.GetComponent<SpriteRenderer>();
                target.uiImage = obj.GetComponent<Image>();
                target.uiButton = obj.GetComponent<Button>();
                target.uiText = obj.GetComponent<Text>();
                target.tmpText = obj.GetComponent<TextMeshProUGUI>();

                // 優先順序處理原始顏色
                if (target.spriteRenderer != null)
                    target.originalColor = target.spriteRenderer.color;
                else if (target.uiImage != null)
                    target.originalColor = target.uiImage.color;
                else if (target.uiButton != null && target.uiButton.image != null)
                    target.originalColor = target.uiButton.image.color;
                else if (target.uiText != null)
                    target.originalColor = target.uiText.color;
                else if (target.tmpText != null)
                    target.originalColor = target.tmpText.color;
                else
                    continue; // 不支援的就略過

                fadeTargets.Add(target);
            }
        }
    }

    private void Start()
    {
        lobbyMaterial.SetFloat("_ChromAberrAmount", 0);
        lobbyMaterial.SetFloat("_TwistUvAmount", 0);
        lobbyMaterial.SetFloat("_Contrast", 1);
        lobbyMaterial.SetFloat("_Brightness", 0);
    }


    public void StartFade()
    {
        timer = 0f;
        isFading = true;
    }

    void Update()
    {
        if (!isFading) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fadeDuration);

        foreach (var target in fadeTargets)
        {
            float alpha = Mathf.Lerp(target.originalColor.a, 0f, t);
            Color newColor = new Color(
                target.originalColor.r,
                target.originalColor.g,
                target.originalColor.b,
                alpha);

            if (target.spriteRenderer != null)
                target.spriteRenderer.color = newColor;
            if (target.uiImage != null)
                target.uiImage.color = newColor;
            if (target.uiButton != null && target.uiButton.image != null)
                target.uiButton.image.color = newColor;
            if (target.uiText != null)
                target.uiText.color = newColor;
            if (target.tmpText != null)
                target.tmpText.color = newColor;

            if (t >= 1f)
                target.obj.SetActive(false);
        }

        if (t >= 1f)
        {
            isFading = false;
            StartCoroutine(ShowDialogueLines());

        }
    }
    IEnumerator ShowDialogueLines()
    {
        yield return new WaitForSeconds(1f); // 等待淡出完成

        GameObject previousText = null;

        foreach (var dialogue in dialogueSequence)
        {
            // 移除上一句
            if (previousText != null)
                Destroy(previousText);

            // 執行事件
            dialogue.onLineStart?.Invoke();

            // 建立新文字
            GameObject obj = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity);
            previousText = obj;

            MenuFloatingText ft = obj.GetComponent<MenuFloatingText>();
            if (ft != null)
            {
                ft.voiceClip = dialogue.voiceClip;
                ft.StartTyping(dialogue.lineText);
            }

            yield return new WaitForSeconds(dialogue.lineText.Length * ft.typingSpeed + delayBetweenLines);
        }

        // 結尾移除最後一句
        if (previousText != null)
            Destroy(previousText);
    }
    public void firstAttack()
    {
        StartCoroutine(DoFirstAttackEffect());
    }

    private IEnumerator DoFirstAttackEffect()
    {
        float timer = 0f;
        float peakGlow = 0.1f;
        float peakParaGlow = 1f;
        float holdDuration = 0.2f; // 頂峰停留時間

        // 上升段：從 0 到 頂峰
        while (timer < Duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / Duration);
            float glowValue = Mathf.Lerp(0f, peakGlow, t);
            float paraglowValue = Mathf.Lerp(0f, peakParaGlow, t);

            lobbyMaterial.SetFloat("_ChromAberrAmount", glowValue);
          ///  lobbyMaterial.SetFloat("_Glow", paraglowValue);

            
            
            yield return null;
        }

        // 頂峰維持一段時間
        yield return new WaitForSeconds(holdDuration);
        animationImageANIM.SetBool("animChange", true);
        // 下降段：從 頂峰 回到 0
        timer = 0f;
        while (timer < Duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / Duration);
            float glowValue = Mathf.Lerp(peakGlow, 0f, t);
            float paraglowValue = Mathf.Lerp(peakParaGlow, 0f, t);

            lobbyMaterial.SetFloat("_ChromAberrAmount", glowValue);
          //  lobbyMaterial.SetFloat("_Glow", paraglowValue);

            yield return null;
        }
    }
    public void SeccondAttack()
    {
        StartCoroutine(DoSeccondAttackAttackEffect());
    }

    private IEnumerator DoSeccondAttackAttackEffect()
    {
        float timer = 0f;
        float TwistGlow = 0.15f;
        float ContrastGlow = 0f;
        float BrightnessGlow = -1f;
        float brightnessDurtion = 3f;

        // 上升段：從 0 到 頂峰
        while (timer < brightnessDurtion)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / brightnessDurtion);
            float glowValue = Mathf.Lerp(0f, TwistGlow, t);
            float ContrastValue = Mathf.Lerp(1f, ContrastGlow, t);
            float BrightnessValue = Mathf.Lerp(0f, BrightnessGlow, t);

            lobbyMaterial.SetFloat("_TwistUvAmount", glowValue);
            lobbyMaterial.SetFloat("_Contrast", ContrastValue);
            lobbyMaterial.SetFloat("_Brightness", BrightnessValue);         
            yield return null;
        }
    }
    public void musicLOWER()
    {
       mainMixer.SetFloat("MusicVolume", -40);
        mainMixer.SetFloat("SFXVolume", 1);
    }
 
    public void NextScene()
    {
        SceneManager.LoadScene("level_1_1");
    }


}


