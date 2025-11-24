using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int livesPerLevel = 3;
    public int currentLives;
    public bool isDead;
    public System.Action<int,int> HealthChanged;
    public System.Action Died;

    void Start()
    {
        ResetLives();
    }

    public void ResetLives()
    {
        currentLives = livesPerLevel;
        isDead = false;
        HealthChanged?.Invoke(currentLives, livesPerLevel);
    }

    public void Damage(int amount)
    {
        if (currentLives <= 0) return;
        currentLives -= amount;
        HealthChanged?.Invoke(currentLives, livesPerLevel);
        if (currentLives <= 0)
        {
            currentLives = 0;
            if (!isDead)
            {
                isDead = true;
                Died?.Invoke();
            }
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentLives += amount;
        if (currentLives > livesPerLevel) currentLives = livesPerLevel;
        HealthChanged?.Invoke(currentLives, livesPerLevel);
    }
}
