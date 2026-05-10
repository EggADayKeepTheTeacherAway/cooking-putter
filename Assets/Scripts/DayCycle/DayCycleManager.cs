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
        UpdateClock();
    }

    public void AdvanceTime()
    {
        currentTime++;

        // Passed 12
        if (currentTime >= 12)
        {
            currentTime = 0;
        }

        UpdateClock();

        Debug.Log("Current Hour: " + GetDisplayedHour());

        // Returned to 12 again
        if (currentTime == 11)
        {
            if (!firstCycle)
            {
                EndDay();
            }

            firstCycle = false;
        }
    }

    private void EndDay()
    {
        Debug.Log("Day Ended!");

        // Reset variables here
        ResetEndOfDayVariables();

        // Reset clock back to 12
        currentTime = 11;

        // Load restaurant scene
        SceneManager.LoadScene(restaurantSceneName);
    }

    private void ResetEndOfDayVariables()
    {

        Debug.Log("Resetting end-of-day variables");
    }

    private void UpdateClock()
    {
        if (clockImage != null &&
            clockSprites.Length > currentTime)
        {
            clockImage.sprite = clockSprites[currentTime];
        }
    }

    public int GetDisplayedHour()
    {
        return currentTime == 11
            ? 12
            : currentTime + 1;
    }
}