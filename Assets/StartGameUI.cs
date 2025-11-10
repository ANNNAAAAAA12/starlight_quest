using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    private GameObject menuUI; 

    void Start()
    {
        
        menuUI = GameObject.Find("MenuUI");

        if (menuUI == null)
        {
            Debug.LogWarning("No se encontró el objeto llamado 'MenuUI'. Verifica el nombre exacto en la jerarquía.");
        }
    }

    public void EmpezarJuego()
    {
        Debug.Log("Botón Empezar presionado");

        if (menuUI != null)
        {
            menuUI.SetActive(false);
            Debug.Log("Menú ocultado automáticamente");
        }
        else
        {
            Debug.LogWarning("No se pudo ocultar el menú porque no se encontró el objeto 'MenuUI'");
        }
    }
}



