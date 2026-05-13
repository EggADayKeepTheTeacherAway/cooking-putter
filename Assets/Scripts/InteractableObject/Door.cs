using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private const string ANIM_PARAM_OPEN = "IsOpen";

    [SerializeField] private Player player;
    [SerializeField] private SceneTransitionData action;
    [SerializeField] private bool needInteract = true;

    [Header("Sound")]
    [SerializeField] private string openDoorSFX;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isInteracting = false;
    private bool isPlayerInRange = false;


    public void Interact()
    {
        if (isInteracting)
            return;

        if (action == null)
        {
            Debug.LogWarning($"Door '{name}': SceneTransitionData (action) is null.");
            return;
        }
        
        isInteracting = true;

        if (!string.IsNullOrEmpty(openDoorSFX))
        {
            AudioManager.Instance.PlayGlobalSFX(openDoorSFX);
        }

        // Trigger animation
        PlayOpenAnimation();
    }

    private void PlayOpenAnimation()
    {
        if (animator == null)
        {
            ExecuteTransition();
            return;
        }

        animator.SetBool(ANIM_PARAM_OPEN, true);

        ExecuteTransition();
    }

    private void ExecuteTransition()
    {
        isPlayerInRange = false;
        isInteracting = false;

        action.Execute();

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.SaveData();
        else
            Debug.LogWarning("PlayerDataManager.Instance is null.");
    }

    private void Update()
    {
        if (isPlayerInRange && !needInteract)
        {
            Interact();
        }
        else if (isPlayerInRange && needInteract && player.input.Player.Interact.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void OnDisable()
    {
        isPlayerInRange = false;
        isInteracting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Door '{name}': OnTriggerEnter2D with {collision.gameObject.name} (tag={collision.tag})");
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log($"Door '{name}': OnTriggerExit2D with {collision.gameObject.name}");
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}