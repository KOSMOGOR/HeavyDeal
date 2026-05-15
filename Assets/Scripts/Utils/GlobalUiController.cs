using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalUiController : SingletonMonoBehaviour<GlobalUiController>
{
    public bool IsClickBlocked => Time.unscaledTime < blockedUntilUnscaledTime;

    [Header("Scenes")]
    [SerializeField] SceneField mainMenuScene;

    [Header("Pause Menu")]
    [SerializeField] PauseMenuView pauseMenuPrefab;

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
    PauseMenuView pauseMenuView;
    GameObject escapeMenuRoot;
    GameObject resumeButtonObject;
    GameObject restartButtonObject;
    GameObject exitButtonObject;

    protected override void AwakeNew()
    {
        if (pauseMenuPrefab == null) {
            pauseMenuPrefab = Resources.Load<PauseMenuView>("Prefabs/PauseMenu");
        }

        currentWindowMode = DetectCurrentWindowMode();
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsurePauseMenuInstance();
        SetEscapeMenuVisible(false);
        RefreshMenuView();
    }

    protected override void OnDestroy()
    {
        if (I == this) SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDestroy();
    }

    void Update()
    {
        if (ShouldShowGameOverMenu()) {
            SetEscapeMenuVisible(true);
        }

        if (Input.GetKeyDown(KeyCode.F11)) {
            CycleWindowMode();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleEscapeMenu();
            return;
        }

        if (IsEscapeMenuVisible && CanResume() && Input.GetMouseButtonDown(0) && !IsPointerOverExitButton() && !IsPointerOverRestartButton()) {
            Resume();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePauseMenuInstance();
        SetEscapeMenuVisible(false);
        RefreshMenuView();
    }

    public void ToggleEscapeMenu()
    {
        if (ShouldShowGameOverMenu()) {
            SetEscapeMenuVisible(true);
            return;
        }

        SetEscapeMenuVisible(!IsEscapeMenuVisible);
    }

    public void Resume()
    {
        if (!CanResume()) return;

        BlockClicks();
        SetEscapeMenuVisible(false);
    }

    public void RestartButtonAction()
    {
        if (!CanRestartCurrentScene()) return;

        BlockClicks();
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        ResetGameplaySingletons();
        SceneManager.LoadScene(activeScene.name);
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
            ResetGameplaySingletons();
            SceneManager.LoadScene(mainMenuScene);
        } else {
            Debug.LogWarning("Main menu scene is not assigned on GlobalUiController.");
        }
    }

    void SetEscapeMenuVisible(bool isVisible)
    {
        RefreshMenuView();

        if (escapeMenuRoot != null) {
            escapeMenuRoot.SetActive(isVisible);
        }

        Time.timeScale = isVisible && !IsInMainMenuScene() ? 0f : 1f;
    }

    void RefreshMenuView()
    {
        bool isInMainMenu = IsInMainMenuScene();
        bool canRestartCurrentScene = CanRestartCurrentScene();
        bool canResumeCurrentScene = CanResume();

        if (resumeButtonObject != null) {
            resumeButtonObject.SetActive(!isInMainMenu && canResumeCurrentScene);
        }

        if (restartButtonObject != null) {
            restartButtonObject.SetActive(canRestartCurrentScene);
        }

        TMP_Text exitButtonText = GetExitButtonText();
        if (exitButtonText != null) {
            exitButtonText.text = isInMainMenu
                ? (pauseMenuView != null ? pauseMenuView.quitGameText : "Выйти из игры")
                : (pauseMenuView != null ? pauseMenuView.exitToMenuText : "В главное меню");
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

    bool IsPointerOverRestartButton()
    {
        if (restartButtonObject == null) return false;
        if (!restartButtonObject.TryGetComponent(out RectTransform restartButtonRect)) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(restartButtonRect, Input.mousePosition);
    }

    TMP_Text GetExitButtonText()
    {
        return pauseMenuView != null ? pauseMenuView.exitButtonText : null;
    }

    bool CanRestartCurrentScene()
    {
        return !IsInMainMenuScene() && GameManager.I != null && GameManager.I.IsSinglePlayer;
    }

    bool CanResume()
    {
        return !IsInMainMenuScene() && !ShouldShowGameOverMenu();
    }

    bool ShouldShowGameOverMenu()
    {
        return GameManager.I != null && GameManager.I.HasLoseInSinglePlayer;
    }

    void EnsurePauseMenuInstance()
    {
        if (pauseMenuView != null) return;

        if (pauseMenuPrefab == null) {
            Debug.LogWarning("Pause menu prefab is not assigned on GlobalUiController and could not be loaded from Resources/Prefabs/PauseMenu.");
            return;
        }

        foreach (Transform child in transform) {
            if (child == null) continue;
            if (child.GetComponent<PauseMenuView>() != null || child.name == "Pause") {
                Destroy(child.gameObject);
            }
        }

        pauseMenuView = Instantiate(pauseMenuPrefab, transform);
        pauseMenuView.name = pauseMenuPrefab.name;

        escapeMenuRoot = pauseMenuView.gameObject;
        resumeButtonObject = pauseMenuView.resumeButtonObject;
        restartButtonObject = pauseMenuView.restartButtonObject;
        exitButtonObject = pauseMenuView.exitButtonObject;

        if (resumeButtonObject != null && resumeButtonObject.TryGetComponent(out Button resumeButton)) {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(Resume);
        }

        if (restartButtonObject != null && restartButtonObject.TryGetComponent(out Button restartButton)) {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartButtonAction);
        }

        if (exitButtonObject != null && exitButtonObject.TryGetComponent(out Button exitButton)) {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitButtonAction);
        }
    }

    void ResetGameplaySingletons()
    {
        if (DealsManager.I != null) {
            DealsManager.I.DestroySingletonRoot();
        }

        if (GameManager.I != null) {
            GameManager.I.DestroySingletonRoot();
        }
    }

    bool IsEscapeMenuVisible => escapeMenuRoot != null && escapeMenuRoot.activeSelf;
}
