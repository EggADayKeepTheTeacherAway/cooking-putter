using UnityEngine;

public class EndLetter : MonoBehaviour, IInteractable
{
    [SerializeField] private float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool moving = true;

    private void Start()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        targetPosition =
            cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));

        Vector3 topPosition =
            cam.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 10f));

        transform.position = topPosition;
    }

    private void Update()
    {
        if (!moving)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;

            moving = false;
        }
    }

    public void Interact()
    {
        RestaurantManager.Instance.ShowEndLetter();

        Destroy(gameObject);
    }
}