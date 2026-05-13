using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerWalking : MonoBehaviour
{
    [SerializeField] private Image playerSprite;
    [SerializeField] private Sprite[] playerSprites;
    [SerializeField] private float interval = 0.1f;

    private int spriteIndex = 0;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;

            if (spriteIndex >= playerSprites.Length)
                spriteIndex = 0;

            playerSprite.sprite = playerSprites[spriteIndex];
            spriteIndex++;
        }
    }
}