using IsoTools;
using IsoTools.Physics;
using IsoTools.Physics.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{

    public Vector3 positionOffset = new Vector3(0f, 0f, 0f);
    public PlayerController playerController;
    public GameObject Groundchecker;
    public float groundNum=0;

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IsoObject IsoplayerController = playerController.GetComponent<IsoObject>();
        IsoObject IsoGroundchecker= Groundchecker.GetComponent<IsoObject>();
        IsoGroundchecker.position = IsoplayerController.position+ positionOffset;
        if (groundNum >= 1)
        {
            playerController.isGrounded = true;
        }
        else
        {
            playerController.isGrounded = false;
        }
    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        
        if (isoCollider.CompareTag("Ground"))
        {
            groundNum++;
            //playerController.isGrounded = true;
            Debug.Log("地板數" + groundNum);
        }
    }
    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        
        if (isoCollider.CompareTag("Ground"))
        {
            groundNum--;
           // playerController.isGrounded = false;
            Debug.Log("地板數"+ groundNum);
        }
    }
    
}
