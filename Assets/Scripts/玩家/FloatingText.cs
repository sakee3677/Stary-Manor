using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float fadeDuration = 1.5f;
    public float typingSpeed = 0.05f;

    private TextMeshProUGUI tmp;
    private Color originalColor;
    private float elapsed;
    private bool isPlaying;
    public Vector3 additionalOffset = Vector3.zero;
    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Play(
      string message,
      Vector3 offset,
      bool useTypingEffect,
      float fadeDuration,
      float typingSpeed,
      float floatSpeed
  )
    {
        transform.position += offset + additionalOffset;
        tmp.text = "";
        originalColor = tmp.color;
        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        elapsed = 0f;
        isPlaying = true;

        // ? 接收外部傳來的參數
        this.fadeDuration = fadeDuration;
        this.typingSpeed = typingSpeed;
        this.floatSpeed = floatSpeed;

        if (useTypingEffect)
            StartCoroutine(TypeText(message));
        else
            tmp.text = message;
    }


    IEnumerator TypeText(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            tmp.text += message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        elapsed += Time.deltaTime;

        float fadeStartTime = fadeDuration * 0.5f;
        float alpha = 1f;
        if (elapsed >= fadeStartTime)
        {
            float fadeElapsed = elapsed - fadeStartTime;
            alpha = Mathf.Lerp(1f, 0f, fadeElapsed / (fadeDuration - fadeStartTime));
        }

        tmp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (elapsed >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
