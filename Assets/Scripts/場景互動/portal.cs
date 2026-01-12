using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{
    public PlayerController playerController;
    private IsoCollisionListener collisionListener;
    void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
    }
        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        //playerController.CanUsedoor = true;
        Debug.Log("可開門");
    }
    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        //playerController.CanUsedoor = false;
        Debug.Log("不可開門");
    }

}
