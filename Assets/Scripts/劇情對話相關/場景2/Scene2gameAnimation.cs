using IsoTools;
using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Scene2gameAnimation : MonoBehaviour
{
    // 震動
    public Transform doorspriteTransform;
    public float shakeAmount = 0.5f;
    public float shakeDuration = 0.5f;
    public int shakeCount = 2;


    public Transform[] fenceSpriteTransforms = new Transform[4];
    public GameObject fence;
    public float fenceshakeAmount = 0.5f;
    public float fenceshakeDuration = 0.5f;
    public int fenceshakeCount = 1;

    public GameObject player;
    public GameObject DOG;

    public SpriteRenderer door;
    public Sprite doorclose;
    public Sprite dooropened;
  
    public float doorOpenDuration = 0.5f;
    public float dogComeOutDuration = 0.5f;
    public float dogSTOPDuration = 3f;

    public float floatSpeed = 30f;
    public float fadeTime = 1.5f;

    public TextMeshProUGUI[] texts; // 將三個 TextMeshPro 拖進 Inspector
    public float floatDistance = 50f; // 飄動的距離（單位像素）
    public float duration = 0.6f;     // 每段文字持續的時間

    public PlayerController playerControl;
    private bool playerCanControl=true;
    public GameObject dialogeTrigger;

    private void Awake()
    {
      
    }

    void Update()
    {
        if (!playerCanControl)
        {
            InputManager.Instance.SetInputEnabled(false);
        }
       

    }


    //門震動
    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = doorspriteTransform.localPosition;

        for (int i = 0; i < shakeCount; i++)
        {
            doorspriteTransform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return new WaitForSeconds(shakeDuration);

            doorspriteTransform.localPosition = originalPos;
            yield return new WaitForSeconds(shakeDuration);
        }

        doorspriteTransform.localPosition = originalPos;
        DialogueManager.Instance.RegisterEventCallback();
    }


    //柵欄鎮董
    public void fenceShake()
    {
        foreach (Transform fence in fenceSpriteTransforms)
        {
            if (fence != null)
                StartCoroutine(shakeSingleFence(fence));
        }
    }

    private IEnumerator shakeSingleFence(Transform fence)
    {
        Vector3 originalPos = fence.localPosition;

        for (int i = 0; i < shakeCount; i++)
        {
            fence.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return new WaitForSeconds(shakeDuration);

            fence.localPosition = originalPos;
            yield return new WaitForSeconds(shakeDuration);
        }

        fence.localPosition = originalPos;
    }

    public void FadeOutFences()
    {
        foreach (Transform fence in fenceSpriteTransforms)
        {
            SpriteRenderer sr = fence.GetComponent<SpriteRenderer>();
            if (sr != null)
                StartCoroutine(FadeOutSpriteRenderer(sr, 1f)); // 1秒淡出
        }
    }

    private IEnumerator FadeOutSpriteRenderer(SpriteRenderer sr, float duration)
    {
        Color originalColor = sr.color;
        float timer = 0f;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
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



    //狗強制移動
    public void DOGStartAutoMoveByInput(Vector2 direction, float duration, float speed, float animSpeed)
    {
        StartCoroutine(DOGStartAutoMoveByInputCoroutine(direction, duration, speed, animSpeed));
    }


    public IEnumerator DOGStartAutoMoveByInputCoroutine(Vector2 direction, float duration, float speed, float animSpeed)
    {
        var parasiteController = DOG.GetComponent<dogEnemy>();
        if (parasiteController == null) yield break;

        parasiteController.isAutoMoving = true;
        parasiteController.speed = speed;
        parasiteController.UpdateMoveInput(direction);

        // 設定動畫播放速度
        if (parasiteController.animator != null)
            parasiteController.animator.speed = animSpeed;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        parasiteController.UpdateMoveInput(Vector2.zero);

        // 恢復動畫播放速度為正常（1f）
        if (parasiteController.animator != null)
            parasiteController.animator.speed = 1f;

        parasiteController.isAutoMoving = false;
    }


    //主角躲藏
    public void hide()
    {
        StartCoroutine(MoveLeftThenUpCoroutine());
    }

    private IEnumerator MoveLeftThenUpCoroutine()
    {

        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(-11.77f, -11.36f, 6f), 6f);
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(-9.77f, -11.36f, 6f), 6f);
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(-9.78f, -11.36f, 6f), 6f);


        // 移動完成後的邏輯（例如通知對話系統）
        DialogueManager.Instance.RegisterEventCallback();
        Dooropened();
        
    }

    public void watch()
    {
        StartCoroutine(WatchCoroutine());

    }
    private IEnumerator WatchCoroutine()
    {
        playerCanControl = false;
        yield return StartAutoMoveToTargetPositionWithAnimationCoroutine(new Vector3(-12f, -2.845f, 6f), 6f);
        DialogueManager.Instance.RegisterEventCallback();
       



    }

    //狗開門出現
    public void Dooropened()
    {
        StartCoroutine(DooropenedCoroutine());
       
    }
    private IEnumerator DooropenedCoroutine()
    {
        playerCanControl=false;
         yield return new WaitForSeconds(0.02f);
        door.sprite = dooropened;
        
       
        yield return new WaitForSeconds(dogComeOutDuration);
        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.left, 0.01f, 0.01f, 0.8f));
        DOG.SetActive(true);
        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.left, 0.2f, 1f, 0.8f));
        yield return new WaitForSeconds(dogComeOutDuration);
        door.sprite = doorclose;
        yield return new WaitForSeconds(1f);
        dogSearch();
    }


    public void dogSearch()
    {
        StartCoroutine(dogSearchCoroutine());
    }

    private IEnumerator dogSearchCoroutine()
    {
       
        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.down, 5f,1f, 0.2f));
       // yield return new WaitForSeconds(dogSTOPDuration);
        ShowFloatingTexts();
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.up,0.1f,5.7f, 0.8f));
        yield return new WaitForSeconds(1F);

        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.up, 0.5f,20F, 20f));//衝撞柵欄
        fenceShake();
        yield return new WaitForSeconds(1F);
        FadeOutFences();
        yield return new WaitForSeconds(1F);
        fence.SetActive(false);
        yield return new WaitForSeconds(1F);
        yield return StartCoroutine(DOGStartAutoMoveByInputCoroutine(Vector2.up, 1f, 8F, 0.8f));

        playerCanControl = true;

        dialogeTrigger.SetActive(true);




    }
    public void ShowFloatingTexts()
    {
        StartCoroutine(PlayFloatingSequence());
    }

    IEnumerator PlayFloatingSequence()
    {
        foreach (var text in texts)
        {
            text.gameObject.SetActive(true);
            Vector3 startPos = text.rectTransform.anchoredPosition;
            float timer = 0f;

            Color originalColor = text.color;
            text.color = originalColor; // 確保每次都是透明度為1開始

            while (timer < duration)
            {
                float t = timer / duration;

                // 漸漸往上飄
                text.rectTransform.anchoredPosition = startPos + Vector3.up * floatDistance * t;

                // 淡出透明
                Color c = text.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                text.color = c;

                timer += Time.deltaTime;
                yield return null;
            }

            text.gameObject.SetActive(false); // 隱藏
        }
    }
}



