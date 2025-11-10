using UnityEngine;

// Asegura que el objeto tenga un Rigidbody para la física
[RequireComponent(typeof(Rigidbody))] 
public class AstronautaController : MonoBehaviour
{
    // VARIABLES PÚBLICAS
    [Header("Movimiento")]
    public float WalkSpeed = 4f;
    public float SprintSpeed = 8f;
    public float rotationSpeed = 10f;
    
    // El Ground Check y el Salto ya no son necesarios si solo usas Locomoción.
    // Los quitamos para simplificar.

    // Referencias
    private Rigidbody rb;
    private Animator animator;
    
    // Variables de Estado
    private float currentSpeed;
    private Vector3 currentVelocity; // Velocidad XZ para el Animator

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb == null || animator == null)
        {
            Debug.LogError("Faltan componentes Rigidbody o Animator en el astronauta.");
        }
        
        // El bloqueo del cursor lo activa el menú. Si el script se inicia, bloquea.
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    // FIXEDUPDATE: Manejo de la física (movimiento)
    void FixedUpdate()
    {
        if (rb != null)
        {
            HandleMovement();
        }
    }
    
    // UPDATE: Manejo de la Rotación y Animaciones
    void Update()
    {
        if (animator != null)
        {
            HandleRotation();
            UpdateAnimator(); // Actualiza el parámetro Speed para el Blend Tree
        }
    }
    
    // LÓGICA DE MOVIMIENTO (W, A, S, D)
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 1. Determinar la velocidad actual (Walk o Sprint con LeftShift)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;

        // 2. Calcular dirección de movimiento (Relativa al personaje)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 moveDirection = (forward * verticalInput + right * horizontalInput).normalized * currentSpeed;

        // 3. Aplicar movimiento, MANTENIENDO la velocidad vertical (rb.velocity.y) para la gravedad.
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        
        // 4. Guardar la velocidad para el Blend Tree
        currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }
    
    // LÓGICA DE ROTACIÓN (Horizontal del personaje con el mouse)
    private void HandleRotation()
    {
        // Rotación del Personaje (basada en el input del ratón en el eje X)
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
    }

    // LÓGICA DE ANIMACIÓN (Locomoción: Estático/Caminar/Correr)
    private void UpdateAnimator()
    {
        // Calcula la magnitud (0 a 1) para el parámetro Speed
        float currentMovementMagnitude = currentVelocity.magnitude / SprintSpeed;
        
        // Establece el valor del parámetro "Speed" en el Animator
        animator.SetFloat("Speed", currentMovementMagnitude); 
    }
}