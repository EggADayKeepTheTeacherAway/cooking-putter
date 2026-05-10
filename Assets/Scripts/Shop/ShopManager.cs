using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Player player;

    public List<ShopItem> shopItems = new List<ShopItem>();

    public ShopInventory shopInventory;

    void Awake()
    {
        player = FindFirstObjectByType<Player>();
        shopItems = new List<ShopItem>(Resources.LoadAll<ShopItem>("ShopItems"));
    }

    public bool BuyItem(ShopItem item)
    {
        // Checks if player has enough money, then adds item to inventory and deducts money.
        
        if (!PlayerDataManager.Instance.TrySpendMoney(item.price))
        {
            Debug.Log("Not enough money!");
            return false;
        }

        ItemData baseItem = item.itemData;
        PlayerDataManager.Instance.AddItem(baseItem, 1);

        // After buying successfully
        shopInventory.GenerateInventoryUI();

        return true;
    }
}