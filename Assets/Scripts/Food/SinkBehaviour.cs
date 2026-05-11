using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SinkBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite fullSinkSprite;
    [SerializeField] private Vector3 fullSinkPositionOffset = Vector3.zero;
    [SerializeField] private float positionMoveSpeed = 0f;
    
    private SpriteRenderer sr;
    private Sprite defaultSprite;
    private Vector3 defaultPosition;
    private bool isFull = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) defaultSprite = sr.sprite;
        defaultPosition = transform.position;
    }

    private void Update()
    {
        if (isMoving && positionMoveSpeed > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionMoveSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void Fill()
    {
        if (sr == null) return;
        if (fullSinkSprite != null)
        {
            sr.sprite = fullSinkSprite;
        }
        else
        {
            Debug.LogWarning("SinkBehaviour.Fill called but fullSinkSprite is not assigned.");
        }
        isFull = true;

        // Move sink to full position
        targetPosition = defaultPosition + fullSinkPositionOffset;
        if (positionMoveSpeed > 0f)
        {
            isMoving = true;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    public void Empty()
    {
        if (sr == null) return;
        sr.sprite = defaultSprite;
        isFull = false;

        // Move sink back to default position
        targetPosition = defaultPosition;
        if (positionMoveSpeed > 0f)
        {
            isMoving = true;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    public bool IsFull => isFull;
}
