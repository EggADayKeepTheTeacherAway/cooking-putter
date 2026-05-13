using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_JumpScare : MonoBehaviour
{
    [SerializeField] private float expandSpeed = 1f;
    [SerializeField] private float maximumSize = 1000f;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private string jumpScareSound = "jumpscare";

    private void OnEnable()
    {
        AudioManager.Instance.PlayGlobalSFX(jumpScareSound);
    }

    void Update()
    {
        rectTransform.sizeDelta += Vector2.one * expandSpeed * Time.deltaTime;

        if (rectTransform.sizeDelta.x >= maximumSize)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}