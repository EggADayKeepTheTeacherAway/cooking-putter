using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bellRingSFX;
    [SerializeField] private Player player;
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
    private IEnumerator EndRestaurantNightCo()
    {
        Debug.Log("Restaurant Night Ended!");
        player.LockControls();
        audioSource.PlayOneShot(bellRingSFX);

        yield return new WaitForSeconds(1f);

        Debug.Log("Trying to show result UI");

        Debug.Log(ResultUI.Instance);

        ResultUI.Instance.Show(OnResultFinished);
    }
    private void OnResultFinished()
    {
        player.UnlockControls();
        PlayerDataManager.Instance.AdvanceDay();
        PlayerDataManager.Instance.SaveData();

        if (DayCycleManager.Instance != null)
        {
            DayCycleManager.Instance.ResetToTwelve();
        }

        SceneTransitionManager.Instance.SetForcedSpawnPosition(
            SceneTransitionManager.Instance.RestaurantTownSpawnPosition
        );

        SceneTransitionManager.Instance.StartTransition("TownScene");
    }
    private void EndRestaurantNight()
    {
        StartCoroutine(EndRestaurantNightCo());
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