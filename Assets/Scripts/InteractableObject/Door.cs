using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private SceneTransitionData action;
    [SerializeField] private bool needInteract = true;

    [Header("Sound")]
    [SerializeField] private string openDoorSFX;

    private bool isPlayerInRange = false;

    public void Interact()
    {
        if (action == null)
        {
            Debug.LogWarning($"Door '{name}': SceneTransitionData (action) is null.");
            return;
        }


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
            AudioManager.Instance.PlayGlobalSFX(openDoorSFX);

            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Door '{name}': OnTriggerEnter2D with {collision.gameObject.name} (tag={collision.tag})");
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"Door '{name}': OnTriggerExit2D with {collision.gameObject.name}");
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}