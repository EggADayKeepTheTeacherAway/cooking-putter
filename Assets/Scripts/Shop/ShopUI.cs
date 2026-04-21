using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public Shop shop;                 // logic
    public GameObject itemPrefab;     // ShopItemUI prefab
    public Transform contentParent;   // Content object

    void Start()
    {
        foreach (var item in shop.shopItems)
        {
            //Debug.Log(item);
            //Debug.Log(item.itemData);
            //Debug.Log(item.itemData.itemIcon);
            //Debug.Log(item.itemData.itemName);
            //Debug.Log(item.price);

            GameObject obj = Instantiate(itemPrefab, contentParent);

            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.Setup(item, shop);
        }
    }
}