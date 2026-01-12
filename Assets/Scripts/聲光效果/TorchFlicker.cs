using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    public Light2D torchLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= flickerSpeed)
        {
            timer = 0f;
            torchLight.intensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
