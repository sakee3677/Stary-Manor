using IsoTools;
using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using static InputManager;

public class parasite : MonoBehaviour, IControllable
{
    [SerializeField] public IsoObject isoObject;
    private ParasiteHost controls;
    public PlayerController playerController;
    public parasite parasiteController;
    

    public Vector2 moveInput;

    public float groundNum = 0;
    public bool isParasited = false;
    public bool isGrounded;
    public float speed = 2f; // 移動速度
    public float jumpVelocity = 0.5f;
    private Vector2 lastInput = Vector2.zero;
    private Vector2 lastDirection = Vector2.left;
    private Vector3 LastPosition;

    public bool unparasiteSleep;
    public bool isSleeping; // 在你其他程式切換這個值即可控制睡著狀態
    public bool canAttack;

    [Header("Patrol Settings")]
    public List<Vector3> targetPositions = new List<Vector3>(); // 在 Inspector 中手動設置巡邏點
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    private Vector3 currentTarget;
    private bool hasTarget = false;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;
    private bool movingOnX = true; // 控制現在正在走哪個軸
    public bool isPatrolling = true; // 控制是否啟用巡邏


    public InteractPromptFader interactionHintPrefab;
    private InteractPromptFader hintInstance;
    private IUsable currentItem; // 當前選中的物品
    public bool CanUse = false;
    public enum AttackPhase
    {
        None,
        Prepare,
        Wait,
        Charge,
        Recover
    }

    [Header("Attack Settings")]
    public float waitBeforeAttackTime = 0.3f;
    public float attackTurnTime = 0.5f; // 預備時間  
    public float chargeDuration = 0.6f;    // 衝刺時間
    public float recoverTime = 1.0f;       // 衝刺後等待時間
    private bool isAttacking = false;
    private Vector3 attackDirection;
    private float attackTimer = 0f;
    
    private AttackPhase attackPhase = AttackPhase.None;


    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float chargeSpeed = 5f;


    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;

    private Animator animator; // 用於控制子項目的Animator
    public RuntimeAnimatorController newController;
    public RuntimeAnimatorController OrgController;

    private void Awake()
    {
      

        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
        parasiteController = GetComponent<parasite>();
    
    
   
        animator = GetComponentInChildren<Animator>();



    }

    // IParasiteHost接口實現
    public void ActivateHost()//啟用該寄生物調用
    {
        animator.runtimeAnimatorController = newController;
        var isoRigidbody = GetComponent<IsoRigidbody>();
        isoRigidbody.isKinematic = false;
        // 啟用被寄生物的控制邏輯
        gameObject.tag = "PlayerParasite";
       
        isParasited = true; // 設置為玩家控制狀態
        IsoWorld.GetWorld(0).gravity = new Vector3(0, 0, -9f);
        isSleeping = !true;
        canAttack = !true;
    }
  


    public void DeactivateHost()//關閉該寄生物調用
    {
        animator.runtimeAnimatorController = OrgController;
        // 停用被寄生物的控制邏輯
        gameObject.tag = "ParasiteHost";
        
        isParasited = false; // 回到自主行動狀態
        moveInput = Vector2.zero; // 重設玩家輸入
       
        canAttack = true;


    }
    void Start()
    {
        isoObject = GetComponent<IsoObject>();
        LastPosition = isoObject.position;
        if (unparasiteSleep)
        {
            isSleeping = true;
        }
        else
        {
            isSleeping = false;
            isPatrolling = true;
        }
      
    }

    void Update()
    {
     
        
            UpdateAnimation(moveInput);
       

        if (isParasited)
        {
           
            this.gameObject.GetComponent<GroundProjection>().EnableProjection();
            this.gameObject.GetComponent<GroundProjection>().UNSetDotColor();
            isoObject = GetComponent<IsoObject>();
            LastPosition = isoObject.position;          
            // 玩家控制邏輯
            Move();
        }
        else if(!isParasited && unparasiteSleep)//如果脫離附身就睡覺
        {
            this.gameObject.GetComponent<GroundProjection>().DisableProjection();
            isoObject = GetComponent<IsoObject>();
            isoObject.position = new Vector3(LastPosition.x, LastPosition.y, isoObject.position.z);
            isSleeping = true;
        }
        else
        {
            if (isAttacking)
            {
                HandleAttack();
            }
            else if(isPatrolling)
            {
                Patrol();
            }
        }



        if (groundNum >= 1)
        {
            isGrounded = true;
            
        }
        else
        {
            isGrounded = false;
        }

       
       
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
       
        Vector3 velocity = new Vector3(inputDirection.x * speed, inputDirection.y * speed, isoRigidbody.velocity.z);
            isoRigidbody.velocity = velocity;
        
       
    }
    public void UpdateAnimation(Vector2 direction)
    {
        bool isMoving = direction != Vector2.zero;

        // 更新方向記錄（只在非睡覺時更新，睡覺時保留上次方向）
        if (!isSleeping && isMoving)
        {
            lastDirection = direction;
        }

        // Animator 參數：移動狀態
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSleeping", isSleeping); // 可在 Animator 製作 Sleep blend tree 或 animation layer

        if (isSleeping)
        {
            animator.speed = 1f;
            // 睡覺狀態下，只用上 or 下的動畫
            if (lastDirection.y > 0)
            {
                animator.SetFloat("SleepDir", 1); // 上
            }
            else
            {
                animator.SetFloat("SleepDir", -1); // 下（預設）
            }
        }
        else
        {
            // 非睡覺狀態，照方向撥放動畫
            if (isMoving)
            {
                animator.SetFloat("MoveX", direction.x);
                animator.SetFloat("MoveY", direction.y);
                animator.speed = 1f; // 正常播放動畫
            }
            else
            {
                // 不動時保留最後方向（保持站立朝向動畫）
                animator.SetFloat("MoveX", lastDirection.x);
                animator.SetFloat("MoveY", lastDirection.y);
                animator.speed = 0f; // 停止動畫，定格

            }
        }
    }



    public void OnJump()
    {
        var isoRigidbody = GetComponent<IsoRigidbody>();
        if (isGrounded)
        {
            if (isoRigidbody != null)
            {
                // 添加跳躍力
                isoRigidbody.AddForce(new Vector3(0, 0, jumpVelocity), ForceMode.Impulse);

            }

        }
    }
    public void OnJumpCanceled()
    {

    }
    private void Patrol()
    {
        if (targetPositions.Count == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
                hasTarget = false; // 清除目前目標，下次重新選擇
            }
            else
            {
                Debug.Log($"已抵達巡邏點 ({currentTarget.x}, {currentTarget.y})，等待中... {waitDuration:F2} 秒");
            }
            return;
        }

        if (!hasTarget)
        {
            int randomIndex = -1;

            // 至少兩個巡邏點才做排除，目前點
            if (targetPositions.Count > 1)
            {
                do
                {
                    randomIndex = Random.Range(0, targetPositions.Count);
                }
                while (targetPositions[randomIndex] == currentTarget);
            }
            else
            {
                randomIndex = 0; // 只有一個點就只能選它
            }

            currentTarget = targetPositions[randomIndex];
            hasTarget = true;
            movingOnX = true; // 每次新目標都從 X 軸開始走
            Debug.Log($"前往新的巡邏點 ({currentTarget.x}, {currentTarget.y})");
        }

        Vector3 direction = currentTarget - isoObject.position;
        moveInput = Vector2.zero;

        // 先走 X 軸，如果 X 已經到了，再走 Y
        if (movingOnX)
        {
            if (Mathf.Abs(direction.x) > 0.05f)
            {
                moveInput = new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                movingOnX = false; // X 軸走完，切換到 Y 軸
            }
        }

        if (!movingOnX)
        {
            if (Mathf.Abs(direction.y) > 0.05f)
            {
                moveInput = new Vector2(0, Mathf.Sign(direction.y));
            }
        }

        Move();

        // 判斷是否到達目標
        Vector2 isoPos2D = new Vector2(isoObject.position.x, isoObject.position.y);
        Vector2 target2D = new Vector2(currentTarget.x, currentTarget.y);
        if (Vector2.Distance(isoPos2D, target2D) < 0.2f)
        {
            isWaiting = true;
            waitTimer = 0f;
            waitDuration = Random.Range(minWaitTime, maxWaitTime);
            moveInput = Vector2.zero;
        }
    }




    public void use()
    {
        if (currentItem != null && CanUse)
        {
            // 如果有實現 IUsable，觸發物品的 Use 方法
            currentItem.UseObject();
        }

    }

    public void LeaveParasite()
    {
        playerController.EndParasiting();
        this.gameObject.GetComponent<GroundProjection>().DisableProjection();
    }

    public void OnParasite()
    {
        Debug.Log("已經附身了");
    }

    public void Attack(Vector3 targetPosition)
    {
        if (!canAttack || isAttacking) return;

        float xDiff = Mathf.Abs(targetPosition.x - isoObject.position.x);
        float yDiff = Mathf.Abs(targetPosition.y - isoObject.position.y);

        // 玩家與敵怪是否在X或Y軸上幾乎對齊（用更嚴格的條件）
        if (xDiff < 0.8f || yDiff < 0.8f)
        {
            Debug.Log("攻擊玩家位置：" + targetPosition);
            isAttacking = true;
            attackPhase = AttackPhase.Prepare;
            attackTimer = 0f;

            Vector3 diff = targetPosition - isoObject.position;

            // 先決定是哪一軸比較接近（代表要沿著那個軸移動）
            if (Mathf.Abs(xDiff) < Mathf.Abs(yDiff))
            {
                // X 軸比較接近 → 攻擊沿 Y 軸
                attackDirection = new Vector3(0, Mathf.Sign(diff.y), 0);
            }
            else
            {
                // Y 軸比較接近 → 攻擊沿 X 軸
                attackDirection = new Vector3(Mathf.Sign(diff.x), 0, 0);
            }

            moveInput = Vector2.zero;
            hasTarget = false;
            isWaiting = false;
        }
    }

    private void HandleAttack()
    {
        attackTimer += Time.deltaTime;

        switch (attackPhase)
        {
          

            case AttackPhase.Prepare:
                moveInput = attackDirection;
                isoObject.position += (Vector3)(moveInput * normalSpeed * Time.deltaTime);
                if (attackTimer >= attackTurnTime)
                {
                    attackTimer = 0f;
                    attackPhase = AttackPhase.Wait;
                    Debug.Log("準備完畢，開始衝刺！");
                }
                break;
            case AttackPhase.Wait:
                moveInput = Vector2.zero;
                isoObject.position += Vector3.zero;
                // 聽到聲音的延遲
                if (attackTimer >= waitBeforeAttackTime)
                {
                    attackTimer = 0f;
                    attackPhase = AttackPhase.Charge;
                    Debug.Log("延遲完畢，準備攻擊！");
                }
                break;

            case AttackPhase.Charge:
                var parasiteController = GetComponent<parasite>();
         
                moveInput = new Vector2(attackDirection.x, attackDirection.y);
                isoObject.position += (Vector3)(moveInput * chargeSpeed * Time.deltaTime);

       
                if (attackTimer >= chargeDuration)
                {
                    attackTimer = 0f;
                    attackPhase = AttackPhase.Recover;
                    moveInput = Vector2.zero;

                    Debug.Log("衝刺結束，進入回復階段");
                }
                break;

            case AttackPhase.Recover:
                if (attackTimer >= recoverTime)
                {
                    isAttacking = false;
                    attackPhase = AttackPhase.None;
                    attackTimer = 0f;
                    Debug.Log("回復結束，回到巡邏狀態");
                }
                break;
        }
    }

    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {

        if (isoCollider.CompareTag("Ground"))
        {
            groundNum++;
            Debug.Log("寄生物偵測到地板");

        }
    }
    void OnIsoTriggerExit(IsoCollider isoCollider)
    {

        if (isoCollider.CompareTag("Ground"))
        {
            groundNum--;
            Debug.Log("寄生物離開地板");
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
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("ConversationObject"))
        {
            currentItem = isoCollision.gameObject.GetComponent<IUsable>();
            CanUse = true;
            ShowHint();
        }
    }
    void OnIsoCollisionExit(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("ConversationObject"))
        {
            CanUse = false;
            currentItem = isoCollision.gameObject.GetComponent<IUsable>();
            currentItem.UnUseObject();
            HideHint();
        }
    }
    }
