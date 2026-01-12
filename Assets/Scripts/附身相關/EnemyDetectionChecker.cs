using UnityEngine;
using IsoTools.Physics;
using IsoTools;

public class EnemyDetectionChecker : MonoBehaviour
{
    public parasite dog; // 參考主敵人腳本
    public IsoObject detector;
    public IsoObject dogIso;
    public float x,y;
    public PlayerController playerController;
  

    public bool playerEnter;

    private IsoTriggerListener IsoTriggerListener;





    void Awake()
    {

        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
    void Update()
    {
        detector.position = new Vector3(dogIso.position.x-x, dogIso.position.y-y, dogIso.position.z);

        if (playerEnter)
        {
            Debug.Log("狗偵測到玩家");
            PlayerController player = playerController.GetComponent<PlayerController>();
            if (player != null)
            {
                // 玩家有輸入或跳躍時觸發攻擊
                if (player.moveInput != Vector2.zero)
                {
                    dog.Attack(playerController.isoObject.position);
                }
            }
        }

    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        if (isoCollider.CompareTag("Player"))
        {
            playerEnter = true;         
        }
    }
    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        playerEnter = false;
    }
}
