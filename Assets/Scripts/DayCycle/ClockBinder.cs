using UnityEngine;
using UnityEngine.UI;

public class ClockUIBinder : MonoBehaviour
{
    [SerializeField] private Image clockImage;

    private void Start()
    {
        if (DayCycleManager.Instance != null)
        {
            DayCycleManager.Instance.clockImage = clockImage;

            DayCycleManager.Instance.RefreshClock();
        }
    }
}