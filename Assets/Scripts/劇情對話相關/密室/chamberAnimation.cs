using IsoTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.Collections.AllocatorManager;

public class chamberAnimation : MonoBehaviour
{
    public GameObject player;
    public GameObject animasprite;


    public SpriteRenderer Black;
    public float fadeD = 2f;
    public float fadeDuration = 1f;
    public SpriteRenderer ANIMA;

   
    // Start is called before the first frame update
    void Start()
    {
        playerStart();
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
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(-35f, -9.3f, 1.3f), 4.5f);
        yield return FadeOutSprite();
        yield return PLAYERDisablE();
        yield return AnimafadE();


    }

    public void BlackFade()
    {
        StartCoroutine(FadeOutSprite());
    }

    private IEnumerator FadeOutSprite()
    {
        if (Black == null)
            yield break;

        // 確保使用正確範圍的顏色
        Color originalColor = Black.color;

        float timer = 0f;

        while (timer < fadeD)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeD);
            Black.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 確保最終完全不透明
        Black.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }



    public void PLAYERDisable()
    {
        StartCoroutine(PLAYERDisablE());
    }

    private IEnumerator PLAYERDisablE()
    {
       player.SetActive(false);
        yield return null;
    }

    public void Animafade()
    {
        StartCoroutine(AnimafadE());
    }

    private IEnumerator AnimafadE()
    {
        if (ANIMA == null)
            yield break;
        animasprite.SetActive(true);
        Color originalColor = ANIMA.color;
        float timer = 0f;

        while (timer < fadeD)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            ANIMA.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }


        ANIMA.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
       
    }
   
}
