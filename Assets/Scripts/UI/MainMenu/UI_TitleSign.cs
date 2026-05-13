using UnityEngine;

public class UI_TitleSign : MonoBehaviour
{
    [SerializeField] private RectTransform titleSign;

    [Header("Slide")]
    [SerializeField] private float startY = 615F;
    [SerializeField] private float endY = 17f;
    [SerializeField] private float slideSpeed = 5f;

    private void Start()
    {
        titleSign.anchoredPosition = new Vector2(
            titleSign.anchoredPosition.x,
            startY
        );
    }

    private void Update()
    {
        Vector2 targetPos = new Vector2(
            titleSign.anchoredPosition.x,
            endY
        );

        titleSign.anchoredPosition = Vector2.Lerp(
            titleSign.anchoredPosition,
            targetPos,
            slideSpeed * Time.deltaTime
        );
    }
}