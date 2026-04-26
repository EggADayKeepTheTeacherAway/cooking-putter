using UnityEngine;

public class Material : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
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

        if (this.gameObject.GetComponentInChildren<ParticleSystem>() != null)
        {
            this.gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
            player.AddItem(new ItemData { itemName = gameObject.name }, 1);
        }
        Debug.Log("Interacted with material: " + gameObject.name);
        ShowCollectionPopup();

        SetInactiveState();
    }

    private void ShowCollectionPopup()
    {
        if (collectionPopup != null)
        {
            collectionPopup.SetActive(true);
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
        if (canInteract && isPlayerInRange && player.input.Player.Interact.WasPressedThisFrame())
        {
            Interact();
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