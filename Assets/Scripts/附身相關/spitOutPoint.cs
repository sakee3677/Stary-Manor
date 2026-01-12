using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class spitOutPoint : MonoBehaviour
{
    public Slimee Slimeee;

    public float TriggerNum=0;

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    // Start is called before the first frame update

    private void Awake()
    {
  

        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }

        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerNum >= 1)
        {
            Slimeee.CanSpit = false;
        }
        else
        {
            Slimeee.CanSpit = true;
        }
    }

    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {

        TriggerNum++;

        Debug.Log("spittrigger");
        


    }

    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        TriggerNum--;


        Debug.Log("spittriggerExit");

    }
}
