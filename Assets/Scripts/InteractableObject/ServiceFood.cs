using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ServiceFood : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;

    private void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public void Interact()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (player == null)
        {
            Debug.LogWarning("ServiceFood: no Player found to carry food.");
            return;
        }

        FoodServiceManager.GetOrCreateInstance().TryPickupNextOrderForPlayer(player);
    }
}
