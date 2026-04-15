using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Entity player;
    [SerializeField] private SceneTransitionManager transitionManager;
    [SerializeField] private SceneTransitionData action;

    public void Interact()
    {
        action.Execute(transitionManager);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player.input.Player.Interact.IsPressed())
        {
            Interact();
        }
    }
}
