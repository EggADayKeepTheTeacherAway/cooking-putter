using UnityEngine;

public class CookBookCanvas : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool isCookBookOpen;

    public void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Update()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null && player.input != null)
        {
            if (player.input.Player.OpenCookBook.WasPressedThisFrame())
            {
                ToggleCookBook();
            }

            if (player.input.Player.CloseCookBook.WasPressedThisFrame() && isCookBookOpen)
            {
                CloseCookBook();
            }
        }
    }

    public void ToggleCookBook()
    {
        if (!isCookBookOpen)
            OpenCookBook();
        else
            CloseCookBook();
    }

    public void OpenCookBook()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            isCookBookOpen = true;
        }
    }

    void CloseCookBook()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            isCookBookOpen = false;
        }
    }
}
