using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CreditScene : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UI_CreditRoll creditRoll;

    public void OnPointerDown(PointerEventData eventData)
    {
        creditRoll.IncreaseRollSpeed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        creditRoll.ResetRollSpeed();
    }
}