using System.Collections.Generic;
using UnityEngine;

public class FoodOrderVisuals : MonoBehaviour
{
    private readonly Dictionary<Customer, GameObject> customerOrderPreviews = new();
    private readonly Dictionary<Customer, GameObject> foodOnTableObjects = new();

    [SerializeField] private Vector3 customerPreviewOffset = new Vector3(0f, 1.4f, 0f);
    [SerializeField] private Vector3 foodOnTableOffset = new Vector3(0f, -0.8f, 0f);
    [SerializeField] private float previewScale = 1.5f;
    [SerializeField] private float foodOnTableScale = 1.6f;
    [SerializeField] private string previewSortingLayerName = "Foreground";
    [SerializeField] private int previewSortingOrder = 100;
    [Header("Bubble Background")]
    [SerializeField] private Sprite bubbleSprite;
    [SerializeField] private Color bubbleColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Vector3 bubbleOffset = Vector3.zero;
    [SerializeField] private float bubbleScale = 0.1f;
    [SerializeField] private string bubbleSortingLayerName = "Foreground";
    [SerializeField] private int bubbleSortingOrderOffset = -1;
    [SerializeField] private string foodOnTableSortingLayerName = "Customer";
    [SerializeField] private int foodOnTableSortingOrder = 0;

    public void SpawnCustomerOrderPreview(Customer customer, Food food)
    {
        if (customer == null || food == null || food.Icon == null) return;

        RemoveCustomerOrderPreview(customer);

        var preview = CreatePreviewObject($"{food.FoodName} Preview", food.Icon, customerPreviewOffset, customer.transform, previewScale);
        customerOrderPreviews[customer] = preview;

        if (preview != null)
        {
            var sr = preview.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 150;
            }
        }
    }

    public void RemoveCustomerOrderPreview(Customer customer)
    {
        if (customer == null) return;

        if (customerOrderPreviews.TryGetValue(customer, out var preview) && preview != null)
        {
            Destroy(preview);
        }

        customerOrderPreviews.Remove(customer);
    }

    public GameObject SpawnFoodOnTable(Customer customer, Food food)
    {
        if (customer == null || food == null || food.Icon == null) return null;

        Vector3 foodPosition = GetFoodOnTablePosition(customer);
        GameObject foodObject = CreateFoodObjectWithSorting($"{food.FoodName} OnTable", food.Icon, foodPosition, null, foodOnTableScale, foodOnTableSortingLayerName, foodOnTableSortingOrder);

        RemoveBubbleFromObject(foodObject);
        foodOnTableObjects[customer] = foodObject;
        return foodObject;
    }

    public bool TryGetFoodOnTable(Customer customer, out GameObject foodObject)
    {
        foodObject = null;
        return customer != null && foodOnTableObjects.TryGetValue(customer, out foodObject) && foodObject != null;
    }

    public void RemoveFoodOnTableMapping(Customer customer)
    {
        if (customer == null) return;
        foodOnTableObjects.Remove(customer);
    }

    public void RemoveFoodFromTable(Customer customer)
    {
        if (customer == null) return;

        if (foodOnTableObjects.TryGetValue(customer, out var foodObject) && foodObject != null)
        {
            var dirtyDish = foodObject.GetComponent<DirtyDish>();
            if (dirtyDish != null && dirtyDish.table != null)
            {
                dirtyDish.table.ClearDirtyDish();
                dirtyDish.table = null;
            }

            Destroy(foodObject);
        }

        foodOnTableObjects.Remove(customer);
    }

    public void RemoveBubbleFromObject(GameObject obj)
    {
        if (obj == null || bubbleSprite == null) return;

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

    public void ClearAllPreviews()
    {
        foreach (var preview in customerOrderPreviews.Values)
        {
            if (preview != null)
            {
                Destroy(preview);
            }
        }

        customerOrderPreviews.Clear();
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

        if (createBubble && bubbleSprite != null)
        {
            var bubble = new GameObject(objectName + " Bubble");
            bubble.transform.SetParent(preview.transform);
            bubble.transform.localPosition = bubbleOffset;
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
}
