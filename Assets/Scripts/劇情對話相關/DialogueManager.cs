using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [HideInInspector]
    public DialogueTrigger currentTrigger;
    private int remainingCallbacks = 0; // 新增成員變數
    [Header("UI 設定")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    private Queue<DialogueLine> sentences;
    


    [Header("對話狀態")]
    private bool isDialogueActive = false;
    public PlayerController playerControl;
 
    public bool eventCompleted = false;
    public bool isWaitingForInput = false;

    void Awake()
    {
        Instance = this;
        sentences = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueLine[] dialogueLines, DialogueTrigger trigger)
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        sentences.Clear();
        currentTrigger = trigger;  //  記住是誰觸發的

        foreach (DialogueLine line in dialogueLines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {

        if (sentences.Count == 0)
        {
          
            EndDialogue();
            return;
        }
        DialogueLine line = sentences.Dequeue();
        speakerText.text = line.speaker;
        dialogueText.text = line.sentence;
       
        Debug.Log("剩下對話" + sentences.Count);


        bool hasEvent = line.onDialogueEvent.GetPersistentEventCount() > 0;

        if (hasEvent)
        {
            Debug.Log("hasEvent...");          
            isWaitingForInput = false;
            StartCoroutine(WaitForEventAndEnableInput(line));
        }
        else
        {
           
            isWaitingForInput = true;
        }
    }

    IEnumerator WaitForEventAndEnableInput(DialogueLine line)
    {
        remainingCallbacks = line.onDialogueEvent.GetPersistentEventCount();
        Debug.Log($"觸發事件數量：{remainingCallbacks}");

        if (remainingCallbacks == 0)
        {
            isWaitingForInput = true;
            yield break;
        }

        eventCompleted = false;

        line.onDialogueEvent.Invoke(); // 各事件會在執行完後呼叫 RegisterEventCallback()

        yield return new WaitUntil(() => eventCompleted);
        isWaitingForInput = true;
    }



public void EndDialogue()
    {
        Debug.Log("對話結束");
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
       
        if (currentTrigger != null)
        {
           
            currentTrigger.destroyDialogueTrigger();
            currentTrigger = null; // 清空避免重複
        }
    }

    void Update()
    {
        if (isDialogueActive && isWaitingForInput && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton3)))
        {
            DisplayNextSentence();
        }
        
    }

    public void RegisterEventCallback()
    {
        remainingCallbacks--;
        Debug.Log($"事件完成，剩下 {remainingCallbacks} 個");

        if (remainingCallbacks <= 0)
        {
            eventCompleted = true;
        }
    }
    public void ResetCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
