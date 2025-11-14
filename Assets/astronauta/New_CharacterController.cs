using UnityEngine;
[RequireComponent (typeof(CharacterController))]
public class New_CharacterController : MonoBehaviour
{
    [Header("Movimiento")]
    public float WalkSpeed = 4f;
    public float SprintSpeed = 6f;
    public float JumpHeight = 2f;
    public float RotationSpeed = 10f;

    public float mouseSensivity = 1f;

    public float gravity = -20f;

    [Header("Referenciación")]

    public Transform cameraTransform;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 Velocity;
    private float currentSpeed;
    private float yaw;
    private bool IsGrounded;


    private Vector3 externalVelocity = Vector3.zero;
    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public float CurrentYaw => yaw;




    
    void Start()
    {
        characterController = GetComponent<CharacterController>();


        
    }

    // Update is called once per frame
    void Update()
    {
       HandleMovement();
       HandleRotation();
       updateAnimator();
    }
    

    private void HandleMovement()
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
    IsMoving = inputDirection != Vector3.zero;

    Vector3 moveDirection = Vector3.zero;

    if (IsMoving)
    {
        // Dirección de la cámara en plano horizontal
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // Dirección final influenciada por la cámara
        moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // Velocidad
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;

        // Rotación hacia dirección del movimiento
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
    }
    else
    {
        currentSpeed = 0f;
    }

    // Movimiento horizontal
    Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;

    // Gravedad
    Velocity.y += gravity * Time.deltaTime;
    movement.y = Velocity.y * Time.deltaTime;

    characterController.Move(movement);

    if (characterController.isGrounded)
    {
        Velocity.y = -2f; // Mantener grounded
    }

    // Salto
    if (Input.GetButtonDown("Jump") && characterController.isGrounded)
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
    }
}





    void HandleRotation()
    {
        float mouseX= Input.GetAxis("Mouse X") * mouseSensivity;
        yaw += mouseX; 

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yaw, 0f), RotationSpeed* Time.deltaTime);
        }
    }

    void updateAnimator()
    {
        float SpeedPercent = IsMoving ? (currentSpeed == SprintSpeed? 1f : 0.5f) :0f;
        animator?.SetFloat("Speed", SpeedPercent, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", Velocity.y);
    }


    }

