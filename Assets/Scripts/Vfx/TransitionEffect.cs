using UnityEngine;

public class TransitionEffect : MonoBehaviour
{
    
    private static TransitionEffect instance;
    public static TransitionEffect Instance => instance;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
