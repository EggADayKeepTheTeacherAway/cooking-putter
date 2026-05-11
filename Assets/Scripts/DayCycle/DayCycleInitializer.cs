using UnityEngine;

public class DayCycleInitializer : MonoBehaviour
{
    [SerializeField] private GameObject dayCycleManagerPrefab;

    private void Awake()
    {
        if (DayCycleManager.Instance == null)
        {
            Instantiate(dayCycleManagerPrefab);
        }
    }
}