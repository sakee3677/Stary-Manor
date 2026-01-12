using IsoTools.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSwitchScenes : MonoBehaviour
{
    private IsoCollisionListener collisionListener;
    public string sceneName; // 要切換的場景名稱

    void Awake()
    {
        // 確保有 IsoCollisionListener
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
    }

    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        // 當有物件進入傳送門時切換場景
        if (isoCollider.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    void OnIsoTriggerExit(IsoCollision isoCollision)
    {

    }
}
