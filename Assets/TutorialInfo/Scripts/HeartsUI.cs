using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    public PlayerHealth health;
    public Text heartsText;

    void Awake()
    {
        if (health == null) health = FindObjectOfType<PlayerHealth>();
        UpdateText();
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
        }
    }

    void OnDestroy()
    {
        if (health != null)
            health.HealthChanged -= OnHealthChanged;
    }

    void OnHealthChanged(int current, int max)
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (heartsText == null || health == null) return;
        int cur = Mathf.Max(0, health.currentLives);
        int max = Mathf.Max(cur, health.livesPerLevel);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < cur; i++) sb.Append('♥');
        for (int i = cur; i < max; i++) sb.Append('♡');
        heartsText.text = sb.ToString();
    }
}

