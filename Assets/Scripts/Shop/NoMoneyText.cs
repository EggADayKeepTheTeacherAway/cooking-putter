using UnityEngine;
using System.Collections;

public class NoMoneyText : MonoBehaviour
{
    public CanvasGroup noMoneyGroup;

    Coroutine fadeCoroutine;

    void Awake()
    {
        noMoneyGroup.alpha = 0f;
    }

    public void ShowNoMoney()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        noMoneyGroup.alpha = 1f;
        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        yield return Fade(0f, 1f, 0.3f);
        yield return new WaitForSeconds(1.5f);
        yield return Fade(1f, 0f, 0.5f);
    }

    IEnumerator Fade(float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            noMoneyGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        noMoneyGroup.alpha = end;
    }
}