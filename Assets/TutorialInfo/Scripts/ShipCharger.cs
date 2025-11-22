using UnityEngine;

public class ShipCharger : MonoBehaviour
{
    public int requiredDiamonds = 4;
    public GameObject ship;

    void OnTriggerStay(Collider other)
    {
        var ctrl = other.GetComponent<AstronautController>();
        if (ctrl == null) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.HasAllDiamonds(requiredDiamonds)) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ship != null) ship.SetActive(true);
        }
    }
}