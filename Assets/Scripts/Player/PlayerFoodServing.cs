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
        if (player.carriedFood != null && player.carriedFood.IsDone() && player.input.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("Attempting to serve food");
            TryServeFood();
        }
        else
        {
            Debug.Log($"carriedFood: {player.carriedFood}, IsDone: {player.carriedFood?.IsDone()}, Interact: {player.input.Player.Interact.WasPressedThisFrame()}");
        }
    }

    private bool TryServeFood()
    {
        return FoodServiceManager.GetOrCreateInstance().TryServeCarriedFood(player, servingDistance);
    }
}
