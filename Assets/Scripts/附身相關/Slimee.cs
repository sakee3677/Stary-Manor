using IsoTools;
using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InputManager;


public class Slimee : MonoBehaviour, IControllable
{
    private IsoObject isoObject;
    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;


    public Material parasiteMaterial;
    public SpriteRenderer parasiteSprite;
    
    public float secondFadeDuration = 1f; // 第二階段多久內降到0

    public bool isParasited = false;
    public float speed = 2f; // 移動速度
    public float jumpVelocity = 0.5f;
    private Vector2 lastInput = Vector2.zero;
    private Vector2 lastDirection = Vector2.left;
    public Vector2 moveInput;
    public bool isGrounded;
    public bool CanSpit=true;
    public bool isAbsorbing;
    private Vector3 LastPosition;

    public bool iseating;
    private GameObject pendingAbsorbObject; // 暫存等待吸收的物體
    private GameObject absorbedObject = null; // 儲存吸收的物件
    public IsoObject spitOutPoint;

    public float groundNum = 0;

    public float offsetdis;
    public float offsetZ;
    private Animator animator; // 用於控制子項目的Animator
  
    public RuntimeAnimatorController newController;
    public RuntimeAnimatorController OrgController;

    private Slime controls;
    public PlayerController playerController;
    // Start is called before the first frame update

    private void Awake()
    {
       

        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();

     
     
      
        
        animator = GetComponentInChildren<Animator>();
    }
  

   
    public void ActivateHost()//啟用該寄生物調用
    {
        var isoRigidbody = GetComponent<IsoRigidbody>();
        isoRigidbody.isKinematic = false;
        animator.runtimeAnimatorController = newController;
        // 啟用被寄生物的控制邏輯
        gameObject.tag = "PlayerParasite";
        
        isParasited = true; // 設置為玩家控制狀態
        IsoWorld.GetWorld(0).gravity = new Vector3(0, 0, -9f);

    }

    public void DeactivateHost()//關閉該寄生物調用
    {
        // 停用被寄生物的控制邏輯
        animator.runtimeAnimatorController = OrgController;
        gameObject.tag = "ParasiteHost";
        
        isParasited = false; // 回到自主行動狀態
        moveInput = Vector2.zero; // 重設玩家輸入
        

    }


    void Start()
    {
        isParasited = false;
        isoObject = GetComponent<IsoObject>();
        LastPosition = isoObject.position;

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

            Move();

            // 更新吐出點的位置（每一幀根據面向方向）
            UpdateSpitOutPoint();


            if (CanSpit &&( Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton4))) // 按 R 嘗試吐出物體
            {
                Debug.Log("執行兔出");
                SpitOutObject();
            }
            else if(!CanSpit && absorbedObject != null &&( Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton4)))
            {
               
                GetComponent<FloatingTextSpawner>().ShowFloatingText("離牆壁太近，無法吐出 !");

            }
           
               
            

        }

       if(!isParasited)
        {
            this.gameObject.GetComponent<GroundProjection>().DisableProjection();
            isoObject = GetComponent<IsoObject>();
            isoObject.position = new Vector3(LastPosition.x, LastPosition.y, isoObject.position.z);
            
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
    
    private void UpdateSpitOutPoint()
    {
        if (spitOutPoint == null) return;

        // 取得目前 slime 的 IsoObject 位置
        Vector3 basePosition = GetComponent<IsoObject>().position;

        // 根據移動方向來偏移
        float offsetDistance = offsetdis;
        float ZoffsetDistance = offsetZ;
        Vector3 directionOffset = new Vector3(lastDirection.x, lastDirection.y, 2f).normalized * offsetDistance;

        // 設定 spitOutPoint 的位置（保持 z 軸不變）
        Vector3 targetPosition = basePosition + directionOffset;
        targetPosition.z = basePosition.z + ZoffsetDistance; // 確保 z 軸不變

        spitOutPoint.position = targetPosition;
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



        UpdateAnimation(moveInput);

        //Debug.Log($"Current Status: {animator.GetInteger("Status")}");
        // 設定速度，保留垂直方向速度
      
            Vector3 velocity = new Vector3(inputDirection.x * speed, inputDirection.y * speed, isoRigidbody.velocity.z);
            isoRigidbody.velocity = velocity;
        
        
       
    }

    public void UpdateAnimation(Vector2 direction)
    {
        bool isMoving = direction != Vector2.zero;
        

        // 設定 Animator 參數
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsEating", iseating);

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
    public void LeaveParasite()
    {
        playerController.EndParasiting();
    }
    public void use()
    {

    }
    public void OnParasite()
    {
        Debug.Log("已經附身史萊姆了");
    }

    // 呼叫這個來吐出物件
    public void SpitOutObject()
    {

        if (absorbedObject == null) return;

        // 使用 IsoObject 的方式來設置位置
        IsoObject iso = absorbedObject.GetComponent<IsoObject>();
        if (iso != null)
        {
            iso.position = spitOutPoint.position;
        }

        // 還原 IsoBoxCollider 為非 trigger
        IsoBoxCollider isoCollider = absorbedObject.GetComponent<IsoBoxCollider>();
        if (isoCollider != null)
        {
            isoCollider.isTrigger = false;
        }

        // 還原 SpriteRenderer 可見
        SpriteRenderer sprite = absorbedObject.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.enabled = true;
        }

        Debug.Log($"吐出物件：{absorbedObject.name}");
        absorbedObject = null;
    }

    // 呼叫這個方法來吸收某個物件
    public void AbsorbObject(GameObject obj)
    {
        if (absorbedObject != null) return; // 只能吸收一個
        
        absorbedObject = obj;

        // 取得 IsoBoxCollider 並改成 isTrigger
        IsoBoxCollider isoCollider = obj.GetComponent<IsoBoxCollider>();
        if (isoCollider != null)
        {
            isoCollider.isTrigger = true;
        }

        // 禁用 SpriteRenderer 讓物件不可見
        SpriteRenderer sprite = obj.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.enabled = false;
        }

        Debug.Log($"吸收了物件：{obj.name}");
    }

    public void stopEatingAnimation()
    {
       
        iseating = !true;
    }
    public void OnAbsorbAnimationEvent()
    {

        if (pendingAbsorbObject != null)
        {
            AbsorbObject(pendingAbsorbObject);
            pendingAbsorbObject = null; // 清除暫存，避免重複吸收
        }
    }
    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {


        if (isoCollider.CompareTag("Ground"))
        {
            groundNum++;
            //playerController.isGrounded = true;
            Debug.Log("地板數" + groundNum);
        }
    }

    void OnIsoTriggerExit(IsoCollider isoCollider)
    {

    
        if (isoCollider.CompareTag("Ground"))
        {
            groundNum--;
            // playerController.isGrounded = false;
            Debug.Log("地板數" + groundNum);
        }
    }
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        
        if (isoCollision.gameObject.CompareTag("Absorbable") && absorbedObject == null && isParasited)
        {
            iseating = true;
            pendingAbsorbObject = isoCollision.gameObject;            
            Debug.Log("碰到吸收物");
        }
    }
    void OnIsoCollisionExit(IsoCollision isoCollision)
    {
    }

    }
