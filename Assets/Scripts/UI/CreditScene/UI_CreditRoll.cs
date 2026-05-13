using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_CreditRoll : MonoBehaviour
{
    [SerializeField] private float rollSpeed = 4.5f;
    [SerializeField] private GameObject jumpScare;

    [SerializeField] private GameObject playerChar;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform checkRect;

    private float initialRollSpeed;

    private void Awake()
    {
        initialRollSpeed = rollSpeed;
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

    public void IncreaseRollSpeed() => rollSpeed *= 3f;

    public void ResetRollSpeed() => rollSpeed = initialRollSpeed;
}