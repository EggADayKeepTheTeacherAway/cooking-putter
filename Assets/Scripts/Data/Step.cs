using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Game/Step")]
public class Step : ScriptableObject
{
    [SerializeField] private string station;
    [SerializeField] private float duration;
}
