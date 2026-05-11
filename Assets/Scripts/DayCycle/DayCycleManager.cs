using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip bellRingSFX;

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
    private IEnumerator EndDayCo()
    {
        Debug.Log("Day Ended!");

        ResetEndOfDayVariables();

        audioSource.PlayOneShot(bellRingSFX);

        yield return new WaitForSeconds(bellRingSFX.length);

        // Reset clock back to 12
        currentTime = 11;

        // Go to restaurant
        SceneTransitionManager.Instance.StartTransition(
            restaurantSceneName
        );
    }
    private void EndDay()
    {
        StartCoroutine(EndDayCo());
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

        Debug.Log("New Day Started!");
    }

    public int GetDisplayedHour()
    {
        return currentTime == 11
            ? 12
            : currentTime + 1;
    }
}