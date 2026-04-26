using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private SceneTransitionData action;
    [SerializeField] private bool needInteract = true;

    public void Interact()
    {
        action.Execute();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player.input.Player.Interact.IsPressed() || !needInteract && collision.CompareTag("Player"))
        {
            Interact();
        }
    }
}
