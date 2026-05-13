using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void StartGame() => SceneTransitionManager.Instance.StartTransition("TownScene");

    public void GoToCredit() => SceneTransitionManager.Instance.StartTransition("CreditScene");

    public void QuitGame() => Application.Quit();
}
