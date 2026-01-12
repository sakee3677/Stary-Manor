using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoTools;
using IsoTools.Physics;

public class presscheaker : MonoBehaviour
{
    public IsoBoxCollider isoBoxCollider;
    public GameObject cangoUI;
    public GameObject portal;

    void Start()
    {
        IsoBoxCollider isoBoxCollider = GetComponent<IsoBoxCollider>();
        isoBoxCollider.isTrigger = true;
        portal.SetActive(false);
    }

    public void CanGo()
    {
        portal.SetActive(true);

        isoBoxCollider.isTrigger = false;
        cangoUI.SetActive(true);
    }
    public void NoGo()
    {
        portal.SetActive(false);
        isoBoxCollider.isTrigger = true;
        cangoUI.SetActive(false);
    }
}
