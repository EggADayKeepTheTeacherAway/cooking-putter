using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_JumpScare : MonoBehaviour
{
    [SerializeField] private float expandSpeed = 1f;
    [SerializeField] private float maximumSize = 1000f;

    [SerializeField] private RectTransform rectTransform;

    

    void Update()
    {
        rectTransform.sizeDelta += Vector2.one * expandSpeed * Time.deltaTime;

        if (rectTransform.sizeDelta.x >= maximumSize)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}