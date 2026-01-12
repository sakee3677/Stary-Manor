using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    private IUsable currentItem; // 當前選中的物品

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame) // 假設 e 鍵是用來執行使用物品
        {
            UseCurrentItem();
        }
    }

    // 設置當前選中的物品
    public void SetCurrentItem(IUsable item)
    {
        currentItem = item;
    }

    // 使用當前選中的物品
    private void UseCurrentItem()
    {
        if (currentItem != null)
        {
            currentItem.UseObject(); // 調用物品的 Use 方法
        }
        else
        {
            Debug.Log("沒有可使用的物品");
        }
    }
}
