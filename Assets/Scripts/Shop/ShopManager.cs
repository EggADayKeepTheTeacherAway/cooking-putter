using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Player player;

    public List<ShopItem> shopItems = new List<ShopItem>();

    void Awake()
    {
        shopItems = new List<ShopItem>(Resources.LoadAll<ShopItem>("ShopItems"));
    }

    public void BuyItem(ShopItem item)
    {
        /*Checks if player has enough money, then adds item to inventory and deducts money.
        
        if (player.money < item.price)
        {
            Debug.Log("Not enough money!");
            return;
        }

        ItemData baseItem = item.itemData;
        player.money -= item.price;
        player.AddItem(baseItem, 1);
        */
        return;
    }


}