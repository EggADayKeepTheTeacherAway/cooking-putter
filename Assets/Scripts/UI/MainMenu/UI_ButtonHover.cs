using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UI_ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float normalSize = 12f;
    [SerializeField] private float hoverSize = 12.5f;

    [SerializeField] private string originalColorCode = "#F5E7D3";
    [SerializeField] private string hoverColorCode = "#F7E4C9";

    private Color originalColor;
    private Color hoverColor;


    private void Awake()
    {
        ColorUtility.TryParseHtmlString(originalColorCode, out originalColor);
        ColorUtility.TryParseHtmlString(hoverColorCode, out hoverColor);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.fontSize = hoverSize;
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.fontSize = normalSize;
        buttonText.color = originalColor;
    }

  
}