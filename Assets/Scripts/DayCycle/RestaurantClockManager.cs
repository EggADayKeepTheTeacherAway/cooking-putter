using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestaurantTimerManager : MonoBehaviour
{
    [Header("Clock")]
    [Range(0, 11)]
    public int currentTime = 11; // Start at 12

    [Header("Clock UI")]
    [SerializeField] private Image clockImage;

    [Header("Clock Sprites")]
    [SerializeField] private Sprite[] clockSprites;

    [Header("Timer")]
    [SerializeField] private float secondsPerHour = 30f;

    private float timer;

    private void Start()
    {
        timer = secondsPerHour;

        RefreshClock();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = secondsPerHour;

            AdvanceHour();
        }
    }

    private void AdvanceHour()
    {
        // 12 -> 1
        if (currentTime == 11)
        {
            currentTime = 0;
        }
        else
        {
            currentTime++;
        }

        RefreshClock();

        Debug.Log("Restaurant Hour: "
            + GetDisplayedHour());

        // Returned to 12 again
        if (currentTime == 11)
        {
            EndRestaurantNight();
        }
    }

    private void EndRestaurantNight()
    {
        Debug.Log("Restaurant Night Ended!");

        // Reset daytime clock to new day
        if (DayCycleManager.Instance != null)
        {
            DayCycleManager.Instance.ResetToTwelve();
        }

        SceneManager.LoadScene("TownScene");
    }

    private void RefreshClock()
    {
        if (clockImage != null &&
            clockSprites.Length > currentTime)
        {
            clockImage.sprite =
                clockSprites[currentTime];
        }
    }

    private int GetDisplayedHour()
    {
        return currentTime == 11
            ? 12
            : currentTime + 1;
    }
}