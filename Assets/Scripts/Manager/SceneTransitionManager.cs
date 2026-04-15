using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Image transitionEffect;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartTransition(string sceneName)
    {
        StartCoroutine(TransitionCo(sceneName));
    }


    private IEnumerator TransitionCo(string sceneName)
    {
        yield return StartCoroutine(FadeToBlackCo());

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;

        yield return StartCoroutine(FadeToTranspalentCo());
    }


    private IEnumerator FadeToBlackCo()
    {
        float time = 0f;

        Color startColor = transitionEffect.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            transitionEffect.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
            yield return null;
        }

        transitionEffect.color = targetColor;
    }


    private IEnumerator FadeToTranspalentCo ()
    {
        float time = 0f;

        Color startColor = transitionEffect.color;
        Color targetColor = new Color(0, 0, 0, 0);

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            transitionEffect.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
            yield return null;
        }

        transitionEffect.color = targetColor;
    }
}
