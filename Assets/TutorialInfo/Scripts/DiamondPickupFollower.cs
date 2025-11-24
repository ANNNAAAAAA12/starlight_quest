using UnityEngine;

public class DiamondPickupFollower : MonoBehaviour
{
    bool collected;
    public enum EffectType { Good, Damage, Heal, Random }
    public EffectType effect = EffectType.Good;
    public int damageAmount = 1;
    public int healAmount = 1;
    Renderer rend;

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        rend = GetComponentInChildren<Renderer>();
        if (effect == EffectType.Random)
        {
            int r = Random.Range(0, 3);
            effect = r == 0 ? EffectType.Good : (r == 1 ? EffectType.Damage : EffectType.Heal);
        }
        ApplyVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        var ctrl = other.GetComponent<AstronautController>();
        if (ctrl == null) return;
        collected = true;

        switch (effect)
        {
            case EffectType.Good:
                if (GameManager.Instance != null) GameManager.Instance.AddDiamond();
                break;
            case EffectType.Damage:
                var phD = FindObjectOfType<PlayerHealth>();
                if (phD != null) phD.Damage(damageAmount);
                break;
            case EffectType.Heal:
                var phH = FindObjectOfType<PlayerHealth>();
                if (phH != null) phH.Heal(healAmount);
                break;
        }

        gameObject.SetActive(false);
    }

    void ApplyVisual()
    {
        if (rend == null) return;
        Color c = Color.cyan;
        if (effect == EffectType.Damage) c = Color.red;
        else if (effect == EffectType.Heal) c = Color.green;
        if (rend.material != null) rend.material.color = c;
    }
}
