using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;

    [Header("Fade Settings")]
    [SerializeField] private float startDelay = 0.8f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Scene BGM")]
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip townBGM;
    [SerializeField] private AudioClip restaurantBGM;
    [SerializeField] private AudioClip forestBGM;

    private float maximumVolume = 0.4f;
    private Coroutine bgmCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayBGM(menuBGM);
                maximumVolume = 0.4f;
                break;

            case "TownScene":
                PlayBGM(townBGM);
                maximumVolume = 0.2f;
                break;
            case "RestaurantScene":
                PlayBGM(restaurantBGM);
                maximumVolume = 0.6f;
                break;
            case "ForestScene":
                PlayBGM(forestBGM);
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip)
            return;

        if (bgmCoroutine != null)
            StopCoroutine(bgmCoroutine);

        bgmCoroutine = StartCoroutine(FadeInBGM(clip));
    }

    private IEnumerator FadeInBGM(AudioClip newClip)
    {
        bgmSource.Stop();

        yield return new WaitForSeconds(startDelay);

        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, maximumVolume, time / fadeDuration);
            yield return null;
        }

        bgmSource.volume = maximumVolume;
    }
}
