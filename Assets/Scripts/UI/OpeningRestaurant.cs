using TMPro;
using UnityEngine;

public class OpeningRestaurantUI : MonoBehaviour
{
    public static OpeningRestaurantUI Instance;

    [SerializeField] private GameObject panel;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}