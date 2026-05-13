using UnityEngine;

public class UI_Sun : MonoBehaviour
{
    [SerializeField] private RectTransform sun;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 5f;
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Floating")]
    [SerializeField] private float moveAmount = 10f;
    [SerializeField] private float moveSpeed = 1f;

    private Vector2 startPos;

    private void Start()
    {
        startPos = sun.anchoredPosition;
    }

    private void Update()
    {
        float time = Time.time;

        // Smooth left/right rotation
        float rotation =
            Mathf.Sin(time * rotationSpeed) * rotationAmount;

        // Slight random floating movement
        float offsetX =
            (Mathf.PerlinNoise(time * moveSpeed, 0f) - 0.5f) * moveAmount;

        float offsetY =
            (Mathf.PerlinNoise(0f, time * moveSpeed) - 0.5f) * moveAmount;

        // Apply rotation
        sun.rotation = Quaternion.Euler(0f, 0f, rotation);

        // Apply movement
        sun.anchoredPosition =
            startPos + new Vector2(offsetX, offsetY);
    }
}