using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditText : MonoBehaviour
{
    [SerializeField] private float rollSpeed = 4.5f;
    [SerializeField] private Transform checkTransform;

    private RectTransform rectTransform;
    private RectTransform checkRect;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        checkRect = checkTransform.GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * rollSpeed * Time.deltaTime;

        // When the bottom child scrolls above the center (y > 0 = above canvas center)
        if (checkRect.position.y >= Screen.height)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}