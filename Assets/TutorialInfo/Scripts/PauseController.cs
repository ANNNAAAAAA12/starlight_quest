using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;
    public string mainMenuSceneName;
    public SceneLoader loader;
    bool paused;
    AstronautController playerCtrl;

    void Awake()
    {
        if (loader == null) loader = GetComponent<SceneLoader>();
        if (pausePanel != null) pausePanel.SetActive(false);
        playerCtrl = FindObjectOfType<AstronautController>();
    }

    void Update()
    {
        if (MainMenuController.MenuActive) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(paused);
        if (playerCtrl == null) playerCtrl = FindObjectOfType<AstronautController>();
        if (playerCtrl != null) playerCtrl.enabled = !paused;
        if (playerCtrl != null) playerCtrl.SetGameplayCursor(!paused);
    }

    public void Resume()
    {
        if (!paused) return;
        TogglePause();
    }

    public void RestartLevel()
    {
        if (loader == null) return;
        Time.timeScale = 1f;
        loader.ReloadCurrent();
    }

    public void GoToMainMenu()
    {
        if (loader == null) return;
        Time.timeScale = 1f;
        loader.LoadByName(mainMenuSceneName);
    }
}
