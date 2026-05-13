using System.Collections;
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

    [SerializeField] private float transitionDelay = 0.5f;

    private bool isInteracting = false;
    private bool isPlayerInRange = false;

    public void Interact()
    {
        if (isInteracting)
            return;

        isInteracting = true;

        if (!string.IsNullOrEmpty(openDoorSFX))
        {
            AudioManager.Instance.PlayGlobalSFX(openDoorSFX);
        }

        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (animator != null)
        {
            animator.SetBool(ANIM_PARAM_OPEN, true);
        }

        yield return new WaitForSeconds(transitionDelay);

        ExecuteTransition();
    }

    private void ExecuteTransition()
    {
        action.Execute();

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.SaveData();
    }

    private void Update()
    {
        if (player == null)
            return;

        if (isPlayerInRange && !needInteract)
        {
            Interact();
        }
        else if (isPlayerInRange &&
                 needInteract &&
                 player.input.Player.Interact.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void OnDisable()
    {
        isPlayerInRange = false;
        isInteracting = false;

        if (animator != null)
        {
            animator.SetBool(ANIM_PARAM_OPEN, false);
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