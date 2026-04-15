using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int money = 100;

    public List<InventoryEntry> inventory = new List<InventoryEntry>();

    public void AddItem(ItemData item, int amount = 1)
    {
        foreach (var entry in inventory)
        {
            if (entry.item == item)
            {
                entry.quantity += amount;
                return;
            }
        }

        inventory.Add(new InventoryEntry { item = item, quantity = amount });
    }
}
