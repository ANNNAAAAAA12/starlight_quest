using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuUI;  // Arrastra tu objeto "MenuUI" aquí

    public void CerrarMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MenuUI no está asignado en el inspector.");
        }
    }
}
