using UnityEngine;
using UnityEngine.InputSystem;

// Handles player input for serving orders
public class ServeInput : MonoBehaviour
{
    [SerializeField] private float serveDistance = 1.6f;
    [SerializeField] private Transform playerTransform;

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
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleServeKey();
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

        if (manager.HasCarriedFood)
        {
            manager.TryServeCarriedFoodFromPlayer(playerTransform.position, serveDistance);
        }
        else
        {
            manager.TryPickupNextFoodForPlayer(playerTransform);
        }
    }
}
