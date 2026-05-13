using UnityEngine;

public class UI_Cloud : MonoBehaviour
{
    [SerializeField] private RectTransform[] clouds;
    [SerializeField] private float cloudSpeed = 50f;

    [SerializeField] private RectTransform cloudStart;
    [SerializeField] private RectTransform cloudStop;

    private void Update()
    {
        foreach (var c in clouds)
        {
            // Move cloud right
            c.anchoredPosition += Vector2.right * cloudSpeed * Time.deltaTime;

            // Get cloud LEFT side position
            float cloudLeftSide = c.position.x - (c.rect.width * 0.5f);

            // If left side reaches stop point
            if (cloudLeftSide >= cloudStop.position.x)
            {
                // Teleport to start point
                c.position = new Vector3(
                    cloudStart.position.x,
                    c.position.y,
                    c.position.z
                );
            }
        }
    }
}