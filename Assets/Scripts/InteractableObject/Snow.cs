using UnityEngine;

public class Snow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.enabled = true;
        }
    }
}