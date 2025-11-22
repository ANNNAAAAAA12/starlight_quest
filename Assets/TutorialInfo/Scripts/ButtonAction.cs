using UnityEngine;

public enum ButtonActionType { Damage, RevealDiamond }

public class ButtonAction : MonoBehaviour
{
    public ButtonActionType type = ButtonActionType.Damage;
    public int damageAmount = 1;
    public GameObject diamondToReveal;

    void OnTriggerEnter(Collider other)
    {
        var ctrl = other.GetComponent<AstronautController>();
        if (ctrl == null) return;

        if (type == ButtonActionType.Damage)
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null) health.Damage(damageAmount);
            return;
        }

        if (type == ButtonActionType.RevealDiamond)
        {
            if (diamondToReveal == null) return;
            diamondToReveal.SetActive(true);
        }
    }
}