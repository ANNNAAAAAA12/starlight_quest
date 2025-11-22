using UnityEngine;

public class DiamondPickupFollower : MonoBehaviour
{
    bool collected;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        var ctrl = other.GetComponent<AstronautController>();
        if (ctrl == null) return;
        collected = true;
        if (GameManager.Instance != null) GameManager.Instance.AddDiamond();
        gameObject.SetActive(false);
    }
}