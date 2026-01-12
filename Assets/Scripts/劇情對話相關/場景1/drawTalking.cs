using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawTalking : MonoBehaviour
{
    private IsoCollisionListener collisionListener;
    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
    }
    // Start is called before the first frame update
   

  
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        Debug.Log("觸碰到畫");
        if (isoCollision.gameObject.CompareTag("Player"))
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("還在摸什麼，快去找!");
        }
    }
    public void drawTeachJump()
    {
        DialogueManager.Instance.RegisterEventCallback();
        StartCoroutine(DrawTeachJump());
    
    }
    IEnumerator DrawTeachJump()
    {
        while (true)
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("按下A跳躍，按住飄逸更遠");
            yield return new WaitForSeconds(6.5f); // 等待 1 秒
        }
    }
    public void drawTeachpara()
    {

        DialogueManager.Instance.RegisterEventCallback();
        StartCoroutine(DrawTeachpara());

    }
    IEnumerator DrawTeachpara()
    {
        yield return new WaitForSeconds(2F);
        while (true)
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("在生物上方時按下RT附身後，來觸發機關");
            yield return new WaitForSeconds(6.5f); // 等待 1 秒
        }
    }
    public void drawTeachUnpara()
    {
        
        DialogueManager.Instance.RegisterEventCallback();
        StartCoroutine(DrawTeachUnpara());

    }
    IEnumerator DrawTeachUnpara()
    {
        yield return new WaitForSeconds(2F);
        while (true)
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("按下LT衝出附身物，空中按住A飄移更遠");
            yield return new WaitForSeconds(6.5f); // 等待 1 秒
        }
    }
    public void drawTeachSlime()
    {

        DialogueManager.Instance.RegisterEventCallback();
        StartCoroutine(DrawTeachSlime());

    }
    IEnumerator DrawTeachSlime()
    {
        yield return new WaitForSeconds(2F);
        while (true)
        {
            GetComponent<FloatingTextSpawner>().ShowFloatingText("史萊姆可吃下物體，按下LB可吐出");
            yield return new WaitForSeconds(10f); // 等待 1 秒
        }
    }
}
