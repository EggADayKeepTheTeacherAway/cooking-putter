using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject fridgePanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer fridgeSr;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;

    private bool isInventoryOpen = false;
    private ShopInventory shopInventory;

    private void Start()
    {
        if (fridgePanel != null)
            fridgePanel.SetActive(false);
        
        fridgeSr.sprite = closeSprite;
        shopInventory = gameObject.AddComponent<ShopInventory>();
        shopInventory.itemUIPrefab = itemUIPrefab;
        shopInventory.contentParent = contentParent;
    }

    public void Interact()
    {
        if (!isInventoryOpen)
            OpenInventory();
        else
            CloseInventory();
    }

    public void OpenInventory()
    {
        if (player.carriedFood != null)
        {
            Debug.LogWarning("Already carry food");
            return;
        }

        if (shopInventory != null)
            shopInventory.GenerateInventoryUI();

        if (fridgePanel != null)
            fridgePanel.SetActive(true);
        
        isInventoryOpen = true;
        if (fridgeSr != null)
        {
            if (openSprite != null) fridgeSr.sprite = openSprite;
        }
        Debug.Log("Fridge opened");
    }

    public void CloseInventory()
    {
        if (fridgePanel != null)
            fridgePanel.SetActive(false);

        isInventoryOpen = false;
        if (fridgeSr != null)
        {
            if (closeSprite != null) fridgeSr.sprite = closeSprite;;
        }
        Debug.Log("Fridge closed");
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            CloseInventory();
        }
    }
}
