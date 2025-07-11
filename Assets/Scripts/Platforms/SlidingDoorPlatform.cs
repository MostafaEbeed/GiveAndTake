using UnityEngine;

public class SlidingDoorPlatform : MovingPlatform
{
    [Header("Sliding Door Settings")]
    public Direction slideDirection = Direction.Up;
    public bool startClosed = true;
    
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    
    protected override void Start()
    {
        base.Start();
        
        if (startClosed)
        {
            // Start at closed position (target position)
            transform.position = targetPosition;
            isAtTarget = true;
        }
    }
    
    protected override void CalculateTargetPosition()
    {
        Vector3 direction = GetSlideDirection();
        targetPosition = startPosition + direction * moveDistance;
    }
    
    private Vector3 GetSlideDirection()
    {
        switch (slideDirection)
        {
            case Direction.Up: return Vector3.up;
            case Direction.Down: return Vector3.down;
            case Direction.Left: return Vector3.left;
            case Direction.Right: return Vector3.right;
            default: return Vector3.up;
        }
    }
    
    public override Vector3 GetMoveDirection()
    {
        return GetSlideDirection();
    }
    
    [ContextMenu("Open Door Platrom")]
    public void OpenDoor()
    {
        if (isAtTarget && !isMoving) // Currently closed
        {
            isMoving = true;
        }
    }
    
    [ContextMenu("Close Door Platrom")]
    public void CloseDoor()
    {
        if (!isAtTarget && !isMoving) // Currently open
        {
            isMoving = true;
        }
    }
    
    public bool IsClosed()
    {
        return isAtTarget;
    }
}