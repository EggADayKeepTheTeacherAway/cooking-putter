using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager missing");
            return;
        }

        Debug.Log("Showing Day");

        dayText.text =
            "Day " + PlayerDataManager.Instance.currentDay;
    }
}