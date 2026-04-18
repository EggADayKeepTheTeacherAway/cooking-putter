using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopInventory : MonoBehaviour
{
    public Player player;

    public GameObject itemUIPrefab;   // prefab with Image + TMP_Text
    public Transform contentParent;   // where items will be placed (e.g. panel)

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        GenerateInventoryUI();
    }

    public void GenerateInventoryUI()
    {
        // clear old UI (important if regenerating)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // loop player items
        foreach (InventoryEntry entry in player.inventory) // assuming Dictionary<ItemData, int>
        {
            if (entry.quantity <= 0) continue;

            ItemData itemData = entry.item;
            Sprite itemicon = itemData.itemIcon;
            int amount = (int)entry.quantity;

            GameObject ui = Instantiate(itemUIPrefab, contentParent); // KEY!! it creates a UI from prefab

            Image icon = ui.GetComponentInChildren<Image>();
            TMP_Text text = ui.GetComponentInChildren<TMP_Text>();

            icon.sprite = itemicon;
            text.text = amount.ToString();
        }
    }
}