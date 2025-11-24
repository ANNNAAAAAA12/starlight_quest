using UnityEngine;

public class DeathPanelController : MonoBehaviour
{
    public PlayerHealth health;
    public GameObject deathPanel;
    public SceneLoader loader;
    public float autoRestartDelay = 2f;
    float timer;

    void Awake()
    {
        if (loader == null) loader = GetComponent<SceneLoader>();
        if (health == null) health = FindObjectOfType<PlayerHealth>();
        if (deathPanel != null) deathPanel.SetActive(false);
        if (health != null) health.Died += OnDied;
    }

    void OnDestroy()
    {
        if (health != null) health.Died -= OnDied;
    }

    void OnDied()
    {
        timer = autoRestartDelay;
        if (deathPanel != null) deathPanel.SetActive(true);
        Time.timeScale = 0f;
        var ctrl = FindObjectOfType<AstronautController>();
        if (ctrl != null) ctrl.enabled = false;
        if (ctrl != null) ctrl.SetGameplayCursor(false);
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0f)
            {
                Time.timeScale = 1f;
                if (loader != null) loader.ReloadCurrent();
            }
        }
    }
}

