using TMPro;
using UnityEngine;
using System.Collections;

public class MoneyPopupUI : MonoBehaviour
{
    public static MoneyPopupUI Instance;

    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip cashSfx;

    private void Awake()
    {
        Instance = this;

        popupRoot.SetActive(false);
    }

    public void Show(int amount)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(amount));
    }

    private IEnumerator ShowRoutine(int amount)
    {
        popupRoot.SetActive(true);

        popupText.text = "+" + amount;

        if (cashSfx != null)
        {
            audioSource.PlayOneShot(cashSfx);
        }

        yield return new WaitForSeconds(2f);

        popupRoot.SetActive(false);
    }
}