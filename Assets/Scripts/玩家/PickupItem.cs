using UnityEngine;
using IsoTools;
using IsoTools.Physics;
public class PickupItem : MonoBehaviour
{
    public string itemName;

    public GameObject goldkeyUI; // 互動提示 UI
    public GameObject door;

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
 
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {

        if (isoCollision.gameObject.CompareTag("Player"))
        {
            Inventory inventory = isoCollision.gameObject.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(itemName);
                goldkeyUI.SetActive(true);
                door.SetActive(true);
                Destroy(gameObject); // 拾取後消失
            }
        }


    }
}
