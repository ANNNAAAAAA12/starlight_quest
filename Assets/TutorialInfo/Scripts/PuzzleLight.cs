using UnityEngine;

public class PuzzleLight : MonoBehaviour
{
    public bool covered;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            covered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            covered = false;
        }
    }
}