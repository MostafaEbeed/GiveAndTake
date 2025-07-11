using UnityEngine;

public class PlatformPlayerController : MonoBehaviour
{
    [Header("Platform Detection")]
    public float groundCheckDistance = 0.2f;
    public LayerMask platformLayer = 1;
    public float platformStickDistance = 0.5f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private CharacterController characterController;
    private MovingPlatform currentPlatform;
    private Vector3 platformVelocity;
    private bool isOnPlatform = false;
    
    // For smooth platform movement
    private Vector3 accumulatedPlatformMovement;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("PlatformPlayerController requires a CharacterController component!");
        }
    }
    
    void Update()
    {
        DetectPlatform();
        UpdatePlatformMovement();
    }
    
    void DetectPlatform()
    {
        // Cast a ray downward to detect platforms
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance + 0.1f, platformLayer))
        {
            MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();
            
            if (platform != null)
            {
                // Check if we're actually on the platform (not just hitting its side)
                if (hit.point.y <= transform.position.y - (characterController.height * 0.5f) + platformStickDistance)
                {
                    if (currentPlatform != platform)
                    {
                        // Left old platform, joined new platform
                        if (currentPlatform != null)
                        {
                            currentPlatform.UnregisterPlayer(this);
                        }
                        
                        currentPlatform = platform;
                        currentPlatform.RegisterPlayer(this);
                        isOnPlatform = true;
                    }
                }
                else
                {
                    // We're too far from the platform surface
                    LeavePlatform();
                }
            }
            else
            {
                // Hit something that's not a platform
                LeavePlatform();
            }
        }
        else
        {
            // No platform detected
            LeavePlatform();
        }
    }
    
    void LeavePlatform()
    {
        if (currentPlatform != null)
        {
            currentPlatform.UnregisterPlayer(this);
            currentPlatform = null;
            isOnPlatform = false;
            platformVelocity = Vector3.zero;
        }
    }
    
    void UpdatePlatformMovement()
    {
        if (isOnPlatform && currentPlatform != null)
        {
            platformVelocity = currentPlatform.GetPlatformVelocity();
        }
        else
        {
            platformVelocity = Vector3.zero;
        }
    }
    
    // Called by the platform when it moves
    public void MovePlatformDelta(Vector3 delta)
    {
        if (isOnPlatform && characterController != null)
        {
            // Accumulate platform movement
            accumulatedPlatformMovement += delta;
        }
    }
    
    // Call this method in your FPS controller's movement update
    public Vector3 GetPlatformMovement()
    {
        Vector3 movement = accumulatedPlatformMovement;
        accumulatedPlatformMovement = Vector3.zero;
        return movement;
    }
    
    // Get platform velocity for physics calculations
    public Vector3 GetPlatformVelocity()
    {
        //return platformVelocity;

        return currentPlatform.GetPlatformVelocity();
    }
    
    public bool IsOnPlatform()
    {
        return isOnPlatform;
    }
    
    public MovingPlatform GetCurrentPlatform()
    {
        return currentPlatform;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;
        
        // Draw ground check ray
        Gizmos.color = isOnPlatform ? Color.green : Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawRay(rayStart, Vector3.down * (groundCheckDistance + 0.1f));
        
        // Draw platform velocity
        if (isOnPlatform && platformVelocity.magnitude > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, platformVelocity);
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"On Platform: {isOnPlatform}");
            if (currentPlatform != null)
            {
                GUI.Label(new Rect(10, 30, 200, 20), $"Platform: {currentPlatform.name}");
            }
        }
    }
#endif
}