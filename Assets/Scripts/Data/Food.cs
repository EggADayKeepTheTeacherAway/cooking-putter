using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Food")]
public class Food : ScriptableObject
{
    [SerializeField] private string foodName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private List<ItemData> ingredients;
    [SerializeField] private List<Step> steps;

    public string FoodName => foodName;
    public Sprite Icon => sprite;
    public List<ItemData> Ingredients => ingredients;
    public List<Step> Steps => steps;

}
