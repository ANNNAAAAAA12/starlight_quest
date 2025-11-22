using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int livesPerLevel = 3;
    public int currentLives;

    void Start()
    {
        ResetLives();
    }

    public void ResetLives()
    {
        currentLives = livesPerLevel;
    }

    public void Damage(int amount)
    {
        if (currentLives <= 0) return;
        currentLives -= amount;
        if (currentLives <= 0)
        {
            currentLives = 0;
        }
    }
}