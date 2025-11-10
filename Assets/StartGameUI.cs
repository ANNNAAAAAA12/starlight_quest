using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    // Hacemos que sea pública para asignarla arrastrando en el Inspector
    public GameObject menuUI; 

    // Ya no necesitamos el método Start() para buscar el objeto.
    /*
    void Start()
    {
        // Se puede eliminar este método si asignas la variable menuUI en el Inspector
    }
    */

   public void EmpezarJuego()
{
    // 1. Ocultar el menú (como ya lo tienes)
    if (menuUI != null)
    {
        menuUI.SetActive(false);
    }
    
    // 2. BUSCAR y ACTIVAR el script del astronauta
    GameObject astronauta = GameObject.Find("astronautaterminadonickz (3) 1"); // Usa el nombre exacto de la jerarquía
    if (astronauta != null)
    {
        // Esto activa el script, que a su vez bloqueará el cursor (si esa línea está en su Start/Update)
        astronauta.GetComponent<AstronautaController>().enabled = true; 
    }
    
    // 3. (Opcional) Asegurarse de que el cursor se oculte si es necesario
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}
}



