using UnityEngine;
using UnityEngine.UI;
using IsoTools;
using IsoTools.Physics;
using UnityEngine.SceneManagement;


public class spike : MonoBehaviour
{
    public string nextSceneName;
    public GameObject player;
    public GameObject spawn;
    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
   

    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }

    // Start is called before the first frame update
    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {
        if (isoCollision.gameObject.CompareTag("Player"))
        {
            player = isoCollision.gameObject;
            IsoObject IsoObjectplayer = player.GetComponent<IsoObject>();
            IsoObject spawnIsoObject = spawn.GetComponent<IsoObject>();
           // SceneManager.LoadScene(nextSceneName);
            IsoObjectplayer.position = spawnIsoObject.position;
        }
    }

    void OnIsoTriggerExit(IsoCollider isoCollider)
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
