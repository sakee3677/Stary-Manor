using System.Collections.Generic;
using UnityEngine;
using IsoTools;
using IsoTools.Physics;

public class Inventory : MonoBehaviour
{
    public List<string> items = new List<string>();

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName))
        {
            items.Add(itemName);
            Debug.Log("獲得道具：" + itemName);
        }
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            Debug.Log("使用道具：" + itemName);
        }
    }
}
