using IsoTools.Physics;
using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    [Tooltip("上樓方向，與玩家 moveInput 一致才會啟動")]
    public Vector2 climbDirection = Vector2.down;

    [Tooltip("每次碰撞時抬升的高度")]
    private float stepHeight = 0.05f;

  

   
    public float stairNUM=0;
    public bool cangoUP;
 
    private PlayerController getplayerGameObject;

    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }


    private void Update()
    {
        if (stairNUM > 0 && getplayerGameObject != null)
        {
            cangoUP = true;
        }
        else
        {
            cangoUP = false;
        }

        if (cangoUP)
        {
            Vector2 input = getplayerGameObject.moveInput.normalized;
            Vector2 stairDir = climbDirection.normalized;

            if (Vector2.Dot(input, stairDir) > 0.5f) // 放寬一點
            {
                float speedFactor = 200f;//速度
                getplayerGameObject.isoObject.position += new Vector3(0, 0, stepHeight * Time.deltaTime * speedFactor);
            }
        }
    }

    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (!isoCollision.gameObject.CompareTag("Player")) return;

        stairNUM++;
        getplayerGameObject = isoCollision.gameObject.GetComponent<PlayerController>();
    }

    void OnIsoCollisionExit(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("Player"))
        {
            stairNUM--;
            if (stairNUM <= 0)
            {
                stairNUM = 0;
                getplayerGameObject = null;
            }
        }
    }
}
