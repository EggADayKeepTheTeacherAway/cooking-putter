using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private SceneTransitionData action;
    [SerializeField] private bool needInteract = true;
    private bool isPlayerInRange = false;

    public void Interact()
    {
        action.Execute();
        PlayerDataManager.Instance.SaveData();
    }

    private void Update()
    {
        if (isPlayerInRange && (player.input.Player.Interact.IsPressed() || !needInteract))
        {
            Interact();
            Debug.Log("Player interacted with door: " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered door area.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left door area.");
        }
    }
}
