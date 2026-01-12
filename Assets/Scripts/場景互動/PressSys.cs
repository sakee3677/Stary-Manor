using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoTools;
using IsoTools.Physics;
using UnityEngine.Events;

public class PressSys : MonoBehaviour
{
    public float Request;
    private float currentWeight = 0;
    private HashSet<IsoCollider> objectsOnPlate = new HashSet<IsoCollider>();
    private IsoCollisionListener collisionListener;
    public UnityEvent unityEvent;
    public UnityEvent onRelease;
    private bool isActivated = false;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public Material BoardMaterial;
    public SpriteRenderer Board;

    void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>();
        if (collisionListener == null)
        {
            collisionListener = gameObject.AddComponent<IsoCollisionListener>();
        }
    }

    void Start()
    {
        BoardMaterial = Board.material;
    }

    void LateUpdate()
    {
        // **重新計算當前重量**
        float totalWeight = 0;
        foreach (var iso in objectsOnPlate)
        {
            if (iso != null && iso.enabled && iso.gameObject.activeInHierarchy)
            {
                totalWeight += GetObjectWeight(iso);
            }
        }

        // **檢查是否需要切換狀態**
        if (totalWeight >= Request && !isActivated)
        {
            ActivatePlate();
        }
        else if (totalWeight < Request && isActivated)
        {
            DeactivatePlate();
        }

        // **更新目前重量**
        currentWeight = totalWeight;
    }

    void OnIsoTriggerEnter(IsoCollider iso_collider)
    {
        if (!objectsOnPlate.Contains(iso_collider) &&
            (iso_collider.CompareTag("Absorbable") || iso_collider.CompareTag("PlayerParasite") || iso_collider.CompareTag("ParasiteHost")))
        {
            objectsOnPlate.Add(iso_collider);
        }
    }

    void OnIsoTriggerExit(IsoCollider iso_collider)
    {
        if (objectsOnPlate.Contains(iso_collider))
        {
            objectsOnPlate.Remove(iso_collider);
        }
    }

    void ActivatePlate()
    {
        isActivated = true;
        BoardMaterial.SetFloat("_HsvBright", 0.4f);
        BoardMaterial.SetFloat("_OffsetUvY", 0.07f);
        unityEvent.Invoke();
        audioSource.Play();
        Debug.Log("壓力板啟動");
    }

    void DeactivatePlate()
    {
        isActivated = false;
        BoardMaterial.SetFloat("_HsvBright", 1f);
        BoardMaterial.SetFloat("_OffsetUvY", 0f);
        onRelease.Invoke();
        Debug.Log("壓力板關閉");
    }

    float GetObjectWeight(IsoCollider iso_collider)
    {
        IsoRigidbody rb = iso_collider.GetComponent<IsoRigidbody>();
        return rb != null ? rb.mass : 0;
    }
}
