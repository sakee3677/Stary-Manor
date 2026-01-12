using UnityEngine;

public class Setting : MonoBehaviour
{
    public GameObject settingUI;

    void Start()
    {
        settingUI.SetActive(false); // 正確的啟用/停用方式
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingUI.SetActive(!settingUI.activeSelf); // 切換開關
        }
    }
}
