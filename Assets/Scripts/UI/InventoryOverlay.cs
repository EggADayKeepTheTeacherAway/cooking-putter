using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;      // Inventory panel
    [SerializeField] private Transform contentParent;        // Where item UIs are placed
    [SerializeField] private GameObject itemUIPrefab;        // Template for inventory items
    [SerializeField] private TextMeshProUGUI moneyDisplay;   // Money text display
    [SerializeField] private TextMeshProUGUI itemCountDisplay; // Total items count

    private bool isInventoryOpen = false;
    private ShopInventory shopInventory;
    private int lastInventoryCount = -1;
    private int lastMoneyAmount = -1;

    private void Start()
    {
        // Hide inventory by default
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        shopInventory = gameObject.AddComponent<ShopInventory>();
        shopInventory.itemUIPrefab = itemUIPrefab;
        shopInventory.contentParent = contentParent;
    }

    private void Update()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null && player.input != null)
        {
            if (player.input.Player.OpenInventory.WasPressedThisFrame())
            {
                ToggleInventory();
            }

            if (player.input.Player.CloseInventory.WasPressedThisFrame() && isInventoryOpen)
            {
                CloseInventory();
            }
        }

        if (isInventoryOpen)
        {
            UpdateMoneyDisplay();
            
            if (PlayerDataManager.Instance.inventory.Count != lastInventoryCount)
            {
                RefreshInventoryDisplay();
                lastInventoryCount = PlayerDataManager.Instance.inventory.Count;
            }
        }
    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        RefreshInventoryDisplay();
        lastInventoryCount = PlayerDataManager.Instance.inventory.Count;

        Debug.Log("Inventory opened");
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        Debug.Log("Inventory closed");
    }

    public void RefreshInventoryDisplay()
    {
        if (shopInventory != null)
            shopInventory.GenerateInventoryUI();

        // Update money display
        UpdateMoneyDisplay();

        // Update item count
        if (itemCountDisplay != null)
            itemCountDisplay.text = $"Items: {PlayerDataManager.Instance.inventory.Count}";
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyDisplay != null)
        {
            int currentMoney = PlayerDataManager.Instance.money;
            
            // Only update if money changed
            if (currentMoney != lastMoneyAmount)
            {
                moneyDisplay.text = $"Money: ${currentMoney}";
                lastMoneyAmount = currentMoney;
            }
        }
    }

    public void OnItemCollected()
    {
        if (isInventoryOpen)
        {
            RefreshInventoryDisplay();
        }
    }

    public bool IsInventoryOpen => isInventoryOpen;
}
