using UnityEngine;

// Handles player input for serving orders
public class ServeInput : MonoBehaviour
{
    [SerializeField] private float serveDistance = 1.6f;
    [SerializeField] private Transform playerTransform;
    private Player player;

    private void Awake()
    {
        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                playerTransform = transform;
            }
        }

        player = playerTransform.GetComponent<Player>();
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    // Hook this to a UI Button's OnClick()
    public void ServeButton()
    {
        HandleServeKey();
    }

    private void HandleServeKey()
    {
        var manager = FoodServiceManager.GetOrCreateInstance();

        if (player != null && player.carriedFood != null)
        {
            manager.TryServePlayerCarriedFood(player, serveDistance);
        }
        else
        {
            manager.TryPickupNextFoodForPlayer(player);
        }
    }
}
