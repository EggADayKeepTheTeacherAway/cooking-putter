using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditText : MonoBehaviour
{
    [SerializeField] private float rollSpeed = 4.5f;
    [SerializeField] private Transform checkTransform;
    [SerializeField] private GameObject jumpScare;

    [SerializeField] private GameObject playerChar;

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

        if (checkRect.position.y >= Screen.height)
        {
            playerChar.SetActive(false);
            jumpScare.SetActive(true);
        }
    }
}