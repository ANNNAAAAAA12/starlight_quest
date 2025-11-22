using UnityEngine;

public class HazardFloor : MonoBehaviour
{
    public int damagePerTick = 1;
    public float tickInterval = 0.7f;
    float timer;

    void OnTriggerStay(Collider other)
    {
        var health = other.GetComponent<PlayerHealth>();
        if (health == null) return;
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            timer = 0f;
            health.Damage(damagePerTick);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            timer = 0f;
        }
    }
}