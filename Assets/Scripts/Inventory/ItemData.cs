using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

}
