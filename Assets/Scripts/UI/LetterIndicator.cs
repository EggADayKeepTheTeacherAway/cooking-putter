using UnityEngine;

public class LetterIndicator : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float moveAmount = 0.1f;
    [SerializeField] private float moveSpeed = 1f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveAmount;

        transform.position = startPos + Vector3.up * offset;
    }
}