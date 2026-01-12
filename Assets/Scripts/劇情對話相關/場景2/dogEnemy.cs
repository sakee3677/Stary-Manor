using IsoTools;
using IsoTools.Physics;
using UnityEngine;

public class dogEnemy : MonoBehaviour
{


    public AudioSource audioSource;
    public AudioClip footstepClips;

    public Vector2 moveInput;
    public bool isGrounded;
    public float speed = 2f;
    public float jumpVelocity = 0.5f;
    public bool isSleeping = false;

    private Vector2 lastInput = Vector2.zero;
    private Vector2 lastDirection = Vector2.left;

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener isoTriggerListener;
    private IsoRigidbody isoRigidbody;
    public Animator animator;

    public bool isAutoMoving = false;

    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        isoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
        isoRigidbody = GetComponent<IsoRigidbody>();
        animator = GetComponentInChildren<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

        }
    }

    private void Start()
    {
        isSleeping = false;
    }

    private void Update()
    {
        
        UpdateAnimation(moveInput);
        Move();



        
    }


    public  void UpdateMoveInput(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            moveInput = Vector2.zero;
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            moveInput = new Vector2(Mathf.Sign(input.x), 0);
        }
        else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            moveInput = new Vector2(0, Mathf.Sign(input.y));
        }
        else
        {
            if (input != lastInput)
            {
                Vector2 delta = input - lastInput;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    moveInput = new Vector2(Mathf.Sign(delta.x), 0);
                }
                else
                {
                    moveInput = new Vector2(0, Mathf.Sign(delta.y));
                }
            }
        }

        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        lastInput = input;
    }


    private void Move()
    {
        if (isoRigidbody == null) return;
        Vector3 velocity = new Vector3(moveInput.x * speed, moveInput.y * speed, isoRigidbody.velocity.z);
        isoRigidbody.velocity = velocity;
    }

    private void Jump()
    {
        if (isGrounded && isoRigidbody != null)
        {
            isoRigidbody.AddForce(new Vector3(0, 0, jumpVelocity), ForceMode.Impulse);
        }
    }

    private void UpdateAnimation(Vector2 direction)
    {
        bool isMoving = direction != Vector2.zero;

        if (!isSleeping && isMoving)
        {
            lastDirection = direction;
        }

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsSleeping", isSleeping);

        if (isSleeping)
        {
            animator.SetFloat("SleepDir", lastDirection.y > 0 ? 1 : -1);
        }
        else
        {
            if (isMoving)
            {
                // 這裡檢查moveInput.magnitude，並根據速度更新pitch
                if (!audioSource.isPlaying)
                {
                    // 計算速度因素
                    float speedFactor = moveInput.magnitude * speed; // 根據輸入與速度計算
                    float dynamicPitch = Mathf.Clamp(0.4f + speedFactor * 0.05f, 0.1f, 1.5f); // 增大變化範圍

                    audioSource.pitch = dynamicPitch;
                    audioSource.clip = footstepClips;
                    audioSource.Play();
                }
                // 輸出當前的音量和音調到控制台
                Debug.Log($"Current Pitch: {audioSource.pitch}, Current Volume: {audioSource.volume}");
                animator.SetFloat("MoveX", direction.x);
                animator.SetFloat("MoveY", direction.y);
                animator.speed = 1f;
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                animator.SetFloat("MoveX", lastDirection.x);
                animator.SetFloat("MoveY", lastDirection.y);
                animator.speed = 0f;
            }
        }
    }

    private void OnIsoTriggerEnter(IsoCollider isoCollider)
    {
        if (isoCollider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnIsoTriggerExit(IsoCollider isoCollider)
    {
        if (isoCollider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}