using UnityEngine;

[CreateAssetMenu(menuName = "Game/Food")]
public class Food : ScriptableObject
{
    [SerializeField] private string foodName;
    [SerializeField] private Sprite sprite;

}
