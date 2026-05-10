using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SinkBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite fullSinkSprite;
    private SpriteRenderer sr;
    private Sprite defaultSprite;
    private bool isFull = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) defaultSprite = sr.sprite;
    }

    public void Fill()
    {
        if (sr == null) return;
        if (fullSinkSprite != null)
        {
            sr.sprite = fullSinkSprite;
        }
        isFull = true;
    }

    public void Empty()
    {
        if (sr == null) return;
        sr.sprite = defaultSprite;
        isFull = false;
    }

    public bool IsFull => isFull;
}
