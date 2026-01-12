using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IsoTools.Physics;
using IsoTools;

public class boardDoor : MonoBehaviour
{

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;

    public AudioSource audioSource;

    public string nextSceneName;
    public string playerTag = "Player";
    public bool opened;
    public SpriteRenderer door;
    public Sprite Orgdoor;
    public Sprite dooropened;

    public bool board1;
    public bool board2;

    private bool wasOpened = false;
    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
    void Start()
    {

    }
    void Update()
    {
        if (board1 && board2)
        {
            door.sprite = dooropened;
            opened=true;
            if (!wasOpened)
            {
                audioSource.Play();
                wasOpened = true;
            }
        }
        else
        {
            door.sprite = Orgdoor;
            wasOpened = false; // 當條件不再成立，下次再成立才會播放
            opened = false;
        }
    }


    //if (opened == true)
    //{
    //    door.sprite = dooropened;
    //    audioSource.Play();
    //}
    //else if(opened == false)
    //{
    //    door.sprite = Orgdoor;
    //}

    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag(playerTag) && opened)
        {
            Debug.Log("Player teleported to: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);

        }
    }
    public void board1active()
    {
        board1 = true;
    }
    public void board2active()
    {
        board2 = true;
    }
    public void board1unactive()
    {
        board1 = false;
    }
    public void board2unactive()
    {
        board2 = false;
    }
 

}