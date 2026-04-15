using UnityEngine;

public class Shop : MonoBehaviour
{
    public Player player;

    public List<ShopItem> shopItems = new List<ShopItem>();

    public void BuyItem(ShopItem item)
    {
        if (player.money < item.price)
        {
            Debug.Log("Not enough money!");
            return;
        }

        player.money -= item.price;
        player.AddItem(item, 1);
    }
}