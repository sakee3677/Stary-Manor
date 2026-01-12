using IsoTools.Physics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dogdoor : MonoBehaviour
{
    //Inspector 內可設定哪些 tag 可以觸發對話**
    private string playertriggerTags= "Player";
    private string parasitetriggerTags = "ParasiteHost";
    public GameObject dog;
    public string nextSceneName;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnIsoTriggerEnter(IsoCollider isoCollider)
    {

        if (parasitetriggerTags.Contains(isoCollider.tag))
        {
            dog.SetActive(false);
        }
        else if (playertriggerTags.Contains(isoCollider.tag))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
