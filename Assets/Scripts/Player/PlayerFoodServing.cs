using UnityEngine;

/// <summary>
/// Handles serving cooked food to customers
/// Attach this script to the Player GameObject
/// </summary>
public class PlayerFoodServing : MonoBehaviour
{
    [SerializeField] private float servingDistance = 2f;
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // Check if player is carrying cooked food and presses interact
        if (player.carriedFood != null && player.carriedFood.IsDone() && player.input.Player.Interact.WasPressedThisFrame())
        {
            TryServeFood();
        }
    }

    private bool TryServeFood()
    {
        // Find nearest customer within serving distance
        Customer[] allCustomers = FindObjectsOfType<Customer>();
        
        Customer nearestCustomer = null;
        float nearestDistance = servingDistance;
        
        foreach (Customer customer in allCustomers)
        {
            if (customer == null) continue;
            
            float distance = Vector2.Distance(transform.position, customer.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCustomer = customer;
            }
        }
        
        if (nearestCustomer == null)
        {
            Debug.Log("No nearby customer to serve food to. Move closer to a customer.");
            return false;
        }
        
        // Check if this customer has an order for this food
        var pendingOrders = FoodServiceManager.GetOrCreateInstance().GetPendingOrders();
        bool hasMatchingOrder = false;
        Food orderedFood = null;
        
        foreach (var (customer, food) in pendingOrders)
        {
            if (customer == nearestCustomer && player.carriedFood != null && food.FoodName == player.carriedFood.GetFoodName())
            {
                hasMatchingOrder = true;
                orderedFood = food;
                break;
            }
        }
        
        if (!hasMatchingOrder)
        {
            Debug.Log($"Customer {nearestCustomer.name} didn't order {player.carriedFood.GetFoodName()}.");
            return false;
        }
        
        // Serve the food using FoodServiceManager
        FoodServiceManager.GetOrCreateInstance().ServeOrderToCustomer(nearestCustomer, orderedFood);
        
        // Clear carried food
        player.ClearCarriedFood();
        
        Debug.Log($"Served {orderedFood.FoodName} to {nearestCustomer.name}");
        return true;
    }
}
