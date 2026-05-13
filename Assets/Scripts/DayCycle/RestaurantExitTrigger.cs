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
                PlayerDataManager.Instance.AdvanceDay();
                PlayerDataManager.Instance.SaveData();
                Debug.Log("New day started");
            }
            SceneTransitionManager.Instance.SetForcedSpawnPosition(
                SceneTransitionManager.Instance.RestaurantTownSpawnPosition
            );
        }
    }
}