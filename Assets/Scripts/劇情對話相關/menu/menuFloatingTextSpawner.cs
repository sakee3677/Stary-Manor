using UnityEngine;

public class menuFloatingTextSpawner : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    public void ShowFloatingText(string message)
    {
        GameObject obj = Instantiate(floatingTextPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        obj.transform.SetParent(transform); // 可選，讓它跟隨角色
        obj.GetComponent<MenuFloatingText>().StartTyping(message);
    }
}
