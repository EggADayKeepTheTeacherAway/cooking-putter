using UnityEngine;

public class DirtyDish : MonoBehaviour, IInteractable
{
    public Customer owner;
    public Table table;
    public ItemData item;

    public void Interact()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("DirtyDish: no Player found to pick up dish.");
            return;
        }

        FoodServiceManager.GetOrCreateInstance().PickupDirtyDish(gameObject, player.transform);
    }
}
