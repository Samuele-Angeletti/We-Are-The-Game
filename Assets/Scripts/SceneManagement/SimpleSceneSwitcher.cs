using UnityEngine;

public class SimpleSceneSwitcher : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void ChangeScene()
    {
        LevelManager.Instance.ChangeScene(sceneName);
    }

    public void AddScene()
    {
        LevelManager.Instance.AddScene(sceneName);
    }
}
