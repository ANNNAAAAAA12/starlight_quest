using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera SetUp")]
    public Transform player;     
    public Transform cameraTarget;  
    public Vector3 shoulderoffset = new Vector3 (0.3f, 1.7f, -2f);         

    public float followSpeed = 10f;

    public float rotationSpeed = 5f; 

    public float mouseSensivity = 0.5f; 

     [Header("Orbita")]

     public float minPitch = -20f;
     
     public float maxPitch = 60f;

     private float yaw ;

     private float pitch ;

     private New_CharacterController playerController; 

     private Transform mainCamera; 

    void Start ()
    {
        if (player != null)
            playerController  = player.GetComponent<New_CharacterController>();
        if (cameraTarget == null)
            cameraTarget = player != null ? player : transform;
        var cam = Camera.main != null ? Camera.main.transform : null;
        mainCamera = cam != null ? cam : transform;

    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();

    }

    void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X")* mouseSensivity ;
        float mouseY = Input.GetAxis("Mouse Y")* mouseSensivity ; 

        if (playerController != null && playerController.IsMoving)
        {
            yaw += playerController.CurrentYaw;
        }
        else
        {
            yaw += mouseX * rotationSpeed;
        }

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


    }
    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = (cameraTarget != null ? cameraTarget.position : transform.position) + rotation * shoulderoffset;

        mainCamera.position = Vector3.Lerp(mainCamera.position, targetPosition, followSpeed * Time.deltaTime);
        if (cameraTarget != null)
            mainCamera.LookAt(cameraTarget);
    }



}

