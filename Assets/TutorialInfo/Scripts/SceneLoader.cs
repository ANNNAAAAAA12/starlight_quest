using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrent()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

