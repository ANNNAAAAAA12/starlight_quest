using UnityEngine;

public class EndGameController : MonoBehaviour
{
    public GameObject endPanel;
    public string mainMenuSceneName;
    public SceneLoader loader;
    public int requiredDiamonds = 4;
    public bool autoShowOnDiamonds;
    bool shown;

    void Awake()
    {
        if (loader == null) loader = GetComponent<SceneLoader>();
        if (endPanel != null) endPanel.SetActive(false);
    }

    void Update()
    {
        if (autoShowOnDiamonds && !shown)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasAllDiamonds(requiredDiamonds))
            {
                ShowEnd();
            }
        }
    }

    public void ShowEnd()
    {
        shown = true;
        Time.timeScale = 0f;
        if (endPanel != null) endPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMainMenu()
    {
        if (loader == null) return;
        Time.timeScale = 1f;
        loader.LoadByName(mainMenuSceneName);
    }

    public void QuitGame()
    {
        if (loader == null) return;
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.ResetDiamonds();
        loader.ReloadCurrent();
    }
}
