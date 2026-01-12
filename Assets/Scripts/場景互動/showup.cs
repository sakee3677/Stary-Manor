using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoTools;
using IsoTools.Physics;

public class showup : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public IsoBoxCollider isoBoxCollider;
    public void Canshow()
    {
        Debug.Log("canshow");
        spriteRenderer.enabled = true;
        isoBoxCollider.isTrigger = false;
    }
    public void Noshow()
    {
        spriteRenderer.enabled = false;
        isoBoxCollider.isTrigger = true;
    }
}
