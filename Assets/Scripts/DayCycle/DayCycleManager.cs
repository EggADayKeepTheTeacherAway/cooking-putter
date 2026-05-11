using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayCycleManager : MonoBehaviour
{
    public static DayCycleManager Instance;

    [Header("Time")]
    [Range(0, 11)]
    public int currentTime = 11; // Start at 12

    [Header("Clock UI")]
    public Image clockImage;

    [Header("Clock Sprites")]
    public Sprite[] clockSprites;

    [Header("Scene")]
    public string restaurantSceneName = "RestaurantScene";

    private bool firstCycle = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshClock();
    }

    public void AdvanceTime()
    {
        // If currently at 11 o'clock
        // next pickup ends the day
        if (currentTime == 10)
        {
            currentTime = 11;

            RefreshClock();

            Debug.Log("Current Hour: 12");

            EndDay();

            return;
        }

        // Normal hour progression
        if (currentTime == 11)
        {
            // 12 -> 1
            currentTime = 0;
        }
        else
        {
            currentTime++;
        }

        RefreshClock();

        Debug.Log("Current Hour: " + GetDisplayedHour());
    }

    private void EndDay()
    {
        Debug.Log("Day Ended!");
        ResetEndOfDayVariables();

        // Reset clock back to 12
        currentTime = 11;
        SceneManager.LoadScene(restaurantSceneName);
    }

    private void ResetEndOfDayVariables()
    {

        Debug.Log("Resetting end-of-day variables");
    }

    public void RefreshClock()
    {
        if (clockImage != null &&
            clockSprites.Length > currentTime)
        {
            clockImage.sprite = clockSprites[currentTime];
        }
    }

    public void ResetToTwelve()
    {
        currentTime = 11;
        RefreshClock();

        Debug.Log("Clock reset to 12");
    }

    public int GetDisplayedHour()
    {
        return currentTime == 11
            ? 12
            : currentTime + 1;
    }
}