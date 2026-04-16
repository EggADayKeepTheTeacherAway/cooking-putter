using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Game/ShopItem")]
public class ShopItem : ScriptableObject
{
    public ItemData itemData;
    public int price;
}
