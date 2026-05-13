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
        return FoodServiceManager.GetOrCreateInstance().TryServeCarriedFood(player, servingDistance);
    }
}
