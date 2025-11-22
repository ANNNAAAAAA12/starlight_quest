using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform target;

    void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;
        other.transform.position = target != null ? target.position : other.transform.position;
    }
}