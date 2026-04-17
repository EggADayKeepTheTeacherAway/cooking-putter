using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text itemName;
    public TMP_Text priceText;
    public Button button;
    public Image icon;

    public GameObject coinPrefab;

    private bool isHovering = false;
    private Vector2 startPos;

    public void Setup(ShopItem item, Shop shop)
    {
        icon.sprite = item.itemData.itemIcon;
        itemName.text = item.itemData.itemName;
        priceText.text = item.price.ToString();

        button.onClick.AddListener(() => 
        {
            shop.BuyItem(item);
            SpawnCoin();
        });
    }   

    void SpawnCoin() 
    {
    Canvas canvas = GetComponentInParent<Canvas>();
    // Get the ROOT canvas in case this is a nested canvas

    GameObject coin = Instantiate(coinPrefab, canvas.transform);
    coin.transform.SetAsLastSibling(); // Now truly on top of everything
    coin.transform.position = icon.transform.position;
    }

    void Start()
    {
        startPos = icon.rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (isHovering)
        {
            float offset = Mathf.Sin(Time.time * 4f) * 4f;
            icon.rectTransform.anchoredPosition = startPos + new Vector2(0, offset);
        }
        else
        {
            icon.rectTransform.anchoredPosition = startPos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}