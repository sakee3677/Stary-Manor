using UnityEngine;
using UnityEngine.UI;
using IsoTools;
using IsoTools.Physics;
using UnityEngine.UIElements;

public class RoomDoor : MonoBehaviour
{
    private IsoCollisionListener collisionListener;
    private IsoTriggerListener IsoTriggerListener;
    private GameObject openedDoorObject;
    public GameObject AnotherRoomDoor;
    public Camera mainCamera;


    public Vector3 positionOffset = new Vector3(2f, 0f, 0f); // 自定義的偏移量，可以根據需要調整
    public Vector3 cameraPosition = new Vector3(2f, 0f, 0f); // 自定義的偏移量，可以根據需要調整


    private void Awake()
    {
        collisionListener = GetComponent<IsoCollisionListener>() ?? gameObject.AddComponent<IsoCollisionListener>();
        IsoTriggerListener = GetComponent<IsoTriggerListener>() ?? gameObject.AddComponent<IsoTriggerListener>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnIsoCollisionEnter(IsoCollision isoCollision)
    {

        if (isoCollision.gameObject.CompareTag("Player") || isoCollision.gameObject.CompareTag("PlayerParasite"))
        {
            openedDoorObject = isoCollision.gameObject;
            IsoObject openedDoorIsoObject = openedDoorObject.GetComponent<IsoObject>();
            IsoObject AnotherRoomDoorIsoObject = AnotherRoomDoor.GetComponent<IsoObject>();
            openedDoorIsoObject.position = AnotherRoomDoorIsoObject.position + positionOffset;
            mainCamera.transform.position = cameraPosition;
        }
    }
  
}
