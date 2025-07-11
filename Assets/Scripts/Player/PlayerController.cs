using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource footstepSound;
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private CinemachineBasicMultiChannelPerlin m_impulseSource;
    public OrbBehavior CurrentOrb { get; set; }

    public CharacterController controller {  get; private set; }
    public CinemachineCamera virtualCamera;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [SerializeField] private float sprintTransitSpeed = 5f;
    [SerializeField] private float gravity = 0;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float rotSpeed = 0;

    private Vector3 m_externalForce = Vector3.zero;
    private float m_externalForceDecay = 10f;


    [Header("Advanced Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool isJumping = false;

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
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private float bobAmplitude = 1f;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        InputManagement();

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

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
    }

    private void LateUpdate()
    {
        CameraBob();
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

        Vector3 totalMovement = move + m_externalForce;
        controller.Move(totalMovement * Time.deltaTime);

        m_externalForce = Vector3.Lerp(m_externalForce, Vector3.zero, m_externalForceDecay * Time.deltaTime);

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

    private void CameraBob()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            noiseComponent.ReactionSettings.AmplitudeGain = bobAmplitude;
            noiseComponent.ReactionSettings.FrequencyGain = bobFrequency;
        }
        else
        {
            noiseComponent.ReactionSettings.AmplitudeGain = 1f;
            noiseComponent.ReactionSettings.FrequencyGain = .1f;
        }
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

                    footstepSound.PlayOneShot(clip);
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


    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        StopAllCoroutines(); 
        StartCoroutine(DoCameraShake(m_impulseSource, amplitude, frequency, duration));
    }

    private System.Collections.IEnumerator DoCameraShake(CinemachineBasicMultiChannelPerlin noise, float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    private void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}
