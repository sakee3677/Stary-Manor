using UnityEngine;

public class StoneLightController : MonoBehaviour
{
    [Header("對應每個顏色的 Light（順序要和 applyColorOrder 對應）")]
    public GameObject[] colorLights;

    public void ShowColorLight(int index)
    {
        for (int i = 0; i < colorLights.Length; i++)
        {
            if (colorLights[i] != null)
                colorLights[i].SetActive(i == index); // 只打開對應顏色光
        }
    }

    public void ResetLights()
    {
        foreach (var light in colorLights)
        {
            if (light != null)
                light.SetActive(false);
        }
    }
}
