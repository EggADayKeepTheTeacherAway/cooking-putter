using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{   
    public TMP_Text moneyText;     // drag your TextMeshPro UI here

    void Update()
    {
        if (moneyText == null)
        {
            Debug.LogError("Money: moneyText not assigned in Inspector!");
            return;
        }

        if (PlayerDataManager.Instance != null)
        {
            moneyText.text = PlayerDataManager.Instance.money.ToString();
        }
    }
}