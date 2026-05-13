using UnityEngine;
using UnityEngine.InputSystem;

// Attach to Player GameObject. Press Space to pick up dirty dishes or drop a carried dish into a nearby sink.
public class PlayerDishInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2.0f;
    [SerializeField] private float washHoldDelay = 0.25f;

    private float washHoldTimer = 0f;
    private bool wasWashTriggeredThisHold = false;

    private void Update()
    {
        var manager = FoodServiceManager.GetOrCreateInstance();
        var nearbySink = FindNearbySink();
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool spaceHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        if (spacePressed)
        {
            if (manager.HasCarriedDirtyDish && nearbySink != null)
            {
                TryDropAtNearbySink(nearbySink);
                wasWashTriggeredThisHold = false;
                washHoldTimer = 0f;
            }
            else
            {
                TryPickupNearbyDirtyDish();
            }
        }

        if (!spaceHeld)
        {
            washHoldTimer = 0f;
            wasWashTriggeredThisHold = false;
            return;
        }

        if (nearbySink != null && nearbySink.IsFull && !manager.HasCarriedDirtyDish)
        {
            washHoldTimer += Time.deltaTime;
            if (!wasWashTriggeredThisHold && washHoldTimer >= washHoldDelay)
            {
                nearbySink.Interact();
                wasWashTriggeredThisHold = true;
            }
        }
        else
        {
            washHoldTimer = 0f;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && manager.HasCarriedDirtyDish)
        {
            TryDropAtNearbySink(nearbySink);
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
            if (col.transform.IsChildOf(transform)) continue;

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

    private void TryDropAtNearbySink(SinkBehaviour foundSink = null)
    {
        if (foundSink != null)
        {
            FoodServiceManager.GetOrCreateInstance().TryDropCarriedDishAtSink(foundSink);
        }
    }

    private SinkBehaviour FindNearbySink()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, interactDistance);
        if (hits == null || hits.Length == 0) return null;

        foreach (var col in hits)
        {
            if (col == null) continue;
            var sink = col.GetComponent<SinkBehaviour>();
            if (sink != null)
            {
                float sq = ((Vector2)transform.position - (Vector2)col.transform.position).sqrMagnitude;
                if (sq <= interactDistance * interactDistance)
                {
                    return sink;
                }
            }
        }

        return null;
    }
}
