using UnityEngine;
using UnityEngine.UI;
using IsoTools;
using IsoTools.Physics;

public class InteractableDoor : MonoBehaviour
{
    public string requiredItem = "金鑰匙";
    private bool playerInRange = false;
    private bool use;
    private bool checking=true;
    private Inventory playerInventory;
    public GameObject interactUI; // 互動提示 UI
    public Text remider;
    public float displayTime = 2f; // 顯示時間（秒）
    public GameObject door;
    public IsoBoxCollider doorcoillder;
    private bool HideMessage=false;
   private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;

    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }

    void Start()
    {
        interactUI.SetActive(false);
    }
 
    public void SetStatus(bool newStatus)
    {
        use = newStatus;
    }

    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("Player"))
        {
            playerInventory = isoCollision.gameObject.GetComponent<Inventory>();
            playerInRange = true;
            HideMessage = true;
            
        }
    }

    void OnIsoCollisionExit(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            remider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!HideMessage) { remider.gameObject.SetActive(false); }
        else if(HideMessage){ remider.gameObject.SetActive(true); }


        if (playerInRange && use && playerInventory != null && playerInventory.HasItem(requiredItem))
        {
           
                playerInventory.RemoveItem(requiredItem);
                Debug.Log("門已解鎖！");
                remider.text = "已使用 " + requiredItem;
                HideMessage=true;
                Invoke("HideMessagee", displayTime); // 設定計時
                OpenDoor();
               
            }
            else if(playerInRange && use && checking)
            {              
                remider.text = "門鎖住了，需要 " + requiredItem;
                Debug.Log("門鎖住了，需要 " + requiredItem);
                HideMessage = true;
                Invoke("HideMessagee", displayTime); // 設定計時
            }
        }
    
    void HideMessagee()
    {
        HideMessage = false;
    }

    void OpenDoor()
    {

        SpriteRenderer doorSprite = door.GetComponentInChildren<SpriteRenderer>();
        doorSprite.enabled = false;
        doorcoillder.isTrigger = true;
        checking = false;

    }
}
