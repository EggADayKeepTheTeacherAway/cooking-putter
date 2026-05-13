using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Game/Step")]
public class Step : ScriptableObject
{
    [SerializeField] private string station;
    [SerializeField] private AudioClip audio;
    [SerializeField] private float duration;

    public string Station => station;

    public AudioClip Audio => audio;
    public float Duration => duration;
}
