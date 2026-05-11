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
    private readonly Dictionary<Customer, GameObject> foodOnTableObjects = new();
    private readonly List<GameObject> carriedDishObjects = new();
    private readonly List<ItemData> carriedDishItems = new();
    private Transform carriedDishCarrier;
    private GameObject serviceWindowPreview;
    private GameObject carriedFoodPreview;
    private Food carriedFood;
    private Transform carriedBy;
    
    // Invoked whenever the pending orders change (enqueue/dequeue/clear).
    public event Action OnOrdersUpdated;

    [SerializeField] private Transform serviceWindowPosition; // Where food displays for serving
    [SerializeField] private Vector3 customerPreviewOffset = new Vector3(0f, 1.3f, 0f);
    [SerializeField] private Vector3 servicePreviewOffset = Vector3.zero;
    [SerializeField] private Vector3 carriedPreviewOffset = new Vector3(0f, 0.9f, 0f);
    [SerializeField] private Vector3 foodOnTableOffset = new Vector3(0f, 0.3f, 0f);
    [SerializeField] private bool requirePickupRange = true;
    [SerializeField] private float pickupDistance = 1.4f;
    [SerializeField] private float previewScale = 1.5f;
    [SerializeField] private float foodOnTableScale = 1.6f;
    [SerializeField] private string previewSortingLayerName = "Foreground";
    [SerializeField] private int previewSortingOrder = 50;
    [Header("Bubble Background")]
    [SerializeField] private Sprite bubbleSprite;
    [SerializeField] private Color bubbleColor = new Color(1f,1f,1f,1f);
    [SerializeField] private Vector3 bubbleOffset = Vector3.zero;
    [SerializeField] private float bubbleScale = 1.0f;
    [SerializeField] private string bubbleSortingLayerName = "Foreground";
    [SerializeField] private int bubbleSortingOrderOffset = -1;
    [SerializeField] private string foodOnTableSortingLayerName = "Customer";
    [SerializeField] private int foodOnTableSortingOrder = 0;
    [SerializeField] private string dirtyDishSortingLayerName = "Customer";
    [SerializeField] private int dirtyDishSortingOrder = 1;
    [SerializeField] private float dirtyDishScale = 1.6f;
    [SerializeField] private ItemData dirtyDishItem; // Reference to Dirty Dish item (ItemData)
    [SerializeField] private Vector3 dishStackOffset = new Vector3(0f, 0.4f, 0f); // Vertical offset per stacked dish
    [SerializeField] private int maxDirtyDishesCarried = 4;

    public bool HasCarriedFood => carriedFood != null;
    public bool HasCarriedDirtyDish => carriedDishObjects.Count > 0;

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
        RefreshServiceWindowPreview();
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
        RefreshServiceWindowPreview();
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
            Debug.LogWarning($"Order not found for customer {customer.name} with food {food.FoodName}");
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
        if (carriedFood == food)
        {
            ClearCarriedPreview();
        }
        SpawnFoodOnTable(customer, food);
        RefreshServiceWindowPreview();
        OnOrdersUpdated?.Invoke();
        return true;
    }

    public bool TryPickupNextFoodForPlayer(Transform carrier, bool ignorePickupRange = false)
    {
        if (carrier == null)
        {
            Debug.LogWarning("Cannot pick up food without a valid carrier transform.");
            return false;
        }

        if (HasCarriedFood)
        {
            Debug.Log("Player is already carrying food.");
            return false;
        }

        if (!ignorePickupRange && requirePickupRange && serviceWindowPosition == null)
        {
            Debug.LogWarning("Service window position is not assigned on FoodServiceManager. Assign serviceWindowPosition to enforce pickup range.");
            return false;
        }

        if (!ignorePickupRange && requirePickupRange && !IsWithinPickupRange(carrier.position))
        {
            if (serviceWindowPosition != null)
            {
                float distance = Vector2.Distance(carrier.position, serviceWindowPosition.position);
                Debug.Log($"Move closer to the service window to pick up food. Current distance: {distance:F2}, required: {pickupDistance:F2}");
            }
            else
            {
                Debug.Log("Move closer to the service window to pick up food.");
            }
            return false;
        }

        while (pendingOrders.Count > 0)
        {
            var next = pendingOrders.Peek();

            if (next.customer == null)
            {
                Debug.LogWarning("Skipping order with no customer assigned.");
                pendingOrders.Dequeue();
                continue;
            }

            if (next.food == null)
            {
                Debug.LogWarning($"Skipping order for {next.customer.name} with no Food assigned.");
                pendingOrders.Dequeue();
                RefreshServiceWindowPreview();
                continue;
            }

            carriedFood = next.food;
            carriedBy = carrier;

            SpawnCarriedPreview(carrier, carriedFood);
            RefreshServiceWindowPreview();
            Debug.Log($"Picked up food: {carriedFood.FoodName}");
            return true;
        }

        Debug.Log("No valid pending orders to pick up.");
        return false;
    }

    public bool IsWithinPickupRange(Vector2 position)
    {
        if (serviceWindowPosition == null) return false;

        float sq = ((Vector2)serviceWindowPosition.position - position).sqrMagnitude;
        return sq <= pickupDistance * pickupDistance;
    }

    public bool TryServeCarriedFoodFromPlayer(Vector2 playerPosition, float maxDistance)
    {
        if (!HasCarriedFood)
        {
            Debug.Log("Player is not carrying food.");
            return false;
        }

        Customer targetCustomer = FindNearestMatchingCustomer(playerPosition, maxDistance, carriedFood);
        if (targetCustomer == null)
        {
            Debug.Log("No nearby customer is waiting for this food.");
            return false;
        }

        bool served = ServeOrderToCustomer(targetCustomer, carriedFood);
        return served;
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
        ClearCarriedPreview();
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
        serviceWindowPreview = CreatePreviewObject($"{food.FoodName} Service Preview", food.Icon, offset, parent, previewScale, false);
    }

    private void RefreshServiceWindowPreview()
    {
        ClearServiceWindowPreview();

        if (HasCarriedFood) return;
        if (pendingOrders.Count <= 0) return;

        var next = pendingOrders.Peek();
        SpawnServiceWindowPreview(next.food);
    }

    private void SpawnCarriedPreview(Transform carrier, Food food)
    {
        if (food == null || food.Icon == null || carrier == null) return;

        ClearCarriedPreviewObject();
        carriedFoodPreview = CreatePreviewObject($"{food.FoodName} Carried", food.Icon, carriedPreviewOffset, carrier, previewScale);
    }

    private GameObject CreatePreviewObject(string objectName, Sprite icon, Vector3 offset, Transform parent, float scale, bool createBubble = true)
    {
        return CreateFoodObjectWithSorting(objectName, icon, offset, parent, scale, previewSortingLayerName, previewSortingOrder, createBubble);
    }

    private GameObject CreateFoodObjectWithSorting(string objectName, Sprite icon, Vector3 offset, Transform parent, float scale, string sortingLayerName, int sortingOrder, bool createBubble = true)
    {
        var preview = new GameObject(objectName);
        preview.transform.SetParent(parent);
        
        if (parent != null)
        {
            preview.transform.localPosition = offset;
        }
        else
        {
            preview.transform.position = offset;
        }
        
        preview.transform.localScale = Vector3.one * scale;

        // Create optional bubble background behind the icon
        if (createBubble && bubbleSprite != null)
        {
            var bubble = new GameObject(objectName + " Bubble");
            bubble.transform.SetParent(preview.transform);
            if (parent != null)
            {
                bubble.transform.localPosition = bubbleOffset;
            }
            else
            {
                bubble.transform.position = offset + bubbleOffset;
            }
            bubble.transform.localScale = Vector3.one * scale * bubbleScale;

            var bubbleSr = bubble.AddComponent<SpriteRenderer>();
            bubbleSr.sprite = bubbleSprite;
            bubbleSr.color = bubbleColor;
            if (!string.IsNullOrWhiteSpace(bubbleSortingLayerName)) bubbleSr.sortingLayerName = bubbleSortingLayerName;
            bubbleSr.sortingOrder = sortingOrder + bubbleSortingOrderOffset;
        }

        var spriteRenderer = preview.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = icon;
        if (!string.IsNullOrWhiteSpace(sortingLayerName))
        {
            spriteRenderer.sortingLayerName = sortingLayerName;
        }
        spriteRenderer.sortingOrder = sortingOrder;

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

    private void ClearCarriedPreview()
    {
        if (carriedFoodPreview != null)
        {
            Destroy(carriedFoodPreview);
            carriedFoodPreview = null;
        }

        carriedFood = null;
        carriedBy = null;
    }

    private void ClearCarriedPreviewObject()
    {
        if (carriedFoodPreview != null)
        {
            Destroy(carriedFoodPreview);
            carriedFoodPreview = null;
        }
    }

    private Customer FindNearestMatchingCustomer(Vector2 center, float maxDistance, Food targetFood)
    {
        Customer nearest = null;
        float bestSq = maxDistance * maxDistance;

        foreach (var (customer, food) in pendingOrders)
        {
            if (customer == null || food != targetFood) continue;

            float sq = ((Vector2)customer.transform.position - center).sqrMagnitude;
            if (sq <= bestSq)
            {
                bestSq = sq;
                nearest = customer;
            }
        }

        return nearest;
    }

    private void SpawnFoodOnTable(Customer customer, Food food)
    {
        if (customer == null || food == null || food.Icon == null) return;

        Vector3 foodPosition = GetFoodOnTablePosition(customer);

        GameObject foodObject = CreateFoodObjectWithSorting($"{food.FoodName} OnTable", food.Icon, foodPosition, null, foodOnTableScale, foodOnTableSortingLayerName, foodOnTableSortingOrder);
        // remove bubble background for placed food so it doesn't show on table
        RemoveBubbleFromObject(foodObject);

        // Store reference to update later when eating is finished
        foodOnTableObjects[customer] = foodObject;
    }

    public void ChangeFoodToDirtyDish(Customer customer)
    {
        if (customer == null) return;
        
        if (!foodOnTableObjects.TryGetValue(customer, out var foodObject) || foodObject == null)
        {
            Debug.LogWarning($"ChangeFoodToDirtyDish: no spawned food object found for customer {customer?.name}");
            return;
        }

        // ensure bubble background removed when converting to dirty dish
        RemoveBubbleFromObject(foodObject);

        if (dirtyDishItem == null || dirtyDishItem.itemIcon == null)
        {
            Debug.LogWarning("DirtyDish item not assigned or has no icon. Falling back to gray tint.");
        }

        var spriteRenderer = foodObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (dirtyDishItem != null && dirtyDishItem.itemIcon != null)
            {
                spriteRenderer.sprite = dirtyDishItem.itemIcon;
                Debug.Log($"Changed {customer.name}'s food to dirty dish (sprite swap)");
            }
            else
            {
                // Fallback: tint the existing sprite to indicate dirty dish
                spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                Debug.Log($"Changed {customer.name}'s food to dirty dish (tinted fallback)");
            }

            if (!string.IsNullOrWhiteSpace(dirtyDishSortingLayerName))
            {
                spriteRenderer.sortingLayerName = dirtyDishSortingLayerName;
            }

            spriteRenderer.sortingOrder = dirtyDishSortingOrder;
            foodObject.transform.localScale = Vector3.one * dirtyDishScale;
            // mark as dirty dish for interaction
            var dd = foodObject.AddComponent<DirtyDish>();
            dd.owner = customer;
            dd.item = dirtyDishItem;
            // ensure there's a collider for clicks
            if (foodObject.GetComponent<Collider2D>() == null)
            {
                var col = foodObject.AddComponent<CircleCollider2D>();
                col.isTrigger = false;
            }
        }
    }

    private void RemoveBubbleFromObject(GameObject obj)
    {
        if (obj == null) return;
        // If no bubble sprite configured, nothing to remove
        if (bubbleSprite == null) return;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (sr.sprite == bubbleSprite || child.name.EndsWith(" Bubble"))
                {
                    Destroy(child.gameObject);
                    return;
                }
            }
            else if (child.name.EndsWith(" Bubble"))
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    // Player picks up a dirty dish GameObject
    public bool PickupDirtyDish(GameObject dishObject, Transform carrier)
    {
        if (dishObject == null || carrier == null) return false;
        var dd = dishObject.GetComponent<DirtyDish>();
        if (dd == null) return false;

        if (carriedDishObjects.Count >= maxDirtyDishesCarried)
        {
            Debug.Log($"Cannot pick up more dirty dishes. Max carried dishes is {maxDirtyDishesCarried}.");
            return false;
        }

        // parent to carrier and position with stack offset
        dishObject.transform.SetParent(carrier);
        Vector3 stackPosition = carriedPreviewOffset + dishStackOffset * carriedDishObjects.Count;
        dishObject.transform.localPosition = stackPosition;
        dishObject.transform.localScale = Vector3.one * dirtyDishScale;

        foreach (var col in dishObject.GetComponentsInChildren<Collider2D>(true))
        {
            col.enabled = false;
        }

        carriedDishObjects.Add(dishObject);
        carriedDishItems.Add(dd.item);
        carriedDishCarrier = carrier;

        // remove mapping from table so it won't be considered on-table anymore
        if (dd.owner != null)
        {
            foodOnTableObjects.Remove(dd.owner);
            dd.owner = null;
        }

        Debug.Log($"Picked up dirty dish: {dd.item?.itemName}. Carrying {carriedDishObjects.Count} dishes.");
        return true;
    }

    // Try to drop all carried dirty dishes into sink
    public bool TryDropCarriedDishAtSink(SinkBehaviour sink)
    {
        if (sink == null) return false;
        if (carriedDishObjects.Count == 0) return false;

        // inform sink how many dishes we're dropping
        sink.Fill();

        // destroy all carried objects and clear state
        foreach (var dish in carriedDishObjects)
        {
            Destroy(dish);
        }
        carriedDishObjects.Clear();
        carriedDishItems.Clear();
        carriedDishCarrier = null;

        Debug.Log("Dropped all dirty dishes into sink");
        return true;
    }

    private Vector3 GetFoodOnTablePosition(Customer customer)
    {
        if (customer == null)
        {
            return Vector3.zero;
        }

        if (customer.seat == null)
        {
            return customer.transform != null ? customer.transform.position : Vector3.zero;
        }

        Vector3 seatPosition = customer.seat.GetSeatPos();
        Vector3 tableOffset = foodOnTableOffset;

        if (customer.seat.seatType == SeatType.Top)
        {
            tableOffset.y = -Mathf.Abs(tableOffset.y);
        }
        else if (customer.seat.seatType == SeatType.Bottom)
        {
            tableOffset.y = Mathf.Abs(tableOffset.y);
        }

        return seatPosition + tableOffset;
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

    public void RemoveFoodFromTable(Customer customer)
    {
        if (customer == null) return;

        if (foodOnTableObjects.TryGetValue(customer, out var foodObject) && foodObject != null)
        {
            Destroy(foodObject);
        }

        foodOnTableObjects.Remove(customer);
    }
}
