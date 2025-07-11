using UnityEngine;

// Base platform class
public abstract class MovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    public bool autoMove = false;
    public float autoMoveDelay = 1f;
    
    [Header("Player Integration")]
    public LayerMask playerLayer = 1;
    public float playerDetectionHeight = 1f;
    
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected Vector3 lastPosition;
    protected bool isMoving = false;
    protected bool isAtTarget = false;
    
    [Header("Debug")]
    public bool showGizmos = true;
    
    // Track players on this platform
    private System.Collections.Generic.HashSet<PlatformPlayerController> playersOnPlatform = 
        new System.Collections.Generic.HashSet<PlatformPlayerController>();
    
    protected virtual void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
        CalculateTargetPosition();
        
        if (autoMove)
        {
            InvokeRepeating(nameof(ToggleMove), autoMoveDelay, autoMoveDelay);
        }
    }
    
    protected virtual void Update()
    {
        Vector3 previousPosition = transform.position;
        
        if (isMoving)
        {
            MovePlatform();
        }
        
        // Calculate movement delta
        Vector3 platformDelta = transform.position - lastPosition;
        
        // Move all players on this platform
        if (platformDelta.magnitude > 0.001f)
        {
            foreach (var player in playersOnPlatform)
            {
                if (player != null)
                {
                    player.MovePlatformDelta(platformDelta);
                }
            }
        }
        
        lastPosition = transform.position;
    }
    
    protected abstract void CalculateTargetPosition();
    
    protected virtual void MovePlatform()
    {
        Vector3 destination = isAtTarget ? startPosition : targetPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            transform.position = destination;
            isMoving = false;
            isAtTarget = !isAtTarget;
        }
    }
    
    public virtual void ToggleMove()
    {
        if (!isMoving)
        {
            isMoving = true;
        }
    }
    
    // Player registration methods
    public void RegisterPlayer(PlatformPlayerController player)
    {
        playersOnPlatform.Add(player);
    }
    
    public void UnregisterPlayer(PlatformPlayerController player)
    {
        playersOnPlatform.Remove(player);
    }
    
    public bool HasPlayer(PlatformPlayerController player)
    {
        return playersOnPlatform.Contains(player);
    }
    
    // Get platform velocity for player physics
    public Vector3 GetPlatformVelocity()
    {
        if (isMoving)
        {
            Vector3 destination = isAtTarget ? startPosition : targetPosition;
            return (destination - transform.position).normalized * moveSpeed;
        }
        return Vector3.zero;
    }
    
    protected virtual void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        
        // Draw current position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(start, GetComponent<Collider>().bounds.size);
        
        // Draw target position
        Gizmos.color = Color.red;
        Vector3 target = start + GetMoveDirection() * moveDistance;
        Gizmos.DrawWireCube(target, GetComponent<Collider>().bounds.size);
        
        // Draw path
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, target);
    }
    
    public abstract Vector3 GetMoveDirection();
}