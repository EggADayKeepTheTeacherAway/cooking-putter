using System.Collections.Generic;
using UnityEngine;

public class DirtyDishService : MonoBehaviour
{
    private readonly List<GameObject> carriedDishObjects = new();
    private readonly List<ItemData> carriedDishItems = new();
    private Transform carriedDishCarrier;
    private FoodOrderVisuals visuals;

    [SerializeField] private string dirtyDishSortingLayerName = "Customer";
    [SerializeField] private int dirtyDishSortingOrder = 2;
    [SerializeField] private float dirtyDishScale = 1f;
    [SerializeField] private ItemData dirtyDishItem;
    [SerializeField] private Vector3 carriedPreviewOffset = new Vector3(0f, 0.9f, 0f);
    [SerializeField] private Vector3 dishStackOffset = new Vector3(0f, 0.1f, 0f);
    [SerializeField] private int maxDirtyDishesCarried = 4;

    public bool HasCarriedDirtyDish => carriedDishObjects.Count > 0;

    private void Awake()
    {
        visuals = GetComponent<FoodOrderVisuals>();
    }

    public void ChangeFoodToDirtyDish(Customer customer)
    {
        if (customer == null) return;

        if (visuals == null)
        {
            visuals = GetComponent<FoodOrderVisuals>();
        }

        if (visuals == null || !visuals.TryGetFoodOnTable(customer, out var foodObject))
        {
            Debug.LogWarning($"ChangeFoodToDirtyDish: no spawned food object found for customer {customer?.name}");
            return;
        }

        visuals.RemoveBubbleFromObject(foodObject);

        if (dirtyDishItem == null || dirtyDishItem.itemIcon == null)
        {
            Debug.LogWarning("DirtyDish item not assigned or has no icon. Falling back to gray tint.");
        }

        var spriteRenderer = foodObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        if (dirtyDishItem != null && dirtyDishItem.itemIcon != null)
        {
            spriteRenderer.sprite = dirtyDishItem.itemIcon;
            Debug.Log($"Changed {customer.name}'s food to dirty dish (sprite swap)");
        }
        else
        {
            spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            Debug.Log($"Changed {customer.name}'s food to dirty dish (tinted fallback)");
        }

        if (!string.IsNullOrWhiteSpace(dirtyDishSortingLayerName))
        {
            spriteRenderer.sortingLayerName = dirtyDishSortingLayerName;
        }

        spriteRenderer.sortingOrder = dirtyDishSortingOrder;
        foodObject.transform.localScale = Vector3.one * dirtyDishScale;

        Table table = customer.GetTable();
        if (table != null)
        {
            table.RegisterDirtyDish();
        }

        var dd = foodObject.AddComponent<DirtyDish>();
        dd.owner = customer;
        dd.table = table;
        dd.item = dirtyDishItem;

        int interactLayer = LayerMask.NameToLayer("Interact");
        if (interactLayer >= 0)
        {
            foodObject.layer = interactLayer;
        }

        if (foodObject.GetComponent<Collider2D>() == null)
        {
            var col = foodObject.AddComponent<CircleCollider2D>();
            col.isTrigger = false;
        }
    }

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

        if (dd.table != null)
        {
            dd.table.ClearDirtyDish();
            dd.table = null;
        }

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

        if (dd.owner != null)
        {
            if (visuals == null)
            {
                visuals = GetComponent<FoodOrderVisuals>();
            }

            visuals?.RemoveFoodOnTableMapping(dd.owner);
            dd.owner = null;
        }

        Debug.Log($"Picked up dirty dish: {dd.item?.itemName}. Carrying {carriedDishObjects.Count} dishes.");
        return true;
    }

    public bool TryDropCarriedDishAtSink(Sink sink)
    {
        if (sink == null || carriedDishObjects.Count == 0) return false;

        sink.Fill();

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

    public void PickupCleanedDishFromWashingStation(Transform station)
    {
        if (carriedDishObjects.Count == 0)
        {
            Debug.LogWarning("PickupCleanedDishFromWashingStation: no dirty dishes to clean.");
            return;
        }

        int lastIndex = carriedDishObjects.Count - 1;
        var dish = carriedDishObjects[lastIndex];
        var item = carriedDishItems[lastIndex];

        if (dish != null)
        {
            Destroy(dish);
        }

        carriedDishObjects.RemoveAt(lastIndex);
        carriedDishItems.RemoveAt(lastIndex);

        if (carriedDishObjects.Count == 0)
        {
            carriedDishCarrier = null;
        }

        Debug.Log($"PickupCleanedDishFromWashingStation: cleaned one dirty dish ({item?.itemName}). Remaining: {carriedDishObjects.Count}");
    }
}
