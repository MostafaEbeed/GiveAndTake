using UnityEngine;

public class VerticalPlatform : MovingPlatform
{
    [Header("Vertical Settings")]
    public Direction verticalDirection = Direction.Up;
    
    public enum Direction
    {
        Up,
        Down
    }
    
    protected override void CalculateTargetPosition()
    {
        Vector3 direction = verticalDirection == Direction.Up ? Vector3.up : Vector3.down;
        targetPosition = startPosition + direction * moveDistance;
    }
    
    public override Vector3 GetMoveDirection()
    {
        return verticalDirection == Direction.Up ? Vector3.up : Vector3.down;
    }
}