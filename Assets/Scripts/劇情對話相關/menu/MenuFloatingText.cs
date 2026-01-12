using System.Collections;
using TMPro;
using UnityEngine;

public class MenuFloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float typingSpeed = 0.5f;
    public AudioClip voiceClip;

    private AudioSource audioSource;

    void Awake()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        audioSource = GetComponent<AudioSource>();
    }

    public void StartTyping(string message)
    {
        StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        textMeshPro.text = "";
        if (voiceClip != null)
        {
            audioSource.clip = voiceClip;
            audioSource.Play();
        }

        foreach (char c in message)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 停止語音（如果還在播）
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
