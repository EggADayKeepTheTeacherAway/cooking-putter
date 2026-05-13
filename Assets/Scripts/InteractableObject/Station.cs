using System.Collections;
using UnityEngine;

public class Station : MonoBehaviour, IInteractable
{
    [SerializeField] private string stationName;
    [SerializeField] private Player player;
    [SerializeField] private GameObject progressBarRoot;
    [SerializeField] private Transform progressFill;
    [SerializeField] private SpriteRenderer stationSr;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private AudioSource audioSource;
    private Coroutine cookingCoroutine = null;

    public void Interact()
    {
        if (cookingCoroutine != null)
        {
            Debug.LogWarning("Already cooking");
            return;
        }
        if (player.carriedFood == null)
        {
            Debug.LogWarning("No carried food");
            return;
        }

        if (player.carriedFood.IsDone()) 
        {
            Debug.LogWarning("Food is already finished");
            return;
        }

        if (player.carriedFood.CurrentStep() == null)
        {
            Debug.LogWarning("No current step on food");
            return;
        }

        if (player.carriedFood.CurrentStep().Station != stationName)
        {
            Debug.LogWarning("Wrong station to process");
            return;
        }

        audioSource.clip = player.carriedFood.CurrentStep().Audio;
        audioSource.loop = true;
        audioSource.priority = 120;
        audioSource.Play();
        cookingCoroutine = StartCoroutine(ProcessCooking());
        if (stationSr != null)
        {
            if (activeSprite != null) stationSr.sprite = activeSprite;
        }

    }

    private IEnumerator ProcessCooking()
    {
        var step = player.carriedFood.CurrentStep();
        float duration = step.Duration;

        if (progressBarRoot != null) progressBarRoot.SetActive(true);

        float elapsed = 0f;
        Debug.Log("Start Cooking");
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (progressFill != null)
            {
                progressFill.localPosition = new Vector3(-((1-t)/2), 0f, 0f);
                progressFill.localScale = new Vector3(t, 0.1f, 1f);
            }
            yield return null;
        }

    
        player.carriedFood.NextStep();
        ResetProgress();
        Debug.Log("Cooking step completed");
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (cookingCoroutine == null) return;
            ResetProgress();
            Debug.Log("Cooking cancelled - player left station");
            
        }
    }

    private void ResetProgress()
    {
        if (progressBarRoot != null)
        {
            progressBarRoot.SetActive(false);
            progressFill.localPosition = new Vector3(-0.5f, 0f, 0f);
            progressFill.localScale = new Vector3(0f, 0.1f, 1f); 
        }
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            if (stationSr != null)
            {
                if (defaultSprite != null) stationSr.sprite = defaultSprite;
            }
            audioSource.Stop();
            cookingCoroutine = null;
        }
    }
}
