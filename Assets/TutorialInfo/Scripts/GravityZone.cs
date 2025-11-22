using UnityEngine;

public class GravityZone : MonoBehaviour
{
    public float gravityValue = -9.81f;
    public float jumpHeightValue = 1.2f;

    void OnTriggerEnter(Collider other)
    {
        var ctrl = other.GetComponent<AstronautController>();
        if (ctrl == null) return;
        ctrl.gravity = gravityValue;
        ctrl.jumpHeight = jumpHeightValue;
    }
}