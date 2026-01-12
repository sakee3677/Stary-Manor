using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class scene1_5Animation : MonoBehaviour
{
    public Transform target; // 主角

    public Vector3 cinematicTargetPos; // 攝影機最終要移動的位置
    public float startOrthoSize = 5f; // 開場攝影機大小
    public Vector3 StartTargetPos;
    public float targetOrthoSize = 10f; // 運鏡後的大小
    public float transitionDuration = 2f;
    public PlayerController playercontroller;
    public Camera cam;

    void Start()
    {
       
        cam = Camera.main;
        cam.orthographicSize = startOrthoSize;
        if (target != null)
        {
            cam.transform.position = StartTargetPos;
        }
    }

    public void TriggerCinematicMovement()
    {
        StartCoroutine(CinematicMoveAndZoom(StartTargetPos, cinematicTargetPos, startOrthoSize, targetOrthoSize));
        DialogueManager.Instance.RegisterEventCallback();
    }

    public void ReverseCinematicMovement()
    {
        StartCoroutine(CinematicMoveAndZoom(cinematicTargetPos, StartTargetPos, targetOrthoSize, startOrthoSize));
        DialogueManager.Instance.RegisterEventCallback();
    }

    private IEnumerator CinematicMoveAndZoom(Vector3 fromPos, Vector3 toPos, float fromSize, float toSize)
    {
        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;
            cam.transform.position = Vector3.Lerp(fromPos, toPos, t);
            cam.orthographicSize = Mathf.Lerp(fromSize, toSize, t);

            timer += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = toPos;
        cam.orthographicSize = toSize;

     
    }
    public void playerScared()
    {
        StartCoroutine(playerTurn());
        
    }
    private IEnumerator playerTurn()
    {

        yield return new WaitForSeconds(2f);
        playercontroller.lastDirection = Vector2.up; // 你要轉向的方向，例如上
        playercontroller.UpdateAnimation(Vector2.zero); // 傳 zero 進去，表示原地不動
        DialogueManager.Instance.RegisterEventCallback();
        
    }
 

}
