using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueLine
{
    public string speaker;  // 說話者名稱
    [TextArea(3, 5)]
    public string sentence; // 對話內容
    public UnityEvent onDialogueEvent = new UnityEvent();
}

public class DialogueTrigger : MonoBehaviour
{
    public GroundProjection PLAYERGroundProjection;
    public string speakerName = "NPC"; // 預設名稱
    public DialogueLine[] dialogueLines; // Inspector 內可修改
    private InputManager playercontrol;
  

    public bool needDestory = true;
    

    //Inspector 內可設定哪些 tag 可以觸發對話**
    public List<string> triggerTags = new List<string>();
    private void Start()
    {
     
    }


    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {


        if (triggerTags.Contains(isoCollider.tag))
        {
            InputManager.Instance.SetInputEnabled(false);
            if (PLAYERGroundProjection != null)
            {
                PLAYERGroundProjection.DisableProjection();
            }
            var myCollider = GetComponent<IsoCollider>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }
            DialogueManager.Instance.StartDialogue(dialogueLines, this);  // ← 傳自己進去


           


        }


    }

    public void destroyDialogueTrigger()
    {
       
            
            if (!needDestory)
        {
            if (PLAYERGroundProjection != null)
            {
                PLAYERGroundProjection.EnableProjection();
            }
            InputManager.Instance.SetInputEnabled(true);
            Debug.Log("保留");
                this.gameObject.SetActive(true);

                // 開始協程來延遲啟用 Collider
                StartCoroutine(EnableColliderWithDelay(2f)); // ← 延遲 1.5 秒，可自行調整時間
            }
            else
            {
            if (PLAYERGroundProjection != null)
            {
                PLAYERGroundProjection.EnableProjection();
            }
            InputManager.Instance.SetInputEnabled(true);
            this.gameObject.SetActive(false); 
                 Debug.Log("!保留");
              
            }
        }
    
 

    private IEnumerator EnableColliderWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        var myCollider = GetComponent<IsoCollider>();
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }
    }
}
  

