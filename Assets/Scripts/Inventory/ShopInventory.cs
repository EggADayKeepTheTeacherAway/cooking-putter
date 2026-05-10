using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopInventory : MonoBehaviour
{
    public GameObject itemUIPrefab;   // prefab with Image + TMP_Text
    public Transform contentParent;   // where items will be placed (e.g. panel)

    void Start()
    {
        GenerateInventoryUI();
    }

    public void GenerateInventoryUI()
    {
        // Safety checks
        if (contentParent == null)
        {
            Debug.LogError("ShopInventory: contentParent not assigned in Inspector!");
            return;
        }

        if (itemUIPrefab == null)
        {
            Debug.LogError("ShopInventory: itemUIPrefab not assigned in Inspector!");
            return;
        }

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("ShopInventory: PlayerDataManager not found in scene!");
            return;
        }

        // clear old UI (important if regenerating)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // loop through PlayerDataManager inventory instead of Player
        foreach (InventoryEntry entry in PlayerDataManager.Instance.inventory)
        {
            if (entry.quantity <= 0) continue;

            ItemData itemData = entry.item;
            if (itemData == null)
            {
                Debug.LogWarning("ShopInventory: ItemData is null, skipping entry");
                continue;
            }

            Sprite itemicon = itemData.itemIcon;
            int amount = (int)entry.quantity;

            GameObject ui = Instantiate(itemUIPrefab, contentParent);

            Image icon = ui.GetComponentInChildren<Image>();
            TMP_Text text = ui.GetComponentInChildren<TMP_Text>();

            if (icon != null)
                icon.sprite = itemicon;

            if (text != null)
                text.text = amount.ToString();
        }
    }
}