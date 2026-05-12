using UnityEngine;
using UnityEngine.UI;

// Simple UI manager to display pending food orders
public class FoodQueueUI : MonoBehaviour
{
    [SerializeField] private Transform contentContainer; // parent for order items
    [SerializeField] private GameObject orderItemPrefab; // prefab with OrderItemUI component

    private void OnEnable()
    {
        if (FoodServiceManager.Instance != null)
            FoodServiceManager.Instance.OnOrdersUpdated += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDisable()
    {
        if (FoodServiceManager.Instance != null)
            FoodServiceManager.Instance.OnOrdersUpdated -= UpdateDisplay;
    }

    public void UpdateDisplay()
    {
        if (contentContainer == null || orderItemPrefab == null) return;

        // Clear existing
        for (int i = contentContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(contentContainer.GetChild(i).gameObject);
        }

        // Populate with pending orders
        var pending = FoodServiceManager.Instance.GetPendingOrders();
        foreach (var (customer, food) in pending)
        {
            var go = Instantiate(orderItemPrefab, contentContainer);
            var ui = go.GetComponent<OrderItemUI>();
            if (ui != null)
                ui.Set(customer, food);
        }
    }
}
