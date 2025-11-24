using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform target;
    public int requiredDiamonds = 0;
    public bool finishOnUse = false;
    public int finishRequiredDiamonds = 4;
    public EndGameController endController;

    public bool CanTeleport()
    {
        if (!finishOnUse && target == null) return false;
        if (GameManager.Instance == null) return true;
        return GameManager.Instance.diamondsCollected >= requiredDiamonds;
    }

    public void TeleportOrFinish(Transform player)
    {
        if (!CanTeleport()) return;

        if (finishOnUse && GameManager.Instance != null && GameManager.Instance.HasAllDiamonds(finishRequiredDiamonds))
        {
            if (endController == null) endController = FindObjectOfType<EndGameController>();
            if (endController != null)
            {
                endController.ShowEnd();
                return;
            }
        }

        if (target != null)
            player.position = target.position;
    }

    void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;
        TeleportOrFinish(other.transform);
    }
}
