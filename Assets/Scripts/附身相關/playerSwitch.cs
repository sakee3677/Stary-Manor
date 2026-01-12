using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static InputManager;

public class playerSwitch : MonoBehaviour
{
    public PlayerController playerController; // 場景中的預設玩家控制器
    private IControllable currentHost;        // 當前寄生的物體

    void Start()
    {
        // 自動設為起始控制器
        if (playerController != null)
        {
            SwitchPlayer(playerController);
        }
        else
        {
            Debug.LogWarning("playerSwitch 找不到預設的 PlayerController。");
        }

        // 每次場景加載後自動重新綁定控制器
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 嘗試找新的 PlayerController（避免 null）
        PlayerController newPlayer = FindObjectOfType<PlayerController>();
        if (newPlayer != null)
        {
            playerController = newPlayer;
            SwitchPlayer(newPlayer);
        }
        else
        {
            Debug.LogWarning("在新場景中找不到 PlayerController。");
        }
    }

    public void SwitchPlayer(IControllable newHost)
    {
        if (currentHost != null)
            currentHost.DeactivateHost();

        currentHost = newHost;

        if (currentHost != null)
        {
            // 傳入目前的輸入值
            Vector2 currentInput = InputManager.Instance.GetCurrentMoveInput();
            newHost.OnMove(currentInput);
            currentHost.ActivateHost();
            InputManager.Instance.SetCurrentController(currentHost);
        }
    }
}
