using UnityEngine;

[CreateAssetMenu(menuName = "Scene Transition/Load Scene")]
public class SceneTransitionData : ScriptableObject
{
    [SerializeField] private string sceneName;

    public void Execute(SceneTransitionManager manager)
    {
        manager.StartTransition(sceneName);
    }
}
