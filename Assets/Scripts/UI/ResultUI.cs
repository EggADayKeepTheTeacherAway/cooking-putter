using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public static ResultUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI revenueText;

    private System.Action onContinue;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    public void Show(System.Action continueAction)
    {
        panel.SetActive(true);

        dayText.text =
            "Day " + PlayerDataManager.Instance.currentDay;

        revenueText.text =
            "Today's Revenue: "
            + PlayerDataManager.Instance.todayRevenue;

        onContinue = continueAction;
    }

    public void Continue()
    {
        panel.SetActive(false);

        onContinue?.Invoke();
    }
}