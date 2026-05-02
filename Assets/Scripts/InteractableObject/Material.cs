using UnityEngine;

public class Material : MonoBehaviour, IInteractable
{
    [Header("Data Mapping")]
    [SerializeField] private ItemData itemToDrop;
    [SerializeField] private int minDropAmount = 1;
    [SerializeField] private int maxDropAmount = 3;
    private int dropAmount;

    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("Collection Popup")]
    [SerializeField] private GameObject collectionPopup; // Assign a UI Canvas child object here
    [SerializeField] private float popupDisplayTime = 1.5f;

    private bool isPlayerInRange = false;
    private bool canInteract = true; // State to track if it's active

    private void Start()
    {
        // Set the initial visual state
        if (sr != null && activeSprite != null)
        {
            sr.sprite = activeSprite;
        }
    }

    public void Interact()
    {
        if (!canInteract) return; // Prevent interaction if not active

        // Safety check: ensure PlayerDataManager exists
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("Material.Interact: PlayerDataManager not found in scene!");
            return;
        }

        // Safety check: ensure itemToDrop is assigned
        if (itemToDrop == null)
        {
            Debug.LogError("Material.Interact: itemToDrop not assigned in Inspector!");
            return;
        }

        dropAmount = Random.Range(minDropAmount, maxDropAmount + 1); // +1 for inclusive max

        if (this.gameObject.GetComponentInChildren<ParticleSystem>() != null)
        {
            this.gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);

            // Add item directly to PlayerDataManager
            PlayerDataManager.Instance.AddItem(itemToDrop, dropAmount);
        }

        Debug.Log($"Collected {dropAmount}x {itemToDrop.itemName}!");

        ShowCollectionPopup();

        SetInactiveState();
    }

    private void ShowCollectionPopup()
    {
        if (collectionPopup != null)
        {
            collectionPopup.SetActive(true);
            collectionPopup.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{itemToDrop.itemName} x{dropAmount}";
            // Hide the popup after some time
            Invoke(nameof(HideCollectionPopup), popupDisplayTime);
        }
    }

    private void HideCollectionPopup()
    {
        if (collectionPopup != null)
        {
            collectionPopup.SetActive(false);
        }
    }

    private void SetInactiveState()
    {
        canInteract = false; // Switch to inactive state after gathering
        if (sr != null && inactiveSprite != null)
        {
            sr.sprite = inactiveSprite;
        }
    }

    private void Update()
    {
        // Only allow interaction input if the object is still interactable
        if (canInteract && isPlayerInRange)
        {
            // Get player input directly from PlayerInputSet
            Player player = FindFirstObjectByType<Player>();
            if (player != null && player.input.Player.Interact.WasPressedThisFrame())
            {
                Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}