using UnityEngine;

public class ShopEnter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject shopCanvasPrefab;

    private GameObject spawnedCanvas;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered shop area.");
            if (spawnedCanvas == null)
            {
                spawnedCanvas = Instantiate(shopCanvasPrefab);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player left shop area.");
            if (spawnedCanvas != null)
            {
                Destroy(spawnedCanvas);
                spawnedCanvas = null;
            }
        }
    }
}
