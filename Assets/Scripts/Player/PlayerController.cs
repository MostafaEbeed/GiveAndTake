using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource footstepSound;
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private CinemachineBasicMultiChannelPerlin m_impulseSource;
    public OrbBehavior CurrentOrb { get; set; }

    public CharacterController controller {  get; private set; }
    public CinemachineCamera virtualCamera;
    private PlatformPlayerController platformController;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [SerializeField] private float sprintTransitSpeed = 5f;
    [SerializeField] private float gravity = 0;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float rotSpeed = 0;

    [Header("Hand Bob Settings")]
    [SerializeField] private Transform handTransform;  
    [SerializeField] private float handBobFrequency = 5f;
    [SerializeField] private float handBobAmplitude = 0.05f;

    private float handBobTimer = 0f;
    private Vector3 handStartLocalPos;

    private Vector3 m_externalForce = Vector3.zero;
    private float m_externalForceDecay = 10f;

    public bool IsMovementLocked { get; set; } = false;
    public bool IsJumpLocked { get; set; } = false;
    public bool IsLookLocked { get; set; } = false;

    public float ExtraGravityMultiplier { get; set; } = 1f;
    public CinemachineBasicMultiChannelPerlin CameraImpulseSource => m_impulseSource;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float SprintSpeedMultiplier
    {
        get => sprintSpeedMultiplier;
        set => sprintSpeedMultiplier = value;
    }

    public float SprintTransitSpeed
    {
        get => sprintTransitSpeed;
        set => sprintTransitSpeed = value;
    }

    public float Gravity
    {
        get => gravity;
        set => gravity = value;
    }

    public float JumpHeight
    {
        get => jumpHeight;
        set => jumpHeight = value;
    }

    public float RotSpeed
    {
        get => rotSpeed;
        set => rotSpeed = value;
    }

    private float verticalVelocity;
    private float currentSpeed;
    public float currentSpeedMultiplier;
    private float xRotation;

    [Header("Camera Bob Settings")]

    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 70f;

    private float targetFOV;

    [SerializeField] private CinemachineImpulseListener noiseComponent;

    [Header("Recoil")]
    private Vector3 a_currentRecoil = Vector3.zero;

    [Header("Footstep Settings")]
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private float stepInterval = 1f;

    private float nextStepTimer = 0;

    [Header("Advanced Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool isJumping = false;
    
    [Header("SFX")]
    [SerializeField] private AudioClip[] groundFootsteps;
    [SerializeField] private AudioClip[] grassFootsteps;
    [SerializeField] private AudioClip[] gravelFootsteps;

    [Header("Input")]
    [SerializeField] private float mouseSensitivity;
    private float moveInput;
    private float turnInput;
    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        platformController = GetComponent<PlatformPlayerController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        handStartLocalPos = handTransform.localPosition;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        
        Movement();

        PlayFootstepSound();

        HandleHandBob();

    }

    private void Movement()
    {
        GroundMovement();
        Turn();
    }

    public void ApplyExternalForce(Vector3 force)
    {
        m_externalForce += force;
    }

    private void GroundMovement()
    {
        Vector3 move = IsMovementLocked ? Vector3.zero : new Vector3(turnInput, 0, moveInput);
        move = virtualCamera.transform.TransformDirection(move);

        if (!IsMovementLocked && Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeedMultiplier = sprintSpeedMultiplier * playerStates.Speed;
            targetFOV = sprintFOV;
        }
        else
        {
            currentSpeedMultiplier = IsMovementLocked ? 0f : 1f * playerStates.Speed;
            targetFOV = normalFOV;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * currentSpeedMultiplier, sprintTransitSpeed * Time.deltaTime);

        virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, targetFOV, sprintTransitSpeed * Time.deltaTime);

        move *= currentSpeed;
        
        move.y = VerticalForceCalculation();
        
        if (platformController.IsOnPlatform())
        {
            Vector3 platformMovement = platformController.GetPlatformVelocity();
            move += platformMovement;
        }

        Vector3 totalMovement = move + m_externalForce;

        totalMovement.y = verticalVelocity;

        if (!controller.isGrounded)
        {
            verticalVelocity -= gravity * ExtraGravityMultiplier * Time.deltaTime;
        }
        else if (verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }

        m_externalForce = Vector3.Lerp(m_externalForce, Vector3.zero, m_externalForceDecay * Time.deltaTime);

        if(controller.enabled)
            controller.Move(totalMovement * Time.deltaTime);
    }

    private void Turn()
    {
        if (IsLookLocked)
            return;

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        virtualCamera.transform.localRotation = Quaternion.Slerp(
            virtualCamera.transform.localRotation,
            Quaternion.Euler(xRotation + a_currentRecoil.y, a_currentRecoil.x, 0),
            rotSpeed * Time.deltaTime
        );

        transform.Rotate(Vector3.up * mouseX);
    }

    private void PlayFootstepSound()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            if (Time.time >= nextStepTimer)
            {
                AudioClip[] footstepClips = DetermineAudioClips();

                if (footstepClips.Length > 0)
                {
                    AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

                    //footstepSound.PlayOneShot(clip);
                    
                    AudioManager.Instance.PlaySFX(clip, this.transform.position);
                }

                nextStepTimer = Time.time + (stepInterval / currentSpeedMultiplier);
            }
        }
    }

    private AudioClip[] DetermineAudioClips()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.5f, terrainLayerMask))
        {
            string tag = hit.collider.tag;

            switch (tag)
            {
                case "Ground":
                    return groundFootsteps;
                case "Grass":
                    return grassFootsteps;
                case "Gravel":
                    return gravelFootsteps;
                default:
                    return groundFootsteps;
            }
        }
        return groundFootsteps;
    }

    public void ForceBounce(float jumpMultiplier = 1f)
    {
        jumpBufferTimer = jumpBufferTime;
        verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2f * jumpMultiplier);
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        StopAllCoroutines(); 
        StartCoroutine(DoCameraShake(m_impulseSource, amplitude, frequency, duration));
    }
    
    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            coyoteTimer = coyoteTime;
            isJumping = false;

            if (!IsJumpLocked && jumpBufferTimer > 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
                jumpBufferTimer = 0f;
                isJumping = true;
            }
            else
            {
                verticalVelocity = -1f;
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;

            if (!IsJumpLocked && jumpBufferTimer > 0f && coyoteTimer > 0f && !isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
                jumpBufferTimer = 0f;
                isJumping = true;
            }

            verticalVelocity -= gravity * ExtraGravityMultiplier * Time.deltaTime;
        }

        return verticalVelocity;
    }

    private void HandleHandBob()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f && !IsMovementLocked)
        {
            handBobTimer += Time.deltaTime * handBobFrequency;

            float bobOffsetY = Mathf.Sin(handBobTimer) * handBobAmplitude;
            float bobOffsetX = Mathf.Cos(handBobTimer * 0.5f) * handBobAmplitude * 0.5f;

            handTransform.localPosition = handStartLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
        }
        else
        {
            handTransform.localPosition = Vector3.Lerp(
                handTransform.localPosition,
                handStartLocalPos,
                Time.deltaTime * 5f
            );

            handBobTimer = 0f;
        }
    }


    private System.Collections.IEnumerator DoCameraShake(CinemachineBasicMultiChannelPerlin noise, float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    public void InputManagement(float moveInput, float turnInput)
    {
        this.moveInput = moveInput;
        this.turnInput = turnInput;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}
