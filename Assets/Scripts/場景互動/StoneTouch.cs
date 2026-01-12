using IsoTools.Physics;
using UnityEngine;

public class StoneTouch : MonoBehaviour
{
    public StoneColorAssigner manager;

    private bool playerInRange = false;

    void OnIsoTriggerEnter(IsoCollider iso_collider)
    {
        if (iso_collider.CompareTag("Player"))
            playerInRange = true;
    }

    void OnIsoTriggerExit(IsoCollider iso_collider)
    {
        if (iso_collider.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton3)))
        {
            manager.TouchStone(gameObject);
        }
    }
}
