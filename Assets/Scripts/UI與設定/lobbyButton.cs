using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class lobbyButton : MonoBehaviour
{
    public string levelName; // 關卡名稱

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadLevel);
    }

    // 載入指定關卡
    void LoadLevel()
    {
        Debug.Log("載入關卡：" + levelName);
        SceneManager.LoadScene(levelName);
    }

    // 結束遊戲
    public void QuitGame()
    {
        Application.Quit();
    }
}
