using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName;
    public SceneLoader loader;
    public bool overlayMode = true;
    public GameObject rootCanvas;
    AstronautController playerCtrl;
    public static bool MenuActive;

    void Awake()
    {
        if (loader == null) loader = GetComponent<SceneLoader>();
        var c = GetComponentInParent<Canvas>();
        if (rootCanvas == null && c != null) rootCanvas = c.gameObject;
        playerCtrl = FindObjectOfType<AstronautController>();
        if (overlayMode)
        {
            Time.timeScale = 0f;
            if (playerCtrl != null) playerCtrl.enabled = false;
            if (playerCtrl != null) playerCtrl.SetGameplayCursor(false);
            MenuActive = true;
        }
    }

    public void StartGame()
    {
        if (overlayMode || string.IsNullOrEmpty(gameSceneName) || gameSceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            Time.timeScale = 1f;
            if (playerCtrl == null) playerCtrl = FindObjectOfType<AstronautController>();
            if (playerCtrl != null) playerCtrl.enabled = true;
            if (playerCtrl != null) playerCtrl.SetGameplayCursor(true);
            if (rootCanvas != null)
            {
                rootCanvas.SetActive(false);
            }
            else
            {
                var canvases = Object.FindObjectsOfType<UnityEngine.Canvas>(true);
                for (int i = 0; i < canvases.Length; i++)
                {
                    if (canvases[i].GetComponentInChildren<MainMenuController>(true) != null)
                        canvases[i].gameObject.SetActive(false);
                }
            }
            MenuActive = false;
            return;
        }
        if (loader == null) return;
        loader.LoadByName(gameSceneName);
    }

    public void QuitGame()
    {
        if (loader == null) return;
        loader.Quit();
    }
}
