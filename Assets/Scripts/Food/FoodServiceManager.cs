using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the food serving system.
/// - Receives orders from customers
/// - Stores pending orders in a queue
/// - Allows player to serve food to waiting customers
/// </summary>
public class FoodServiceManager : MonoBehaviour
{
    public static FoodServiceManager Instance { get; private set; }

    private Queue<(Customer customer, Food food)> pendingOrders = new();
    private readonly Dictionary<Customer, GameObject> customerOrderPreviews = new();
    private GameObject serviceWindowPreview;
    
    // Invoked whenever the pending orders change (enqueue/dequeue/clear).
    public event Action OnOrdersUpdated;

    [SerializeField] private Transform serviceWindowPosition; // Where food displays for serving
    [SerializeField] private Vector3 customerPreviewOffset = new Vector3(0f, 1.3f, 0f);
    [SerializeField] private Vector3 servicePreviewOffset = Vector3.zero;
    [SerializeField] private float previewScale = 0.6f;
    [SerializeField] private int previewSortingOrder = 50;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static FoodServiceManager GetOrCreateInstance()
    {
        if (Instance != null) return Instance;

        var existing = FindFirstObjectByType<FoodServiceManager>();
        if (existing != null)
        {
            Instance = existing;
            return Instance;
        }

        var runtimeObject = new GameObject(nameof(FoodServiceManager));
        Instance = runtimeObject.AddComponent<FoodServiceManager>();
        return Instance;
    }

    /// <summary>
    /// Called when a customer orders food
    /// Adds the order to the pending queue
    /// </summary>
    public void OrderFood(Customer customer, Food food)
    {
        if (customer == null || food == null)
        {
            Debug.LogWarning("OrderFood called with a null customer or food.");
            return;
        }

        pendingOrders.Enqueue((customer, food));
        OnOrderQueued(customer, food);
        SpawnCustomerOrderPreview(customer, food);
        SpawnServiceWindowPreview(food);
        OnOrdersUpdated?.Invoke();
    }

    /// <summary>
    /// Serves the next pending order to the customer
    /// Returns true if successfully served, false if no pending orders
    /// </summary>
    public bool ServeNextOrder()
    {
        if (pendingOrders.Count <= 0)
        {
            Debug.LogWarning("No pending orders to serve!");
            return false;
        }

        var (customer, food) = pendingOrders.Dequeue();
        customer.OnRecievedFood?.Invoke(food);
        OnOrderServed(customer, food);
        RemoveCustomerOrderPreview(customer);
        ClearServiceWindowPreview();
        OnOrdersUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Serves a specific order to a customer (for direct serving)
    /// </summary>
    public bool ServeOrderToCustomer(Customer customer, Food food)
    {
        // Find and remove the order from queue
        var ordersArray = pendingOrders.ToArray();
        bool found = false;

        foreach (var (queuedCustomer, queuedFood) in ordersArray)
        {
            if (queuedCustomer == customer && queuedFood == food)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"Order not found for customer {customer.name} with food {food.name}");
            return false;
        }

        // Rebuild queue without this order
        var tempQueue = new Queue<(Customer, Food)>();
        while (pendingOrders.Count > 0)
        {
            var order = pendingOrders.Dequeue();
            if (order.customer != customer || order.food != food)
            {
                tempQueue.Enqueue(order);
            }
        }
        pendingOrders = tempQueue;

        // Serve the food
        customer.OnRecievedFood?.Invoke(food);
        OnOrderServed(customer, food);
        RemoveCustomerOrderPreview(customer);
        ClearServiceWindowPreview();
        OnOrdersUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Returns the next pending order without removing it from queue
    /// </summary>
    public (Customer customer, Food food)? PeekNextOrder()
    {
        if (pendingOrders.Count <= 0) return null;
        return pendingOrders.Peek();
    }

    /// <summary>
    /// Returns all pending orders
    /// </summary>
    public (Customer customer, Food food)[] GetPendingOrders()
    {
        return pendingOrders.ToArray();
    }

    /// <summary>
    /// Returns number of pending orders
    /// </summary>
    public int GetPendingOrderCount()
    {
        return pendingOrders.Count;
    }

    /// <summary>
    /// Clears all pending orders (use with caution!)
    /// </summary>
    public void ClearAllOrders()
    {
        pendingOrders.Clear();
        ClearAllPreviews();
        OnOrdersUpdated?.Invoke();
    }

    // Called when an order is added to the queue
    private void OnOrderQueued(Customer customer, Food food)
    {
        // You can add UI updates, sounds, or animations here
        Debug.Log($"Order queued: {customer.name} ordered {food.FoodName}. Pending: {pendingOrders.Count}");
    }

    // Called when an order is successfully served
    private void OnOrderServed(Customer customer, Food food)
    {
        // You can add UI updates, sounds, or animations here
        Debug.Log($"Order served: {customer.name} received {food.FoodName}");
    }

    private void SpawnCustomerOrderPreview(Customer customer, Food food)
    {
        if (customer == null || food == null || food.Icon == null) return;

        RemoveCustomerOrderPreview(customer);

        var preview = CreatePreviewObject($"{food.FoodName} Preview", food.Icon, customerPreviewOffset, customer.transform, previewScale);
        customerOrderPreviews[customer] = preview;
    }

    private void SpawnServiceWindowPreview(Food food)
    {
        if (food == null || food.Icon == null) return;

        ClearServiceWindowPreview();

        var parent = serviceWindowPosition != null ? serviceWindowPosition : transform;
        var offset = serviceWindowPosition != null ? servicePreviewOffset : Vector3.zero;
        serviceWindowPreview = CreatePreviewObject($"{food.FoodName} Service Preview", food.Icon, offset, parent, previewScale);
    }

    private GameObject CreatePreviewObject(string objectName, Sprite icon, Vector3 localOffset, Transform parent, float scale)
    {
        var preview = new GameObject(objectName);
        preview.transform.SetParent(parent, false);
        preview.transform.localPosition = localOffset;
        preview.transform.localScale = Vector3.one * scale;

        var spriteRenderer = preview.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = icon;
        spriteRenderer.sortingOrder = previewSortingOrder;

        return preview;
    }

    private void RemoveCustomerOrderPreview(Customer customer)
    {
        if (customer == null) return;

        if (customerOrderPreviews.TryGetValue(customer, out var preview) && preview != null)
        {
            Destroy(preview);
        }

        customerOrderPreviews.Remove(customer);
    }

    private void ClearServiceWindowPreview()
    {
        if (serviceWindowPreview != null)
        {
            Destroy(serviceWindowPreview);
            serviceWindowPreview = null;
        }
    }

    private void ClearAllPreviews()
    {
        foreach (var preview in customerOrderPreviews.Values)
        {
            if (preview != null)
            {
                Destroy(preview);
            }
        }

        customerOrderPreviews.Clear();
        ClearServiceWindowPreview();
    }
}
