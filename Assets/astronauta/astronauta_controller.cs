using UnityEngine;
[RequireComponent (typeof(CharacterController))]

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Movimiento ")]
    public float WalkSpeed = 4f;
    public float SprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;

    public float mouseSensitivity = 1f;
    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 Velocity;
    private float currentSpeed;
    private float yaw;
    private Vector3 externalVelocity = Vector3.zero;

    public bool IsMoving { get; private set; }
   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
