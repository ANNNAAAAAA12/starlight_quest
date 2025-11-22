using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int health = 1;
    public DiamondSpawner spawner;
    bool destroyed;

    public void Hit()
    {
        if (destroyed) return;
        health -= 1;
        if (health <= 0)
        {
            destroyed = true;
            if (spawner != null) spawner.Spawn();
            Destroy(gameObject);
        }
    }
}