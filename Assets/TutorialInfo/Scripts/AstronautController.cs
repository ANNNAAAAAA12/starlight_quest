using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class AstronautController : MonoBehaviour
{
    public Animator animator;
    public Transform cameraTransform;
    public float walkSpeed = 2.5f;
    public float runSpeed = 6f;
    public float rotationSpeed = 720f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public bool lockAndHideCursor = true;

    public float jumpPreDelay = 0.20f;
    float jumpPreDelayTimer = 0f;
    bool jumpingPreDelay = false;

    public float pushForce = 8f;
    public float pushDistance = 1.6f;
    public LayerMask groundLayers = ~0;

    public float groundCheckRadius = 0.4f;
    public float groundCheckOffset = 0.6f;
    public float groundCheckDistance = 0.8f;

    // Animator parameters names
    public string speedParam = "Speed";
    public string groundedParam = "IsGrounded";
    public string runningParam = "IsRunning";
    public string pushingParam = "IsPushing";
    public string isJumpingParam = "isJumping";
    public string blendParam = "Blend";
    public string landingParam = "Landing";
    public string fallingParam = "IsFalling";

    // Punch
    public string punchParam = "Punch";

    CharacterController controller;
    Vector3 velocity;
    bool isRunning;
    bool isGrounded;
    bool isPushing;
    float baseStepOffset;
    float jumpGroundTimer;
    public float jumpGroundDisableTime = 0.12f;
    bool wasGrounded;

    bool isLanding = false;
    float landingLockTimer = 0f;
    public float landingDuration = 0.20f;

    float teleportLockTimer = 0f;
    public float teleportLockDuration = 0.35f;
    public float gravityZoneCheckRadius = 0.8f;
    public LayerMask gravityZoneLayers = ~0;

    // Hashes
    int speedHash, groundedHash, runningHash;
    int pushingHash, isJumpingHash, blendHash;
    int landingHash, fallingHash;
    int punchHash;

    // Cached parameters
    System.Collections.Generic.HashSet<string> animatorParams;
    bool hasSpeed, hasGrounded, hasRunning;
    bool hasPushing, hasIsJumping, hasBlend;
    bool hasLanding, hasFalling, hasPunch;

    public float fallingVelocityThreshold = -0.05f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        baseStepOffset = controller.stepOffset;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        CacheAnimatorParams();

        if (lockAndHideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        GroundCheck();
        HandleMovementAndRotation();
        JumpLogic();
        ApplyGravityAndMove();
        PushLogic();
        PunchLogic();
        UpdateAnimatorState();

        if (isLanding)
        {
            landingLockTimer -= Time.deltaTime;
            if (landingLockTimer <= 0f)
                isLanding = false;
        }

        if (teleportLockTimer > 0f)
            teleportLockTimer -= Time.deltaTime;

        CheckGravityZones();
    }

    Vector3 moveDir;
    float inputMag;

    void HandleMovementAndRotation()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();

        moveDir = (camF * v + camR * h).normalized;
        inputMag = Mathf.Clamp01(moveDir.magnitude);

        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (moveDir.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (isGrounded && !jumpingPreDelay && !animator.GetBool(isJumpingHash) && !isLanding)
        {
            float locomotionValue = inputMag > 0.01f ? (isRunning ? 1f : 0.28f) : 0f;

            if (hasSpeed) animator.SetFloat(speedHash, locomotionValue);
            if (hasBlend) animator.SetFloat(blendHash, locomotionValue);
        }
    }

    void GroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (jumpGroundTimer > 0f)
        {
            jumpGroundTimer -= Time.deltaTime;
            isGrounded = false;
        }

        if (!isGrounded)
        {
            Vector3 origin = transform.position + Vector3.up * groundCheckOffset;

            if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down,
                out var hit, groundCheckDistance, groundLayers))
            {
                if (jumpGroundTimer <= 0f)
                    isGrounded = true;
            }
        }

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        controller.stepOffset = isGrounded ? baseStepOffset : 0f;
    }

    void JumpLogic()
    {
        if (isLanding)
            return;

        // Start jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpingPreDelay = true;
            jumpPreDelayTimer = jumpPreDelay;

            if (hasIsJumping) animator.SetBool(isJumpingHash, true);
            if (hasFalling) animator.SetBool(fallingHash, false);

            if (hasSpeed) animator.SetFloat(speedHash, 0f);
            if (hasBlend) animator.SetFloat(blendHash, 0f);
        }

        // Pre delay
        if (jumpingPreDelay)
        {
            jumpPreDelayTimer -= Time.deltaTime;

            if (jumpPreDelayTimer <= 0f)
            {
                jumpingPreDelay = false;

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpGroundTimer = jumpGroundDisableTime;
                isGrounded = false;
            }
        }

        // Falling
        if (!isGrounded && velocity.y < fallingVelocityThreshold)
        {
            if (hasFalling) animator.SetBool(fallingHash, true);
        }

        // Landing
        if (!wasGrounded && isGrounded)
        {
            if (hasLanding) animator.SetTrigger(landingHash);
            if (hasIsJumping) animator.SetBool(isJumpingHash, false);
            if (hasFalling) animator.SetBool(fallingHash, false);

            isLanding = true;
            landingLockTimer = landingDuration;

            if (hasSpeed) animator.SetFloat(speedHash, 0f);
            if (hasBlend) animator.SetFloat(blendHash, 0f);
        }

        wasGrounded = isGrounded;
    }

    void ApplyGravityAndMove()
    {
        velocity.y += gravity * Time.deltaTime;

        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 horizontalMove = moveDir * speed * inputMag;
        controller.Move(horizontalMove * Time.deltaTime);
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    void PushLogic()
    {
        isPushing = false;

        if (Input.GetKey(KeyCode.E))
        {
            Vector3 origin = transform.position + Vector3.up * 0.3f;  // Más bajo
            Vector3 dir = transform.forward;

            Debug.DrawRay(origin, dir * pushDistance, Color.red);

            if (Physics.Raycast(origin, dir, out var hit, pushDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(transform.forward * pushForce, ForceMode.Force);
                    isPushing = true;
                }
            }
        }
    }


   void PunchLogic()
{
    if (!hasPunch) return;

    bool punchPressed = false;
#if ENABLE_INPUT_SYSTEM
    if (Mouse.current != null)
        punchPressed = Mouse.current.leftButton.wasPressedThisFrame;
#endif
    if (!punchPressed)
        punchPressed = Input.GetMouseButtonDown(0);

    if (punchPressed)
    {
        if (hasSpeed) animator.SetFloat(speedHash, 0f);
        if (hasBlend) animator.SetFloat(blendHash, 0f);

        animator.Update(0f);

        animator.SetTrigger(punchHash);

        moveDir = Vector3.zero;
        inputMag = 0f;

        Vector3 origin = transform.position + Vector3.up * 0.9f;
        Vector3 dir = transform.forward;
        if (Physics.Raycast(origin, dir, out var hit, 1.8f))
        {
            var br = hit.collider.GetComponentInParent<Breakable>();
            if (br != null) br.Hit();
        }
    }
}

    void UpdateAnimatorState()
    {
        if (hasGrounded) animator.SetBool(groundedHash, isGrounded);
        if (hasRunning) animator.SetBool(runningHash, isRunning);
        if (hasPushing) animator.SetBool(pushingHash, isPushing);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var gz = hit.collider.GetComponent<GravityZone>();
        if (gz != null)
        {
            gravity = gz.gravityValue;
            jumpHeight = gz.jumpHeightValue;
        }

        var tp = hit.collider.GetComponent<Teleporter>();
        if (tp != null)
        {
            if (teleportLockTimer > 0f) return;
            if (tp.target == null) return;
            transform.position = tp.target.position;
            teleportLockTimer = teleportLockDuration;
        }
    }

    void CheckGravityZones()
    {
        var hits = Physics.OverlapSphere(transform.position, gravityZoneCheckRadius, gravityZoneLayers, QueryTriggerInteraction.Collide);
        for (int i = 0; i < hits.Length; i++)
        {
            var gz = hits[i].GetComponent<GravityZone>();
            if (gz != null)
            {
                gravity = gz.gravityValue;
                jumpHeight = gz.jumpHeightValue;
                break;
            }
        }
    }

    

    void CacheAnimatorParams()
    {
        animatorParams = new System.Collections.Generic.HashSet<string>();

        foreach (var p in animator.parameters)
            animatorParams.Add(p.name);

        hasSpeed = animatorParams.Contains(speedParam);
        hasGrounded = animatorParams.Contains(groundedParam);
        hasRunning = animatorParams.Contains(runningParam);
        hasPushing = animatorParams.Contains(pushingParam);
        hasIsJumping = animatorParams.Contains(isJumpingParam);
        hasBlend = animatorParams.Contains(blendParam);
        hasLanding = animatorParams.Contains(landingParam);
        hasFalling = animatorParams.Contains(fallingParam);
        hasPunch = animatorParams.Contains(punchParam);

        speedHash = Animator.StringToHash(speedParam);
        groundedHash = Animator.StringToHash(groundedParam);
        runningHash = Animator.StringToHash(runningParam);
        pushingHash = Animator.StringToHash(pushingParam);
        isJumpingHash = Animator.StringToHash(isJumpingParam);
        blendHash = Animator.StringToHash(blendParam);
        landingHash = Animator.StringToHash(landingParam);
        fallingHash = Animator.StringToHash(fallingParam);
        punchHash = Animator.StringToHash(punchParam);
    }
}
