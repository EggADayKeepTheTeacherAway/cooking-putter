using UnityEngine;

public class RestaurantExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (DayCycleManager.Instance != null && DayCycleManager.Instance.isNight)
            {
                DayCycleManager.Instance.ResetToTwelve();

                Debug.Log("New day started");
            }
            SceneTransitionManager.Instance.SetForcedSpawnPosition(
                SceneTransitionManager.Instance.RestaurantTownSpawnPosition
            );
        }
    }
}