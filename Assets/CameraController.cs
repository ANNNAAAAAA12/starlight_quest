using UnityEngine;

// Asegura que este objeto tenga un componente Camera
[RequireComponent(typeof(Camera))] 
public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    // El objeto que la cámara seguirá (tu astronauta)
    public Transform playerTarget; 
    
    // Este script ya está en el CameraTarget, por lo que su propio transform es el punto de pivote.
    // No necesitamos una referencia pública al CameraTarget si este script está en él.
    
    [Header("Configuración")]
    public float distance = 5.0f;       // Distancia de la cámara al CameraTarget
    public float mouseSensitivity = 3.0f; // Sensibilidad de rotación vertical
    public float smoothSpeed = 10f;     // Suavizado del movimiento de seguimiento
    public float minYAngle = -35.0f;    // Límite inferior de rotación vertical
    public float maxYAngle = 60.0f;     // Límite superior de rotación vertical

    private float currentY = 0.0f; // Almacena la rotación vertical (mouse Y)

    // La rotación horizontal (Mouse X) se maneja en el script del astronauta.
    // Aquí solo necesitamos la rotación vertical para la cámara.

    void Start()
    {
        // Asegurarse de que el cursor esté bloqueado y oculto (ya lo tienes en el astronauta, pero no está de más aquí)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Obtener Input de Rotación Vertical (Mouse Y)
        // El signo '-' invierte la rotación para que mirar arriba/abajo se sienta natural
        currentY += Input.GetAxis("Mouse Y") * -mouseSensitivity; 
        
        // 2. Limitar la rotación vertical (mirar arriba/abajo)
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        // LateUpdate se asegura de que el movimiento de la cámara ocurra DESPUÉS de que el 
        // personaje ha terminado de moverse y rotar en Update/FixedUpdate.

        if (playerTarget == null) return; // Salir si no hay un target asignado

        // El propio transform de este GameObject (CameraTarget) es el punto de pivote.
        // Rotar el CameraTarget verticalmente basado en el mouse Y.
        // La rotación horizontal del CameraTarget ya la maneja el astronauta (su padre).
        Quaternion pivotRotation = playerTarget.rotation * Quaternion.Euler(currentY, 0, 0);

        // Calcula la posición final de la cámara (a 'distance' unidades detrás del playerTarget)
        Vector3 targetPosition = playerTarget.position - pivotRotation * Vector3.forward * distance;

        // Aplicar Suavizado (Smooth follow) a la posición
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Hace que la cámara siempre mire al playerTarget (el astronauta)
        transform.LookAt(playerTarget);
    }
}