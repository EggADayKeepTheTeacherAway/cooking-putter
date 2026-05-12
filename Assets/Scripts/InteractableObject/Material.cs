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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gatherSfx;
    [SerializeField, Range(0f, 1f)] private float gatherSfxVolume = 1f;


    [Header("Collection Popup")]
    [SerializeField] private GameObject collectionPopup; // Assign a UI Canvas child object here
    [SerializeField] private float popupDisplayTime = 1.5f;

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
        if (!canInteract) return;

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("Material.Interact: PlayerDataManager not found in scene!");
            return;
        }

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
            if (DayCycleManager.Instance != null)
            {
                DayCycleManager.Instance.AdvanceTime();
            }
        }

        PlayGatherSound();

        Debug.Log($"Collected {dropAmount}x {itemToDrop.itemName}!");

        ShowCollectionPopup();

        SetInactiveState();
    }

    private void PlayGatherSound()
    {
        if (gatherSfx == null){ 
            Debug.LogWarning("Material.PlayGatherSound: gatherSfx not assigned");
            return; 
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(gatherSfx, gatherSfxVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(gatherSfx, transform.position, gatherSfxVolume);
        }
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

}