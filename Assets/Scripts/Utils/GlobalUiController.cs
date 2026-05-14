using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUiController : SingletonMonoBehaviour<GlobalUiController>
{
    public bool IsClickBlocked => Time.unscaledTime < blockedUntilUnscaledTime;

    [Header("Scenes")]
    [SerializeField] SceneField mainMenuScene;

    [Header("Escape Menu")]
    [SerializeField] GameObject escapeMenuRoot;
    [SerializeField] GameObject resumeButtonObject;
    [SerializeField] GameObject exitButtonObject;

    [Header("Windowed Mode")]
    [SerializeField] int windowedWidth = 1600;
    [SerializeField] int windowedHeight = 900;

    [Header("Input")]
    [SerializeField] float clickBlockDuration = 0.2f;

    enum WindowModeCycle
    {
        ExclusiveFullscreen,
        Borderless,
        Windowed
    }

    WindowModeCycle currentWindowMode;
    float blockedUntilUnscaledTime;

    protected override void AwakeNew()
    {
        currentWindowMode = DetectCurrentWindowMode();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetEscapeMenuVisible(false);
        RefreshMenuView();
    }

    void OnDestroy()
    {
        if (I == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) {
            CycleWindowMode();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleEscapeMenu();
            return;
        }

        if (IsEscapeMenuVisible && Input.GetMouseButtonDown(0) && !IsPointerOverExitButton()) {
            Resume();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetEscapeMenuVisible(false);
        RefreshMenuView();
    }

    public void ToggleEscapeMenu()
    {
        SetEscapeMenuVisible(!IsEscapeMenuVisible);
    }

    public void Resume()
    {
        BlockClicks();
        SetEscapeMenuVisible(false);
    }

    public void ExitButtonAction()
    {
        BlockClicks();
        SetEscapeMenuVisible(false);

        if (IsInMainMenuScene()) {
            Application.Quit();
            return;
        }

        if (mainMenuScene != null && mainMenuScene.HasScene()) {
            SceneManager.LoadScene(mainMenuScene);
        } else {
            Debug.LogWarning("Main menu scene is not assigned on GlobalUiController.");
        }
    }

    void SetEscapeMenuVisible(bool isVisible)
    {
        if (escapeMenuRoot != null) {
            escapeMenuRoot.SetActive(isVisible);
        }

        Time.timeScale = isVisible && !IsInMainMenuScene() ? 0f : 1f;
    }

    void RefreshMenuView()
    {
        bool isInMainMenu = IsInMainMenuScene();

        if (resumeButtonObject != null) {
            resumeButtonObject.SetActive(!isInMainMenu);
        }

        TMP_Text exitButtonText = GetExitButtonText();
        if (exitButtonText != null) {
            exitButtonText.text = isInMainMenu ? "Выйти из игры" : "В главное меню";
        }
    }

    void CycleWindowMode()
    {
        currentWindowMode = currentWindowMode switch {
            WindowModeCycle.ExclusiveFullscreen => WindowModeCycle.Borderless,
            WindowModeCycle.Borderless => WindowModeCycle.Windowed,
            _ => WindowModeCycle.ExclusiveFullscreen
        };

        ApplyWindowMode(currentWindowMode);
    }

    void ApplyWindowMode(WindowModeCycle windowMode)
    {
        Resolution resolution = Screen.currentResolution;

        switch (windowMode) {
            case WindowModeCycle.ExclusiveFullscreen:
                Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen);
                break;
            case WindowModeCycle.Borderless:
                Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow);
                break;
            default:
                Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
                break;
        }
    }

    WindowModeCycle DetectCurrentWindowMode()
    {
        return Screen.fullScreenMode switch {
            FullScreenMode.ExclusiveFullScreen => WindowModeCycle.ExclusiveFullscreen,
            FullScreenMode.FullScreenWindow => WindowModeCycle.Borderless,
            _ => WindowModeCycle.Windowed
        };
    }

    bool IsInMainMenuScene()
    {
        return mainMenuScene != null
            && mainMenuScene.HasScene()
            && SceneManager.GetActiveScene().name == mainMenuScene.SceneName;
    }

    void BlockClicks()
    {
        blockedUntilUnscaledTime = Time.unscaledTime + clickBlockDuration;
    }

    bool IsPointerOverExitButton()
    {
        if (exitButtonObject == null) return false;
        if (!exitButtonObject.TryGetComponent(out RectTransform exitButtonRect)) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(exitButtonRect, Input.mousePosition);
    }

    TMP_Text GetExitButtonText()
    {
        return exitButtonObject != null ? exitButtonObject.GetComponentInChildren<TMP_Text>(true) : null;
    }

    bool IsEscapeMenuVisible => escapeMenuRoot != null && escapeMenuRoot.activeSelf;
}
