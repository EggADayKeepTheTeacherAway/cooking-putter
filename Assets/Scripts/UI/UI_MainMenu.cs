using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("TownScene");

    public void GoToCredit() => SceneManager.LoadScene("CreditScene");

    public void QuitGame() => Application.Quit();
}
