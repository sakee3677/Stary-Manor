using IsoTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbyAnimation : MonoBehaviour
{
    public List<GameObject> thunderlight;
    public GameObject player;

    public Camera cam;
    public bool isFollowing = false;
    public Transform target; // 主角
    public Vector3 followOffset = new Vector3(0, 0, -10);
    // Start is called before the first frame update
    void Start()
    {
        playerStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //玩家強制移動
    public void StartAutoMoveToTargetPositionWithAnimation(Vector3 targetPosition, float speed)
    {
        StartCoroutine(StartAutoMoveToTargetPositionWithAnimationCoroutine(targetPosition, speed));
    }

    public IEnumerator StartAutoMoveToTargetPositionWithAnimationCoroutine(Vector3 targetPosition, float speed)
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController == null) yield break;

        IsoObject isoObject = player.GetComponent<IsoObject>();
        Animator animator = player.GetComponentInChildren<Animator>();

        if (isoObject == null || animator == null)
        {
            Debug.LogError("缺少 IsoObject 或 Animator 組件");
            yield break;
        }


        playerController.speed = speed;

        Vector3 direction = (targetPosition - isoObject.position).normalized;
        playerController.OnMove(new Vector2(direction.x, direction.y));

        float distanceThreshold = 0.01f;

        while (Vector3.Distance(isoObject.position, targetPosition) > distanceThreshold)
        {
            isoObject.position = Vector3.MoveTowards(isoObject.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        isoObject.position = targetPosition; // 強制設為目標點
        playerController.OnMove(Vector2.zero);
    }

    public void playerStart()
    {
        StartCoroutine(playerstart());
       
    }
    private IEnumerator playerstart()
    {
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(8.23f, -3.77f, 1.3f), 4.5f);
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(8.23f, -3.77f, 1.3f), 6f);
    }

    void LateUpdate()
    {
        if (isFollowing && target != null)
        {
            cam.transform.position = target.position + followOffset;
        }

    

    }
    public void CamStartFollow()
    {


        StartCoroutine(CamStartFollowCoroutine());
        DialogueManager.Instance.RegisterEventCallback();
    }
    private IEnumerator CamStartFollowCoroutine()
    {
        yield return new WaitForSeconds(1f);
       
        isFollowing = true;

    }
    public void EnableTHUNDERLIGHT()
    {
        foreach (GameObject obj in thunderlight)
        {
            obj.SetActive(true);
        }
    }

    public void DisableTHUNDERLIGHT()
    {
        foreach (GameObject obj in thunderlight)
        {
            obj.SetActive(false);
        }
    }
}
