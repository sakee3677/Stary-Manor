using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneResetter : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ReloadCurrentLevel);
    }

    public void ReloadCurrentLevel()
    {
        string currentLevelName = SceneManager.GetActiveScene().name;
        Debug.Log("重新載入關卡：" + currentLevelName);
        SceneManager.LoadScene(currentLevelName);
    }
}
