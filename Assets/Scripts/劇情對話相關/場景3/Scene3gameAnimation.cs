using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class Scene3gameAnimation : MonoBehaviour
{
    public bool testing;
    public bool Untesting;
    public Material PlayerMaterial;
    public SpriteRenderer PlayerSprite;
    private float exitGlowTimer;
    public float exitGlowDuration = 4f; 


    public PhysicMaterial newMaterial;
    public IsoBoxCollider isoBoxCollider;       
    public PhysicMaterial OrgMaterial;
    private bool changed;
   

    public Transform target; // 主角
    public Vector3 followOffset = new Vector3(0, 0, -10);
    public bool isFollowing = false;
    public GameObject Panel;
    public GameObject chat;
    public GameObject talker;

    public Vector3 cinematicTargetPos; // 攝影機最終要移動的位置
    public float startOrthoSize = 5f; // 開場攝影機大小
    public Vector3 StartTargetPos;
    public float targetOrthoSize = 10f; // 運鏡後的大小
    public float transitionDuration = 2f;

    public  Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        changed = false;
     
        isFollowing = false;
        cam = Camera.main;
        cam.orthographicSize = startOrthoSize;

        if (target != null)
        {
            cam.transform.position = StartTargetPos;
            CamStartFollow();
        }
    }

 


    void LateUpdate()
    {
        if (isFollowing && target != null)
        {
            cam.transform.position = target.position + followOffset;
        }

        if (testing)
        {
            exitGlowTimer += Time.deltaTime;
            float t = Mathf.Clamp01(exitGlowTimer / exitGlowDuration);
            float AlphaOutline = Mathf.Lerp(1f, 20f, t);
            float HandDrawn = Mathf.Lerp(0f, 3f, t);

            if (PlayerMaterial != null)
                PlayerMaterial.SetFloat("_AlphaOutlineGlow", AlphaOutline);
            if (PlayerMaterial != null)
                PlayerMaterial.SetFloat("_HandDrawnAmount", HandDrawn);
        }

    }
    public void CamStartFollow()
    {

       
        StartCoroutine(CamStartFollowCoroutine());
        DialogueManager.Instance.RegisterEventCallback();
    }
    private IEnumerator CamStartFollowCoroutine()
    {
        yield return null;
        isFollowing = true;
        
    }
        public void TriggerCinematicMovement()
    {
        isFollowing = false;
        StartCoroutine(CinematicMoveAndZoom());
    }

    private IEnumerator CinematicMoveAndZoom()
    {
     
        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;
        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;
            cam.transform.position = Vector3.Lerp(startPos, cinematicTargetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetOrthoSize, t);

            timer += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = cinematicTargetPos;
        cam.orthographicSize = targetOrthoSize;
        DialogueManager.Instance.RegisterEventCallback();
       
      
    }
    
    public void wait3sec()
    {
        StartCoroutine(wait3secCoroutin());
    }
    private IEnumerator wait3secCoroutin()
    {
        yield return new WaitForSeconds(2.5f);
        DialogueManager.Instance.RegisterEventCallback();
    }


    public void ChangeMaterial()
    {
       
        if (changed==false)
        {
            isoBoxCollider.material = newMaterial;
            changed = !changed;
        }
       else if (changed == true)
        {
            isoBoxCollider.material = OrgMaterial;
            changed = !changed;
        }
         DialogueManager.Instance.RegisterEventCallback();
    }

    public void testdetect()
    {
        Untesting = false;
        testing = true;
        DialogueManager.Instance.RegisterEventCallback();
    }
    public void Untestdetect()
    {
        testing = false;
        
        DialogueManager.Instance.RegisterEventCallback();
    }

}
