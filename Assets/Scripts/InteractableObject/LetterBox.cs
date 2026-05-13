using UnityEngine;

public class LetterBox : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject letterCanvas;

    private bool isShowingIndicator = false;

    private void Update()
    {
        if (!isShowingIndicator && PlayerDataManager.Instance.currentDay == PlayerDataManager.Instance.foodCriticDay)
        {
            indicator.SetActive(true);
            isShowingIndicator = true;
        }
    }

    public void Interact()
    {
        if (indicator.activeSelf)
        {
            letterCanvas.SetActive(true);
            indicator.SetActive(false);
        }
 
    }
}
