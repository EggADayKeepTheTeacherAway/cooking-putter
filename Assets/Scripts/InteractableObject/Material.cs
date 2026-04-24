using UnityEngine;

public class Material : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    private bool isPlayerInRange = false;

    public void Interact()
    {
        Debug.Log("Interacted with material: " + gameObject.name);
    }

    private void Update()
    {
        if (isPlayerInRange && player.input.Player.Interact.WasPressedThisFrame())
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