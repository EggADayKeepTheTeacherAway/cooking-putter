using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject fridgePanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private Player player;

    private bool isInventoryOpen = false;
    private ShopInventory shopInventory;

    private void Start()
    {
        if (fridgePanel != null)
            fridgePanel.SetActive(false);
        
        shopInventory = gameObject.AddComponent<ShopInventory>();
        shopInventory.itemUIPrefab = itemUIPrefab;
        shopInventory.contentParent = contentParent;
    }

    public void Interact()
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (!isInventoryOpen)
            OpenInventory();
        else
            CloseInventory();
    }

    public void OpenInventory()
    {
        if (fridgePanel != null)
            fridgePanel.SetActive(true);

        isInventoryOpen = true;
        Debug.Log("Inventory opened");
    }

    public void CloseInventory()
    {
        if (fridgePanel != null)
            fridgePanel.SetActive(false);

        isInventoryOpen = false;
        Debug.Log("Inventory closed");
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CloseInventory();
        }
    }
}
