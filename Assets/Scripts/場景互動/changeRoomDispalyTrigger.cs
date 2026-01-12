using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IsoTools.Physics;
using IsoTools;
using Unity.VisualScripting;
public class changeRoomDispalyTrigger : MonoBehaviour
{
    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;


  
    public GameObject prisonCell;
    public GameObject otherScenceBlock;
    public IsoObject ParasiteHost;

    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
        
    }
    void Start()
    {
        SpriteRenderer ParasiteHostSprite = ParasiteHost.GetComponentInChildren<SpriteRenderer>();
        ParasiteHostSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        SpriteRenderer ParasiteHostSprite = ParasiteHost.GetComponentInChildren<SpriteRenderer>();
        if (isoCollider.CompareTag("Player") || isoCollider.CompareTag("PlayerParasite"))
        {
            otherScenceBlock.SetActive(false);
            prisonCell.SetActive(true);
            ParasiteHostSprite.enabled = true;
            Debug.Log("進入牢房");

        }

    }

    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        
        if (isoCollider.CompareTag("Player") || isoCollider.CompareTag("PlayerParasite"))
        {
            otherScenceBlock.SetActive(true);
            prisonCell.SetActive(false);
            Debug.Log("離開牢房");
        }
    }
}
