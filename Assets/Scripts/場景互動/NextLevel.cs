using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IsoTools.Physics;
using IsoTools;

public class NextLevel : MonoBehaviour
{

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;

    public string nextSceneName;
    public string playerTag = "Player";
    public bool opened;
    public SpriteRenderer door;
    public Sprite dooropened;
    public Sprite doors;


    public AudioSource audioSource;
    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
    void Start()
    {

    }
   
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag(playerTag) && opened)
        {
            Debug.Log("Player teleported to: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);

        }
    }
   
    public void openDoor()
    {
        audioSource.Play();
        StartCoroutine(Dooropen());
    }
    public void closeDoor()
    {

        opened = false;
        door.sprite = doors;
    }
    IEnumerator Dooropen()
    {
        opened = true;
        door.sprite = dooropened;
       
        DialogueManager.Instance.RegisterEventCallback();
        yield return null;
    }
   
}

