using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public interface IControllable
    {
        void OnMove(Vector2 direction);
        void OnJump();
        void OnJumpCanceled();
        void OnParasite();
        void LeaveParasite();
        void ActivateHost();
        void DeactivateHost();
        void use();
    }

    public static InputManager Instance { get; private set; }
  
    private bool inputEnabled = true;
    private ALLplayer controls;
    private IControllable currentController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new ALLplayer();
        BindInputs();


    }
    private void Update()
    {
        if (inputEnabled)
        {
            

        }
        else
        {
           
            currentController?.OnMove(Vector2.zero);
        }
    }
   

private void OnEnable() => controls?.Enable();
    private void OnDisable() => controls?.Disable();

    void BindInputs()
    {
        controls.movement.move.performed += ctx =>
        {
            if (!inputEnabled) return;
            Vector2 input = ctx.ReadValue<Vector2>();
            currentController?.OnMove(input);
        };
        controls.movement.move.canceled += ctx =>
        {
      
            if (!inputEnabled) return;
            currentController?.OnMove(Vector2.zero);
        };

        controls.movement.jump.performed += ctx =>
        {
         
            if (!inputEnabled) return;
            currentController?.OnJump();
        };
        controls.movement.jump.canceled += ctx =>
        {
          
            if (!inputEnabled) return;
            currentController?.OnJumpCanceled();
        };
        controls.movement.OnLeaveParasite.performed += ctx =>
        {
         
            if (!inputEnabled) return;
            currentController?.LeaveParasite();
        };
        controls.movement.Parasite.performed += ctx =>
        {
          
            if (!inputEnabled) return;
            currentController?.OnParasite();
        };
        controls.movement.use.performed += ctx =>
        {
          
            if (!inputEnabled) return;
            currentController?.use();
        };
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        // 當禁用輸入時，強制讓角色停止移動
        if (!enabled)
        {
            currentController?.OnMove(Vector2.zero);
        }
    }
   



    public void SetCurrentController(IControllable controller) => currentController = controller;

    public Vector2 GetCurrentMoveInput() => controls.movement.move.ReadValue<Vector2>();

    // 靜態初始化方式：若還沒有則生成
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitIfNeeded()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("InputManager");
            go.AddComponent<InputManager>();
        }
    }
}
