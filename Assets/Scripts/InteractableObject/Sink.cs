using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SinkBehaviour : MonoBehaviour, IInteractable
{
    [Header("Sink Visual")]
    [SerializeField] private bool changeSinkSprite = true;
    [SerializeField] private Sprite fullSinkSprite;
    [SerializeField] private Vector3 fullSinkPositionOffset = Vector3.zero;
    [SerializeField] private float positionMoveSpeed = 0f;

    [Header("Washing")]
    [SerializeField] private GameObject progressBarRoot;
    [SerializeField] private Transform progressFill;
    [SerializeField] private float washDuration = 2f;
    
    private SpriteRenderer sr;
    private Sprite defaultSprite;
    private Vector3 defaultPosition;
    private bool isFull = false;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Coroutine washingCoroutine;
    private Sprite fallbackSprite;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) defaultSprite = sr.sprite;
        defaultPosition = transform.position;

        ResolveProgressBarReferences();
        EnsureProgressBarExists();
        HideProgressBar();
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
        if (changeSinkSprite && sr != null)
        {
            if (fullSinkSprite != null)
            {
                sr.sprite = fullSinkSprite;
            }
            else
            {
                Debug.LogWarning("SinkBehaviour.Fill called but fullSinkSprite is not assigned.");
            }
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
        if (changeSinkSprite && sr != null)
        {
            sr.sprite = defaultSprite;
        }

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

    public void Interact()
    {
        var manager = FoodServiceManager.GetOrCreateInstance();
        if (manager.HasCarriedDirtyDish)
        {
            manager.TryDropCarriedDishAtSink(this);
            return;
        }

        if (washingCoroutine != null)
        {
            Debug.LogWarning("Already washing");
            return;
        }

        if (!isFull)
        {
            Debug.LogWarning("No dirty dishes in sink to wash");
            return;
        }

        washingCoroutine = StartCoroutine(ProcessWashing());
    }

    private IEnumerator ProcessWashing()
    {
        ResolveProgressBarReferences();
        ShowProgressBar();

        float elapsed = 0f;
        Debug.Log("Start Washing");

        while (elapsed < washDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / washDuration);

            if (progressFill != null)
            {
                progressFill.localPosition = new Vector3(-((1f - t) / 2f), 0f, 0f);
                progressFill.localScale = new Vector3(t, 0.1f, 1f);
            }

            yield return null;
        }

        FoodServiceManager.GetOrCreateInstance().PickupCleanedDishFromWashingStation(transform);
        Empty();
        ResetProgressBar();
        Debug.Log("Washing completed");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (washingCoroutine == null) return;

        ResetProgressBar();
        Debug.Log("Washing cancelled - player left sink");
    }

    private void ResolveProgressBarReferences()
    {
        if (progressBarRoot == null)
        {
            Transform foundRoot = FindChildRecursive(transform, "ProgressBar");
            if (foundRoot != null)
            {
                progressBarRoot = foundRoot.gameObject;
            }
        }

        if (progressBarRoot != null && progressFill == null)
        {
            progressFill = FindChildRecursive(progressBarRoot.transform, "Fill");
        }
    }

    private void EnsureProgressBarExists()
    {
        if (progressBarRoot != null) return;

        var root = new GameObject("ProgressBar");
        root.transform.SetParent(transform);
        root.transform.localPosition = new Vector3(0f, 1f, 0f);
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        CreateProgressBarPart("Bg", root.transform, new Color(0f, 0f, 0f, 0.7f), Vector3.zero, new Vector3(1f, 0.1f, 1f), 0);
        var fill = CreateProgressBarPart("Fill", root.transform, new Color(0f, 1f, 0f, 1f), new Vector3(-0.5f, 0f, 0f), new Vector3(0f, 0.1f, 1f), 1);

        progressBarRoot = root;
        progressFill = fill != null ? fill.transform : null;

        // Keep the root disabled until washing starts.
        progressBarRoot.SetActive(false);
    }

    private Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null) return fallbackSprite;

        fallbackSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f),
            100f);
        return fallbackSprite;
    }

    private GameObject CreateProgressBarPart(string partName, Transform parent, Color color, Vector3 localPosition, Vector3 localScale, int sortingOrder)
    {
        var part = new GameObject(partName);
        part.transform.SetParent(parent);
        part.transform.localPosition = localPosition;
        part.transform.localRotation = Quaternion.identity;
        part.transform.localScale = localScale;

        var renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = GetFallbackSprite();
        renderer.color = color;
        if (sr != null)
        {
            renderer.sortingLayerName = sr.sortingLayerName;
            renderer.sortingOrder = sr.sortingOrder + 100 + sortingOrder;
        }

        return part;
    }

    private static Transform FindChildRecursive(Transform parent, string targetName)
    {
        if (parent == null) return null;

        foreach (Transform child in parent)
        {
            if (child.name == targetName) return child;

            Transform nested = FindChildRecursive(child, targetName);
            if (nested != null) return nested;
        }

        return null;
    }

    private void ShowProgressBar()
    {
        if (progressBarRoot != null)
        {
            progressBarRoot.SetActive(true);
        }

        if (progressFill != null)
        {
            progressFill.localPosition = new Vector3(-0.5f, 0f, 0f);
            progressFill.localScale = new Vector3(0f, 0.1f, 1f);
        }
    }

    private void HideProgressBar()
    {
        if (progressBarRoot != null)
        {
            progressBarRoot.SetActive(false);
        }
    }

    private void ResetProgressBar()
    {
        HideProgressBar();

        if (progressFill != null)
        {
            progressFill.localPosition = new Vector3(-0.5f, 0f, 0f);
            progressFill.localScale = new Vector3(0f, 0.1f, 1f);
        }

        if (washingCoroutine != null)
        {
            StopCoroutine(washingCoroutine);
            washingCoroutine = null;
        }
    }

    public bool IsFull => isFull;
}
