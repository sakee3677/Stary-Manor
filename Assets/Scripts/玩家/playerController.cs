using IsoTools.Physics;
using IsoTools;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using static parasite;
using TMPro;
using static InputManager;
using Unity.Burst.CompilerServices;





public class PlayerController : MonoBehaviour, IControllable
{
    public IsoBoxCollider playercoillder;
    private PlayerControl controls;
    public IsoObject isoObject;          // 当前 IsoObject
    public IsoObject parasiteIso;
    public Material PlayerMaterial;
    public SpriteRenderer PlayerSprite;
    private Material parasiteMaterial;
    private SpriteRenderer parasiteSprite;
    public InteractPromptFader interactionHintPrefab;
    private InteractPromptFader hintInstance;
    private IUsable currentItem; // 當前選中的物品

    [Header("Movement Setting")]
    public float jumpVelocity = 0.5f;
    public float speed = 0.5f;
    public float UnGroundSpeed = 0.5f;
    public bool isGrounded;
    public Vector2 moveInput;
    public Vector2 lastDirection = Vector2.zero;
    public float parasiteExitForce = 5f;
    private bool isJumpHeld = false;
    private Vector2 lastInput = Vector2.zero;
    public bool CanUse=false; 
    [Header("Parasite Setting")]
    public float GoingParasiteSpeed;
    public float glowDuration = 1f; // 幾秒內完成從低到高
    private float glowLerpTimer = 0f;
    private bool isGlowing = false;
    private bool secondFadeStarted = false;
    public float secondFadeDuration = 1f; // 第二階段多久內降到0
 
    [SerializeField] private float CanParasiteGlowDuration = 1.5f; // 亮暗變化所需時間
    [SerializeField] private float CanParasiteDrawnDuration = 1.5f;
    private float drawnMin = 0f;
    private float drawnMax = 3f;
    private float glowMin = 1f;
    private float glowMax = 20f;
    private float currentGlow = 1f; // 初始亮度
    private float currentDrawn = 0f;

    public float exitGlowDuration = 1f; // 光暈恢復時間
    private float exitGlowTimer;
    private bool isExitingParasite = false;
    public bool animationParasite;

    public playerSwitch playerSwitch;
    public GameObject playerSprite;
    private GameObject hostObject;
    public string parasiteTag = "ParasiteHost";
    private bool isParasiting = false;
    public bool targetDetected = false;
    private Vector2 parasiteOffset;
    public Vector3 positionOffset = new Vector3(0f, 0f, 1f); // 自定義的偏移量，可以根據需要調整

    [Header("Shadow Setting")]
    public GameObject shadow;


    public enum parasitingPhase
    {
        None,
        prepare,
        parasiting
    }
    private parasitingPhase ParasitingPhase = parasitingPhase.None;



    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;



    public Animator animator; // 用於控制子項目的Animator


    //使用物品

    //public bool CanUsedoor = false;


    void Start()
    {

        isGrounded = true;
       
    }


    void Awake()
    {

        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();

        animator = GetComponentInChildren<Animator>();
        isoObject = GetComponent<IsoObject>();

    }




    public void ActivateHost()
    {

        this.gameObject.GetComponent<GroundProjection>().EnableProjection();

        Debug.Log("玩家控制啟用");
    }

    public void DeactivateHost()
    {
       

        isJumpHeld = false;
        isGrounded = false;
        Debug.Log("玩家控制停用");
    }


    public void Update()
    {
        UpdateAnimation(moveInput);
        var isoRigidbody = GetComponent<IsoRigidbody>();
        // 當玩家開始下落且跳躍鍵持續被按住時，才開始降低重力
        if (isoRigidbody.velocity.z <= 0 && isJumpHeld && !isGrounded)
        {
            IsoWorld.GetWorld(0).gravity = new Vector3(0, 0, -1f);
            //Debug.Log("重力降低");
        }
        else
        {
            IsoWorld.GetWorld(0).gravity = new Vector3(0, 0, -9f);
            // Debug.Log("重力恢復");
        }
        if (targetDetected)
        {
            this.gameObject.GetComponent<GroundProjection>().SetDotColor();
            //Debug.Log("玩家可以跳躍");
        }
        else
        {
            this.gameObject.GetComponent<GroundProjection>().UNSetDotColor();
            //Debug.Log("玩家在空中，無法跳躍");
        }
        //CheckGrounded();

        // 確保只有在沒有寄生時才能移動
        if (!isParasiting)
        {
            Move();
        }
        else if (isParasiting)
        {
            HandleParasting();

        }

        if (isExitingParasite)
        {
            exitGlowTimer += Time.deltaTime;
            float t = Mathf.Clamp01(exitGlowTimer / exitGlowDuration);

            // 亮度從10降到1
            float glowValue = Mathf.Lerp(10f, 1f, t);

            if (PlayerMaterial != null)
                PlayerMaterial.SetFloat("_GlowGlobal", glowValue);
            if (parasiteMaterial != null)
                parasiteMaterial.SetFloat("_GlowGlobal", glowValue);

            if (t >= 1f)
            {
                // 完成亮度恢復
                isExitingParasite = false;
                Debug.Log("亮度恢復完成！");
            }
        }

        float targetGlow = targetDetected ? glowMax : glowMin;
        float targetDrawn = targetDetected ? drawnMax : drawnMin;

        // 緩慢靠近目標值
        currentGlow = Mathf.MoveTowards(currentGlow, targetGlow, (glowMax - glowMin) / CanParasiteGlowDuration * Time.deltaTime);
        currentDrawn = Mathf.MoveTowards(currentDrawn, targetDrawn, (drawnMax - drawnMin) / CanParasiteDrawnDuration * Time.deltaTime);

        if (PlayerMaterial != null)
            PlayerMaterial.SetFloat("_AlphaOutlineGlow", currentGlow);
        PlayerMaterial.SetFloat("_HandDrawnAmount", currentDrawn);



    }
    public void OnMove(Vector2 input)
    {
        // 如果沒有輸入，清空 moveInput
        if (input == Vector2.zero)
        {
            moveInput = Vector2.zero;
            return;
        }

        // 計算輸入向量的角度 (0 度為右，逆時針計算)
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 設定4個對角方向的範圍
        if (angle >= 0 && angle < 90) moveInput = new Vector2(1, 0);     // 右上
        else if (angle >= 90 && angle < 180) moveInput = new Vector2(0, 1); // 左上
        else if (angle >= 180 && angle < 270) moveInput = new Vector2(-1, 0); // 左下
        else moveInput = new Vector2(0, -1); // 右下

        // 記錄最後方向
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        lastInput = input;
    }




    public void Move()
    {
        var isoRigidbody = GetComponent<IsoRigidbody>();
        Vector3 inputDirection = new Vector3(moveInput.x, moveInput.y);

  

        

        //Debug.Log($"Current Status: {animator.GetInteger("Status")}");
        // 設定速度，保留垂直方向速度
        if (isGrounded)
            {
                Vector3 velocity = new Vector3(inputDirection.x * speed, inputDirection.y * speed, isoRigidbody.velocity.z);
                isoRigidbody.velocity = velocity;
            }
           else if (!isGrounded)
            {
                Vector3 velocity = new Vector3(inputDirection.x * UnGroundSpeed, inputDirection.y * UnGroundSpeed, isoRigidbody.velocity.z);
                isoRigidbody.velocity = velocity;
            }
        }

    public void UpdateAnimation(Vector2 direction)
    {
        bool isMoving = direction != Vector2.zero;

        // 設定 Animator 參數
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("Parasting", animationParasite);

        if (isMoving)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
        else
        {
            // 停止時保留最後方向
            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
        }
    }

   

public void OnJump()
    {
        Debug.Log("嘗試跳躍");
        isJumpHeld = true;
        var isoRigidbody = GetComponent<IsoRigidbody>();
        if (isGrounded)
        {
            if (isoRigidbody != null)
            {
                // 添加跳躍力
                isoRigidbody.AddForce(new Vector3(0, 0, jumpVelocity), ForceMode.Impulse);

            }       
            
        }
        if (!isGrounded)
        {
            Debug.Log("已達最大跳躍次數");
        }
    }

    public void OnJumpCanceled()
    {
        isJumpHeld = false;
        // 停止跳躍的減重邏輯
    }


    public void OnParasite()
    {
        if (!isParasiting && hostObject != null && targetDetected)
        {
            isParasiting = true;
            ParasitingPhase = parasitingPhase.prepare;
        }

    }
    //private void Parasiting()
    //{
    //    if (!isParasiting && hostObject != null && targetDetected)
    //    {
    //        if (Player != null)
    //        {
    //            Player.enabled = false;    

    //            shadow.SetActive(false);
    //        }

    //        IsoObject playerIsoObject = GetComponent<IsoObject>();
    //        Vector3 newPosition = new Vector3(playerIsoObject.positionX, playerIsoObject.positionY, 50F);
    //        playerIsoObject.position = newPosition;




    //        playerSwitch.SwitchPlayer(hostObject.GetComponent<IParasiteHost>());
    //        // 隱藏子物件

    //        Debug.Log("寄生成功！");
    //        isParasiting = true;
    //    }
    //    else
    //    {
    //        Debug.Log("偵測不到寄生物");
    //    }
    //}
    public void ParanimationSlw()//animation事件
    {
        animator.speed = 0.5f;
    }
    public void ParanimationEnd()//animation事件
    {
     
        ParasitingPhase = parasitingPhase.parasiting;
        Debug.Log("抵達目標，進入 Parasiting 階段！");
       
    }

    private void HandleParasting()//寄生程序
    {


        switch (ParasitingPhase)
        {
            case parasitingPhase.prepare:

                Debug.Log("進入附身階段");

                animationParasite = true;

                //InputManager.Instance.gameObject.SetActive(false);
                Vector3 offset = new Vector3(0f, 0f, 0.1f);
                playercoillder.isTrigger = true;
                // 用 MoveTowards 慢慢靠近 parasitePOS
                isoObject.position = Vector3.MoveTowards(
                    isoObject.position,        // 當前位置
                    parasiteIso.position + offset,               // 目標位置
                    GoingParasiteSpeed * Time.deltaTime // 每秒移動速度
                );
                // 判斷是否到達目標
                if (Vector3.Distance(isoObject.position, parasiteIso.position + offset) < 0.05f)
                {
                    
                    var isoRigidbody = GetComponent<IsoRigidbody>();
                    isoRigidbody.isKinematic = true;
                    // 切換到 parasiting 階段
                    //InputManager.Instance.gameObject.SetActive(true);
                }
                break;


            case parasitingPhase.parasiting:
              
                if (!isGlowing)
                {
                    glowLerpTimer = 0f;
                    isGlowing = true;
                    secondFadeStarted = false; // 確保第二階段還沒開始
                }

                if (!secondFadeStarted)
                {
                  
                    // 第一階段：Glow從0升到10
                    glowLerpTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(glowLerpTimer / glowDuration);
                    float glowValue = Mathf.Lerp(1f, 50f, t);
                    float paraglowValue = Mathf.Lerp(1f, 90f, t);

                    PlayerMaterial.SetFloat("_GlowGlobal", glowValue);
                    parasiteMaterial.SetFloat("_GlowGlobal", paraglowValue);

                    if (t >= 1f)
                    {
                        animator.speed = 0f;
                        // 第一階段完成
                        secondFadeStarted = true;
                        glowLerpTimer = 0f; // 重置計時器給第二階段用
                        this.gameObject.GetComponent<GroundProjection>().DisableProjection();
                        PlayerSprite.enabled = false;
                        shadow.SetActive(false);
                        PlayerMaterial.SetFloat("_GlowGlobal", 1f);
                        // 調整player位置
                        IsoObject playerIsoObject = GetComponent<IsoObject>();
                        Vector3 newPosition = new Vector3(playerIsoObject.positionX, playerIsoObject.positionY, 50f);
                        playerIsoObject.position = newPosition;
                        

                    }
                }
                else
                {
                   

                    // 第二階段：parasiteMaterial的Glow從10降到0
                    glowLerpTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(glowLerpTimer / secondFadeDuration);
                    float paraglowValue = Mathf.Lerp(90f, 1f, t);

                    parasiteMaterial.SetFloat("_GlowGlobal", paraglowValue);

                    if (t >= 1f)
                    {
                        // 第二階段也完成了！
                        parasiteMaterial.SetFloat("_GlowGlobal", 1f);
                        animationParasite = false;
                        animator.speed = 1f;
                        

                        // 最後切換
                        playerSwitch.SwitchPlayer(hostObject.GetComponent<IControllable>());
                        isGlowing = false;
                        ParasitingPhase = parasitingPhase.None;
                        // 如果要，這裡可以考慮切換parasitingPhase
                    }
                }


                break;
            case parasitingPhase.None:
                break;
        }
    }


    public void EndParasiting()
    {
        Debug.Log("嘗試脫離附身");
        if (hostObject != null)
        {
            IsoObject hostIsoObject = hostObject.GetComponent<IsoObject>();
            if (hostIsoObject != null)
            {
                IsoObject playerIsoObject = GetComponent<IsoObject>();
                if (playerIsoObject != null)
                {
                    Vector3 newPosition = hostIsoObject.position + positionOffset;
                    playerIsoObject.position = newPosition;
                }
            }

            var isoRigidbody = GetComponent<IsoRigidbody>();
            if (isoRigidbody != null)
            {
                isoRigidbody.isKinematic = false;
                isoRigidbody.AddForce(new Vector3(0, 0, parasiteExitForce), ForceMode.Impulse);
            }

            if (PlayerSprite != null)
            {
                PlayerSprite.enabled = true;
                playercoillder.isTrigger = false;
                shadow.SetActive(true);
            }

            // 抓到材料
            PlayerMaterial = PlayerSprite.material;
            SpriteRenderer hostSprite = hostObject.GetComponent<SpriteRenderer>();
            if (hostSprite != null)
            {
                parasiteMaterial = hostSprite.material;
            }

            // 啟動亮度回復流程
            isExitingParasite = true;
            exitGlowTimer = 0f;

            playerSwitch.SwitchPlayer(this);
            isParasiting = false;

            Debug.Log("已脫離寄生，開始亮度回復！");
        }
    }

    public void LeaveParasite()
    {
        Debug.Log("玩家不能脫離附身");
    }

    //private void CheckGrounded()
    //{
    //    isoObject = GetComponent<IsoObject>();

    //    // 定義射線的起點和 Iso 世界方向
    //    Vector3 offset = new Vector3(0, 0, 1f); // 向上偏移 1 單位，以防止射線從內部發射
    //    Vector3 rayOrigin = isoObject.position + offset; // 使用 IsoObject 位置發射射線
    //    Vector3 rayDirection = new Vector3(0, 0, -1f).normalized; // 向 Iso 世界的「下方」發射

    //    // 定義碰撞資訊變數
    //    IsoRaycastHit isoHitInfo;

    //    // 使用 IsoPhysics.Raycast 進行射線檢測
    //    bool isHit = IsoPhysics.Raycast(
    //        new Ray(rayOrigin, rayDirection), // 射線
    //        out isoHitInfo,                   // 碰撞資訊
    //        maxRaycastDistance,               // 最大距離
    //        groundLayer.value,                // 只檢測地面圖層
    //        QueryTriggerInteraction.Ignore    // 忽略觸發器
    //    );

    //    // 繪製射線（可視化用）
    //    Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * maxRaycastDistance, isHit ? Color.green : Color.red, 100f);

    //    // 更新 isGrounded 狀態和與地面的距離
    //    if (isHit)
    //    {

    //        isGrounded = true;
    //        groundHeight = isoHitInfo.point.z; // 記錄地面的 Iso Z 軸高度
    //        float distanceToGround = isoHitInfo.distance; // 計算與地面的距離
    //      //Debug.Log($"玩家著地，地面高度: {groundHeight}, 與地面距離: {distanceToGround}");
    //    }
    //    else
    //    {
    //        isGrounded = false;
    //        //Debug.Log("玩家不在地面上");
    //    }
    //}




    //private void use()
    //{
    //    if (CanUsedoor)
    //    {
    //        cagedoor.SetStatus(true);   
    //    }
    //}
    //private void nouse()
    //{
    //    if (CanUsedoor)
    //    {
    //        cagedoor.SetStatus(false);
    //    }
    //}
    public void use()
    {
        if (currentItem != null && CanUse)
        {
            // 如果有實現 IUsable，觸發物品的 Use 方法
            currentItem.UseObject();
        }
       
    }   
   
    void ShowHint()
    {
        if (hintInstance == null)
        {
            hintInstance = Instantiate(interactionHintPrefab);
        }

        hintInstance.Show(this.transform); // 傳入玩家位置
    }

    void HideHint()
    {
        if (hintInstance != null)
        {
            hintInstance.Hide();
        }
    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
     {
        // 檢查是否是寄生物
        if (isoCollider.CompareTag(parasiteTag))
        {
            Debug.Log("偵測到寄生物: " + isoCollider.gameObject.name);
            hostObject = isoCollider.gameObject;
            targetDetected = true;
           
            // 新增的：直接抓 spriteRenderer 和 material
            SpriteRenderer hostSpriteRenderer = hostObject.GetComponentInChildren<SpriteRenderer>();
            IsoObject PARASITEOBJECT = hostObject.GetComponent<IsoObject>();
            if (hostSpriteRenderer != null)
            {
                parasiteIso = PARASITEOBJECT;
                parasiteSprite = hostSpriteRenderer;
                parasiteMaterial = hostSpriteRenderer.material;

                Debug.Log("已抓到寄生對象的 SpriteRenderer 和 Material");
            }
            else
            {
                Debug.LogWarning("寄生目標上找不到 SpriteRenderer！");
            }
        }
        
    }

      void OnIsoTriggerExit(IsoCollider isoCollider)
     {
         // 檢查是否是寄生物
         if (isoCollider.gameObject == hostObject)
         {
             Debug.Log("寄生物離開: " + isoCollider.gameObject.name);
             targetDetected = false;
          
        }
     }

     void OnIsoCollisionEnter(IsoCollision isoCollision)
     {

         if (isoCollision.gameObject.CompareTag("Door"))
         {
            // CanUsedoor = true;
             Debug.Log("可開門");
         }
        if (isoCollision.gameObject.CompareTag("Board"))
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("我太輕了..");
        }
        if (isoCollision.gameObject.CompareTag("Absorbable"))
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("推不動");
        }
        if (isoCollision.gameObject.CompareTag("ConversationObject"))
        {
            currentItem = isoCollision.gameObject.GetComponent<IUsable>();
            CanUse = true;
            ShowHint();
        }

    }

     void OnIsoCollisionExit(IsoCollision isoCollision)
     {

         if (isoCollision.gameObject.CompareTag("Door"))
         {
             //CanUsedoor = false;
             Debug.Log("不可開門");
         }
        if (isoCollision.gameObject.CompareTag("ConversationObject"))
        {
            CanUse = false;
            currentItem = isoCollision.gameObject.GetComponent<IUsable>();
            currentItem.UnUseObject();
            HideHint();
        }
    }


}
