using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{   
    public Player player;          // drag your Player here
    public TMP_Text moneyText;     // drag your TextMeshPro UI here

    void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (player != null && moneyText != null)
        {
            moneyText.text = player.money.ToString();
        }
    }
}