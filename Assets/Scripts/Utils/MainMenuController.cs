using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] SceneField playScene;

    public void Play()
    {
        if (GlobalUiController.I != null && GlobalUiController.I.IsClickBlocked) return;

        if (playScene == null || !playScene.HasScene()) {
            Debug.LogWarning("Play scene is not assigned on MainMenuController.");
            return;
        }

        SceneManager.LoadScene(playScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
