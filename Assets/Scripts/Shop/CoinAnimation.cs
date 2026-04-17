using UnityEngine;
using UnityEngine.UI;

public class CoinAnimation : MonoBehaviour
{
    public float floatSpeed = 50f;
    public float lifetime = 1f;

    private CanvasGroup cg;
    private float timer = 0f;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // move up
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // fade out
        timer += Time.deltaTime;
        cg.alpha = 1 - (timer / lifetime);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}