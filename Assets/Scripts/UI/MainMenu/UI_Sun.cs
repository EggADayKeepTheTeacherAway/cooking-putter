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

        float rotation =
            Mathf.Sin(time * rotationSpeed) * rotationAmount;

        float offsetX =
            (Mathf.PerlinNoise(time * moveSpeed, 0f) - 0.5f) * moveAmount;

        float offsetY =
            (Mathf.PerlinNoise(0f, time * moveSpeed) - 0.5f) * moveAmount;

        sun.rotation = Quaternion.Euler(0f, 0f, rotation);

        sun.anchoredPosition =
            startPos + new Vector2(offsetX, offsetY);
    }
}