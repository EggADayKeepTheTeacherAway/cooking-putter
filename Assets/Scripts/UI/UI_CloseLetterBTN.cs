using UnityEngine;

public class UI_CloseLetterBTN : MonoBehaviour
{
    public void CloseLetterUI() => transform.root.gameObject.SetActive(false);
}
