using UnityEngine;

public class HorizontalPlatform : MovingPlatform
{
    [Header("Horizontal Settings")]
    public Direction horizontalDirection = Direction.Right;
    
    public enum Direction
    {
        Left,
        Right
    }
    
    protected override void CalculateTargetPosition()
    {
        Vector3 direction = horizontalDirection == Direction.Right ? Vector3.right : Vector3.left;
        targetPosition = startPosition + direction * moveDistance;
    }
    
    public override Vector3 GetMoveDirection()
    {
        return horizontalDirection == Direction.Right ? Vector3.right : Vector3.left;
    }
}