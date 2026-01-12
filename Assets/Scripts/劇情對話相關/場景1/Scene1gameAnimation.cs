using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using static Unity.Collections.AllocatorManager;

public class Scene1gameAnimation : MonoBehaviour
{
    //畫出現
    public Light2D drawlight;
    public float startIntensity = 0f;
    public float endIntensity = 1f;
    public float fadeDuration = 1f;
    private bool drawApper;

    //畫講話
    public GameObject drawTalkinglight;
    public AudioClip[] voiceClips;  // 把3個音效丟進來
    public AudioSource audioSource;
    public float MintalkingLength;
    public float MaxxtalkingLength;
    public float MintalkingPitch;
    public float MaxxtalkingPitch;
    public Material DrawMaterial;
    public SpriteRenderer Draw;

    public AudioMixer mainMixer;
    public SpriteRenderer Black;
    public float fadeD = 2f;

    void Awake()
        {
            if (audioSource == null)
            {   
                audioSource = GetComponent<AudioSource>();
             
            }
        drawTalkinglight.SetActive(!true);
    }

    void Start()
    {
        blackfade();
        mainMixer.SetFloat("MusicVolume", -12);
        mainMixer.SetFloat("SFXVolume", -12);
        DrawMaterial = Draw.material;
    }
    public void blackfade()
    {
        StartCoroutine(FadeOutSprite());
    }

    private IEnumerator FadeOutSprite()
    {
        if (Black == null)
            yield break;

        Color originalColor = Black.color;
        float timer = 0f;

        while (timer < fadeD)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            Black.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 確保最後完全透明
        Black.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    //場景一
    //畫出現
    public void FadeInLight()
    {
        StartCoroutine(FadeLightCoroutine());
    }

    private IEnumerator FadeLightCoroutine()
    {
        float time = 0f;
        drawlight.intensity = startIntensity;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            drawlight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        // 確保最後強度正確
        drawlight.intensity = endIntensity;
        drawApper=true;
        DialogueManager.Instance.RegisterEventCallback();
    }


    //畫說話
    public void PlayRandomVoice()
    {
        if (voiceClips.Length == 0 || audioSource == null)
        {
            Debug.LogWarning("沒有音效或 AudioSource 為空！");
            DialogueManager.Instance.RegisterEventCallback(); // 確保對話不阻塞
            return;
        }

        AudioClip clip = voiceClips[Random.Range(0, voiceClips.Length)];
        float randomPitch = Random.Range(MintalkingPitch, MaxxtalkingPitch);
        float randomDuration = Random.Range(MintalkingLength, MaxxtalkingLength);

        audioSource.pitch = randomPitch;
        audioSource.clip = clip;
        audioSource.Play();
        if (drawTalkinglight != null && drawApper == true)
        {
            drawTalkinglight.SetActive(true);
            DrawMaterial.SetFloat("_ShakeUvSpeed", 4f);
        }
        else
        {
            Debug.Log("沒偵測到燈光&話還沒出現");
        }
     
        Debug.Log($"PlayRandomVoice ▶️ 說話長度: {randomDuration:F2} 秒, Pitch: {randomPitch:F2}");

        StartCoroutine(StopVoiceAfterDelay(randomDuration));
    }


    IEnumerator StopVoiceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
        drawTalkinglight.SetActive(!true);
        DrawMaterial.SetFloat("_ShakeUvSpeed", 1.5f);
        // ✅ 音效結束後再標記事件完成
        DialogueManager.Instance.RegisterEventCallback();
    }





}

