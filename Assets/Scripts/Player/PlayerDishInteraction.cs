using UnityEngine;
using UnityEngine.InputSystem;

// Attach to Player GameObject. Press Space to pick up dirty dishes or drop a carried dish into a nearby sink.
public class PlayerDishInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2.0f;

    private void Update()
    {
        var manager = FoodServiceManager.GetOrCreateInstance();

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (manager.HasCarriedDirtyDish)
            {
                TryDropAtNearbySink();
            }
            else
            {
                TryPickupNearbyDirtyDish();
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && manager.HasCarriedDirtyDish)
        {
            TryDropAtNearbySink();
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

    private void TryDropAtNearbySink()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, interactDistance);
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
