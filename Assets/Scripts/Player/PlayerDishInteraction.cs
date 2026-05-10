using UnityEngine;
using UnityEngine.InputSystem;

// Attach to Player GameObject. Press Space to pick up dirty dishes and click sink to drop.
public class PlayerDishInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2.0f;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryPickupNearbyDirtyDish();
        }

        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryDropAtClickedSink();
        }
    }

    private void TryPickupNearbyDirtyDish()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, interactDistance);
        if (hits == null || hits.Length == 0) return;

        DirtyDish nearestDish = null;
        float bestSq = float.MaxValue;

        foreach (var col in hits)
        {
            if (col == null) continue;

            var dd = col.GetComponent<DirtyDish>();
            if (dd == null) continue;

            float sq = ((Vector2)transform.position - (Vector2)col.transform.position).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                nearestDish = dd;
            }
        }

        if (nearestDish != null)
        {
            FoodServiceManager.GetOrCreateInstance().PickupDirtyDish(nearestDish.gameObject, transform);
        }
    }

    private void TryDropAtClickedSink()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 wp = Camera.main != null ? Camera.main.ScreenToWorldPoint(mousePos) : new Vector3(mousePos.x, mousePos.y, 0f);
        Vector2 point = new Vector2(wp.x, wp.y);
        var hits = Physics2D.OverlapPointAll(point);
        if (hits == null || hits.Length == 0) return;

        SinkBehaviour foundSink = null;
        Collider2D target = null;

        foreach (var col in hits)
        {
            if (col == null) continue;
            var sink = col.GetComponent<SinkBehaviour>();
            if (sink != null)
            {
                foundSink = sink;
                target = col;
                break;
            }
        }

        if (foundSink == null || target == null) return;

        float sq = ((Vector2)transform.position - (Vector2)target.transform.position).sqrMagnitude;
        if (sq <= interactDistance * interactDistance)
        {
            FoodServiceManager.GetOrCreateInstance().TryDropCarriedDishAtSink(foundSink);
        }
    }
}
