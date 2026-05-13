using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    public void Interact()
    {
        if (player.carriedFood == null) return;
        player.DiscardFood();
    }
}
