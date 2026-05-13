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
    private Customer carriedOrderCustomer;
    private FoodOrderVisuals visuals;
    private DirtyDishService dirtyDishes;
    
    // Invoked whenever the pending orders change (enqueue/dequeue/clear).
    public event Action OnOrdersUpdated;

    public bool HasCarriedDirtyDish => dirtyDishes != null && dirtyDishes.HasCarriedDirtyDish;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        visuals = GetComponent<FoodOrderVisuals>();
        if (visuals == null)
        {
            visuals = gameObject.AddComponent<FoodOrderVisuals>();
        }

        dirtyDishes = GetComponent<DirtyDishService>();
        if (dirtyDishes == null)
        {
            dirtyDishes = gameObject.AddComponent<DirtyDishService>();
        }
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
        visuals?.SpawnCustomerOrderPreview(customer, food);
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
        visuals?.RemoveCustomerOrderPreview(customer);
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
        visuals?.RemoveCustomerOrderPreview(customer);
        visuals?.SpawnFoodOnTable(customer, food);
        OnOrdersUpdated?.Invoke();
        return true;
    }

    public bool TryPickupNextOrderForPlayer(Player player)
    {
        if (player == null)
        {
            Debug.LogWarning("Cannot pick up food without a valid player.");
            return false;
        }

        if (player.carriedFood != null)
        {
            Debug.Log("Player is already carrying food.");
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
                continue;
            }

            carriedOrderCustomer = next.customer;

            if (player.PickUpServiceFood(next.food))
            {
                Debug.Log($"Picked up food: {next.food.FoodName} for {carriedOrderCustomer.name}");
                return true;
            }

            carriedOrderCustomer = null;
            return false;
        }

        Debug.Log("No valid pending orders to pick up.");
        return false;
    }

    public bool TryServeCarriedFood(Player player, float maxDistance)
    {
        if (player == null || player.carriedFood == null)
        {
            Debug.Log("Player is not carrying food.");
            return false;
        }

        if (!player.carriedFood.IsDone())
        {
            Debug.Log("Carried food is not ready to serve.");
            return false;
        }

        Vector2 playerPosition = player.transform.position;
        Food carriedFood = player.carriedFood.SourceFood;
        string carriedFoodName = player.carriedFood.GetFoodName();

        bool hasSpecificCarriedOrder = TryGetCarriedOrderCustomerInRange(playerPosition, maxDistance, carriedFood, carriedFoodName, out Customer targetCustomer);
        Food orderedFood = null;

        if (targetCustomer != null)
        {
            orderedFood = FindPendingFoodForCustomer(targetCustomer, carriedFood, carriedFoodName);
        }
        else if (!hasSpecificCarriedOrder)
        {
            (targetCustomer, orderedFood) = FindNearestMatchingOrder(playerPosition, maxDistance, carriedFood, carriedFoodName);
        }

        if (targetCustomer == null || orderedFood == null)
        {
            Debug.Log("No nearby customer is waiting for this food.");
            return false;
        }

        if (!ServeOrderToCustomer(targetCustomer, orderedFood))
        {
            return false;
        }

        player.DiscardFood();
        carriedOrderCustomer = null;
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
        carriedOrderCustomer = null;
        visuals?.ClearAllPreviews();
        OnOrdersUpdated?.Invoke();
    }

    // Called when an order is added to the queue
    private void OnOrderQueued(Customer customer, Food food)
    {
        Debug.Log($"Order queued: {customer.name} ordered {food.FoodName}. Pending: {pendingOrders.Count}");
    }

    // Called when an order is successfully served
    private void OnOrderServed(Customer customer, Food food)
    {
        Debug.Log($"Order served: {customer.name} received {food.FoodName}");
    }

    private (Customer customer, Food food) FindNearestMatchingOrder(Vector2 center, float maxDistance, Food targetFood, string targetFoodName)
    {
        Customer nearest = null;
        Food nearestFood = null;
        float bestSq = maxDistance * maxDistance;

        foreach (var (customer, food) in pendingOrders)
        {
            if (customer == null || !IsMatchingFood(food, targetFood, targetFoodName)) continue;

            float sq = ((Vector2)customer.transform.position - center).sqrMagnitude;
            if (sq <= bestSq)
            {
                bestSq = sq;
                nearest = customer;
                nearestFood = food;
            }
        }

        return (nearest, nearestFood);
    }

    private bool TryGetCarriedOrderCustomerInRange(Vector2 center, float maxDistance, Food targetFood, string targetFoodName, out Customer targetCustomer)
    {
        targetCustomer = null;
        if (carriedOrderCustomer == null) return false;

        foreach (var (customer, food) in pendingOrders)
        {
            if (customer != carriedOrderCustomer || !IsMatchingFood(food, targetFood, targetFoodName)) continue;

            float sq = ((Vector2)customer.transform.position - center).sqrMagnitude;
            if (sq <= maxDistance * maxDistance)
            {
                targetCustomer = customer;
                return true;
            }

            Debug.Log($"Move closer to {customer.name} to serve {targetFoodName}.");
            return true;
        }

        carriedOrderCustomer = null;
        return false;
    }

    private Food FindPendingFoodForCustomer(Customer targetCustomer, Food targetFood, string targetFoodName)
    {
        foreach (var (customer, food) in pendingOrders)
        {
            if (customer == targetCustomer && IsMatchingFood(food, targetFood, targetFoodName))
            {
                return food;
            }
        }

        return null;
    }

    private bool IsMatchingFood(Food orderFood, Food carriedFood, string carriedFoodName)
    {
        if (orderFood == null) return false;
        if (carriedFood != null && orderFood == carriedFood) return true;

        return NormalizeFoodName(orderFood.FoodName) == NormalizeFoodName(carriedFoodName);
    }

    private string NormalizeFoodName(string foodName)
    {
        return string.IsNullOrWhiteSpace(foodName)
            ? string.Empty
            : foodName.Replace(" ", string.Empty).Trim().ToLowerInvariant();
    }

    public void ChangeFoodToDirtyDish(Customer customer)
    {
        dirtyDishes?.ChangeFoodToDirtyDish(customer);
    }

    public bool PickupDirtyDish(GameObject dishObject, Transform carrier)
    {
        return dirtyDishes != null && dirtyDishes.PickupDirtyDish(dishObject, carrier);
    }

    public bool TryDropCarriedDishAtSink(Sink sink)
    {
        return dirtyDishes != null && dirtyDishes.TryDropCarriedDishAtSink(sink);
    }

    public void RemoveFoodFromTable(Customer customer)
    {
        visuals?.RemoveFoodFromTable(customer);
    }

    public void PickupCleanedDishFromWashingStation(Transform station)
    {
        dirtyDishes?.PickupCleanedDishFromWashingStation(station);
    }
}
