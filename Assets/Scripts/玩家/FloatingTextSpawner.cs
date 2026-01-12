using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    [Header("Floating Text Settings")]
    public bool useTypingEffect = true;
    public float fadeDuration = 1.5f;
    public float typingSpeed = 0.05f;
    public float floatSpeed = 0.5f;
    public Vector3 spawnOffset = new Vector3(0, 50f, 0);

    public void ShowFloatingText(string message)
    {
        GameObject obj = Instantiate(floatingTextPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        obj.transform.SetParent(transform); // 如果你要它跟著角色動

        FloatingText floatingText = obj.GetComponent<FloatingText>();
        floatingText.Play(
            message,
            spawnOffset,
            useTypingEffect,
            fadeDuration,
            typingSpeed,
            floatSpeed
        );
    }
}
