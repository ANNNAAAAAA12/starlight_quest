using UnityEngine;

public class LightCover : MonoBehaviour
{
    public bool covered;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            covered = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
            covered = false;
    }
}